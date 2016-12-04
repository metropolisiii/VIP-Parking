using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using VIP_Parking.Models.Database;
using VIP_Parking.ViewModels;

namespace VIP_Parking.Helpers
{
    public static class ReservationsHelper
    {
        //Update the reservation model from viewModel data
        public static int UpdateReservation(VIPPARKINGEntities1 db, Reservation reservation, ReservationVM viewModel, int userID, string waiting_list, byte approve = 0)
        {
            bool update = false;
            int gatecode = 0;
            int eventID = 0;

            DateTime start_time, end_time;

            //We're either updating or inserting a reservation
            if (reservation.Reserv_ID != 0)
                update = true;

            //Make sure the num of slots the user is asking for does not exceed the number of slots available. If it does, display the number of slots available.
            bool isReserveable = false;
            if (approve == 0 || approve == 2)
                isReserveable = true;
            else
                isReserveable = NumSlotsHelper.isReserveable(viewModel);

            //If we're creating a reservation, the requester's id won't be set, so we set it with the current user's id           
            if (viewModel.Requester_ID == 0)
                reservation.Requester_ID = userID;
            else
                reservation.Requester_ID = viewModel.Requester_ID;
            reservation.RecipientName = viewModel.RecipientName;
            reservation.NumOfSlots = viewModel.NumOfSlots;
            reservation.RecipientEmail = viewModel.RecipientEmail;
            reservation.Category_ID = viewModel.Category_ID;
            var date = DateTime.ParseExact(viewModel.Date, "MM/dd/yyyy", null);
            
            //Get the event. If one doesn't exist create one.
            if (viewModel.Event != null && isReserveable)
               eventID = EventsHelper.GetOrCreateEvent(viewModel.Event, date);
            
            //Get the current gate code
            if (viewModel.GateCode == 0)
            {
                var g = db.GateCodes.Where(i => i.StartDate <= date && i.EndDate >= date).SingleOrDefault();
                if (g != null)
                    gatecode = g.GateCode1;
            }
            else
                gatecode = viewModel.GateCode;
            reservation.GateCode = gatecode;

            if (eventID != 0)
                reservation.Event_ID = eventID;
         
            //Format the start and end times
            string start_temp = viewModel.Date + " " + viewModel.Start_Time + " " + viewModel.Start_Ampm;
            string end_temp = viewModel.Date + " " + viewModel.End_Time + " " + viewModel.End_Ampm;
            start_time = DateTime.ParseExact(start_temp, "MM/dd/yyyy h:mm tt", null);
            end_time = DateTime.ParseExact(end_temp, "MM/dd/yyyy h:mm tt", null);

            reservation.Start_Time = start_time;
            reservation.End_Time = end_time;
            reservation.Dept_ID = viewModel.Dept_ID;
            reservation.Approved = approve;

            //If this is a waiting list request
            if (waiting_list != null)
                reservation.isWaitingList = true;
            else
                reservation.isWaitingList = false;

            reservation.RequesterEmail = viewModel.Requester_Email;
            if (update)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
            }
            else {
                db.Reservations.Add(reservation);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
            }
            return reservation.Reserv_ID;
        }

        public static ReservationVM ViewModelFromReservation(Reservation reservation)
        {
            var viewModel = new ReservationVM
            {
                Reserv_ID = reservation.Reserv_ID,
                RecipientName = reservation.RecipientName,
                RecipientEmail = reservation.RecipientEmail,
                Date = reservation.Start_Time.ToString("MM/dd/yyyy"),
                Start_Time = reservation.Start_Time.ToString("h:mm"),
                Start_Ampm = reservation.Start_Time.ToString("tt"),
                End_Time = reservation.End_Time.ToString("h:mm"),
                End_Ampm = reservation.End_Time.ToString("tt"),
                NumOfSlots = reservation.NumOfSlots,
                Requester_ID = reservation.Requester_ID,
                Requester_Email = reservation.Requester.Email,
                Dept_ID = reservation.Dept_ID,
                Approved = reservation.Approved
            };
            if (reservation.Event != null)
                viewModel.Event = reservation.Event.Event_Name;
            if (reservation.GateCode1 != null)
                viewModel.GateCode = reservation.GateCode1.GateCode1;

            return viewModel;
        }

        public static void UpdateReservationLots(VIPPARKINGEntities1 db, string[] selectedLots, Reservation reservation)
        {
            if (selectedLots == null)
                selectedLots = new string[0];
            var selectedLotsHS = new HashSet<string>(selectedLots);
            var reservationLots = new HashSet<int>
                (reservation.Lots.Select(l => l.Lot_ID));

            foreach (var lot in db.Lots)
            {
                if (selectedLotsHS.Contains(lot.Lot_ID.ToString()))
                {
                    if (!reservationLots.Contains(lot.Lot_ID))
                    {
                        reservation.Lots.Add(lot);
                    }
                }
                else {
                    if (!selectedLotsHS.Contains(lot.Lot_ID.ToString()))
                    {
                        reservation.Lots.Remove(lot);
                    }
                }
            }
        }
        public static void ApproveReservation(ReservationVM reservationVM, Reservation existingReservation)
        {
            var x = reservationVM.Requester_Email;
            string event_name = "";
            VIPPARKINGEntities1 db = new VIPPARKINGEntities1();

            //Create permits for each parking space
            List<string> attachments = PermitsHelper.CreatePermits(reservationVM);

            //Send email to administrator and requester to notify that reservation was approved
            //Get email recipients
            var admin_results = db.Requesters.Where(i => i.IsAdmin == true);
            List<string> recipients = new List<string>();
            foreach (var rec in admin_results)
                recipients.Add(rec.Email);
            
            recipients.Add(reservationVM.Requester_Email);
            
            if (reservationVM.Event != null)
                event_name = reservationVM.Event;

            string message = reservationVM.RecipientName + ",<br/><br/>You are receiving this email to notify you that your reservation for " + event_name + " " + reservationVM.Date + " " + reservationVM.Start_Time + reservationVM.Start_Ampm + " - " + reservationVM.End_Time + reservationVM.End_Ampm + " was approved. Attached to this email is a parking permit for each parking space that you requested. <br/><br/>The gate code for this parking lot is <b>" + reservationVM.GateCode + "</b>.<br/><br/>";

            var lots = LotsHelper.PopulateAllowedLotsData(existingReservation);
            if (lots != null)
            {
                message = message + "The following lots are available for parking: <br/>";
                foreach (var lot in lots)
                {
                    if (lot.Allowed)
                        message = message + lot.Lot_Name + "<br/>";
                }
                message = message + "<br/><br/>";
            }
            message = message + "If you have any questions, you may contact <a href = 'mailto:ruparking@regis.edu'>ruparking@regis.edu.</a><br/>< br/> Regis Parking Administration";
            EmailHelper.SendEmail("Reservation for Regis VIP Parking was Approved!", message, recipients, attachments);
        }
    }
}