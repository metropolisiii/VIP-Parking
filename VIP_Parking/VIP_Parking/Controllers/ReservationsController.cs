using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VIP_Parking.Models.Database;
using VIP_Parking.ViewModels;
using VIP_Parking.Helpers;
using System.IO;

namespace VIP_Parking.Controllers
{
    public class ReservationsController : Controller
    {
        private VIPPARKINGEntities1 db = new VIPPARKINGEntities1();

        // GET: Reservations
        [Authorize]
        public ActionResult Index()
        {
            //Get this user's current reservations. Any reservations made in the past will not be displayed 
            //Only the logged in user or an admin can get a list of reservations. If the logged in user is not an admin, he only sees his reservations. Admins see all reservations.
            try {
                var reservations = db.Reservations.Include(r => r.Event).Where(r => r.Start_Time >= DateTime.Now && !r.isWaitingList).OrderBy(r => r.Approved).ThenBy(r => r.Start_Time);

                if (!(bool)Session["isAdmin"]) //Admins get listing of all reservations. Requesters only see their requests.
                {
                    int userID = (int)Session["userID"];
                    reservations = (IOrderedQueryable<Reservation>)reservations.Where(r => r.Requester_ID == userID);
                }
                return View(reservations.ToList());
            }
            catch(Exception e)
            {
                return RedirectToAction("Logoff", "Login");
            }
        }
        // GET: Reservations/WaitingList
        [Authorize]
        public ActionResult WaitingList()
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();

            //Get all reservations that are on the waiting list
            var reservations = db.Reservations.Include(r => r.Event).Where(r => r.Start_Time >= DateTime.Now && r.isWaitingList).OrderBy(r => r.Start_Time);
            TempData["waiting_list"] = true;
            return View("Index",reservations);
        }
        // GET: Reservations/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
               return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
           
            Reservation reservation = db.Reservations.Find(id);

            if (reservation == null)
                return HttpNotFound();
            
            //Make sure the reservation belongs to this user
            if ((int)Session["userID"] != reservation.Requester_ID && !(bool)Session["isAdmin"])
                return HttpNotFound();

            //Set variables for template status label
            TempData["status"] = "Not Approved";
            TempData["status_class"] = "red";

            //Set view data for when a reservation has been approved
            if (reservation.Approved == 1)
            {
                TempData["status"] = "Approved!";
                TempData["status_class"] = "green";
            }            
            return View(reservation);
        }        

        // GET: Reservations/Create
        [Authorize]
        public ActionResult Create()
        {
            TempData["requester_email"] = Session["email"];
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title");
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Dept_name", Session["Dept_ID"]);
            ViewBag.Requester_ID = new SelectList(db.Requesters.OrderBy(x => x.Firstname), "Requester_ID", "Fullname", Session["userID"]);
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReservationVM reservationVM, string waiting_list)
        {
            //Build Select Lists
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservationVM.Category_ID);
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Dept_name", reservationVM.Dept_ID);
            ViewBag.Requester_ID = new SelectList(db.Requesters.OrderBy(x => x.Firstname), "Requester_ID", "Fullname", reservationVM.Requester_ID);

            //If the reservation form validates
            if (ModelState.IsValid)
            {
                var r = new Reservation();
                int reserv_ID = ReservationsHelper.UpdateReservation(db, r, reservationVM, (int)Session["userID"], waiting_list);
                bool isReserveable = NumSlotsHelper.isReserveable(reservationVM);
                if (!isReserveable && waiting_list == null)
                {
                    //Format the start and end times
                    string start_temp = reservationVM.Date + " " + reservationVM.Start_Time + " " + reservationVM.Start_Ampm;
                    string end_temp = reservationVM.Date + " " + reservationVM.End_Time + " " + reservationVM.End_Ampm;
                    DateTime start_time = DateTime.ParseExact(start_temp, "MM/dd/yyyy h:mm tt", null);
                    DateTime end_time = DateTime.ParseExact(end_temp, "MM/dd/yyyy h:mm tt", null);
                    ModelState.AddModelError("ErrorNumSlots", "You have exceeded the number of available spaces. The number of avaiable spaces is: " + (NumSlotsHelper.getSlotsInLot() - NumSlotsHelper.getSlotsTaken(start_time, end_time)) + ". You may check back later to see if any spaces have opened up. <button id='waiting_list' name='waiting_list' value='waiting_list' type='submit'>Place me on a waiting list</button> ");
                    return View(reservationVM);
                }
                //Email administrators notification to administrator notifying him or her that someone has placed a reservation request
                //Get administrators
                var admin_results = db.Requesters.Where(i => i.IsAdmin == true);
                List<string> recipients = new List<string>();
                foreach (var rec in admin_results)
                    recipients.Add(rec.Email);
                if (waiting_list == null)
                {
                    EmailHelper.SendEmail("Someone Has Submitted a Reservation for a Parking Spot", Session["firstname"] + " " + Session["lastname"] + " has submitted a request for " + reservationVM.NumOfSlots + " spaces on " + reservationVM.Date + ". You may log into the Regis Parking Reservation system at <a href='http://parking.regis.edu/admin'>http://parking.regis.edu/admin</a> to review this reservation.", recipients);

                    //Email requestor notifying him or her that a reservation has been made and needs reviewing
                    EmailHelper.SendEmail("Comfirmation for Regis Parking Lot Reservation", Session["firstname"] + ",<br/><br/>You are receiving this email to verify that we have received your submission for a parking request for " + reservationVM.NumOfSlots + " spaces on " + reservationVM.Date + ". Please give us 24 hours to review your reservation to determine if we will be able to accommodate your request.<br/><br/>Sincerely, <br/>Regis Parking Administration", (string)Session["email"]);
                    return RedirectToAction("Success", new { status = "request", reserv_id = reserv_ID });
                }
                else {
                    //If the user placed himself on a waiting list, email the administrator
                    EmailHelper.SendEmail("Someone Was Placed on the Regis Parking Waiting List", Session["firstname"] + " " + Session["lastname"] + " has submitted a request for " + reservationVM.NumOfSlots + " spaces on " + reservationVM.Date + "between "+reservationVM.Start_Time+reservationVM.Start_Time+" and "+reservationVM.End_Time+reservationVM.End_Ampm+". Currently, the lot is full at this time and the requester has chosen to be placed on the waiting list. You may log into the Regis Parking Reservation system at <a href='http://parking.regis.edu/admin'>http://parking.regis.edu/admin</a> to review this reservation.", recipients);
                    return RedirectToAction("Success",new {status = "waiting list", reserv_id = reserv_ID });
                }                
            }
           
            return View(reservationVM);
        }

        
        // GET: Reservations/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Reservation reservation = db.Reservations.Find(id);
            
            //Make sure the admin is editing
            if (reservation == null || (bool)Session["isAdmin"] != true)
                return HttpNotFound();
            
            //Get information to populate select lists
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservation.Category_ID);
            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID", reservation.Event_ID);
            ViewBag.Requester_ID = new SelectList(db.Requesters, "Requester_ID", "Username", reservation.Requester_ID);
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Dept_name", reservation.Dept_ID);
            ViewBag.Lots = LotsHelper.PopulateAllowedLotsData(reservation);
            TempData["requester_email"] = reservation.RequesterEmail;
            return View(ReservationsHelper.ViewModelFromReservation(reservation));
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Reserv_ID,RecipientName,RecipientEmail,Category_ID,Event,Date,Start_Time,Start_Ampm,End_Time,End_Ampm,NumOfSlots,Requester_ID,Requester_Email,Dept_ID,GateCode,Approved")] ReservationVM reservationVM, string waiting_list, string approve, string[] selectedLots)
        {
            //Make sure the the admin is editing
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();

            var existingReservation = db.Reservations.Find(reservationVM.Reserv_ID);
            if (ModelState.IsValid)
            {

                int a = Convert.ToInt32(approve);

                if (a != 0 || waiting_list != null)
                {
                    //Makes sure there is still enough spaces
                    if (a == 1)
                    {
                        bool isReserveable = NumSlotsHelper.isReserveable(reservationVM);
                        if (!isReserveable && waiting_list == null)
                        {
                            //Format the start and end times
                            string start_temp = reservationVM.Date + " " + reservationVM.Start_Time + " " + reservationVM.Start_Ampm;
                            string end_temp = reservationVM.Date + " " + reservationVM.End_Time + " " + reservationVM.End_Ampm;
                            DateTime start_time = DateTime.ParseExact(start_temp, "MM/dd/yyyy h:mm tt", null);
                            DateTime end_time = DateTime.ParseExact(end_temp, "MM/dd/yyyy h:mm tt", null);
                            ModelState.AddModelError("ErrorNumSlots", "You have exceeded the number of available spaces. The number of avaiable spaces is: " + (NumSlotsHelper.getSlotsInLot() - NumSlotsHelper.getSlotsTaken(start_time, end_time)) + ". You may check back later to see if any spaces have opened up. <button id='waiting_list' name='waiting_list' value='waiting_list' type='submit'>Place me on a waiting list</button> ");
                            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservationVM.Category_ID);
                            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID", reservationVM.Event_ID);
                            ViewBag.GateCode = new SelectList(db.GateCodes, "GateCode1", "GateCode1", reservationVM.GateCode);
                            ViewBag.Requester_ID = new SelectList(db.Requesters, "Requester_ID", "Username", reservationVM.Requester_ID);
                            ViewBag.Dept_ID = new SelectList(db.Departments, "Dept_ID", "Dept_name", reservationVM.Dept_ID);
                            ViewBag.Lots = LotsHelper.PopulateAllowedLotsData(existingReservation);
                            return View(reservationVM);
                        }
                    }
                    ReservationsHelper.UpdateReservation(db, existingReservation, reservationVM, reservationVM.Requester_ID, waiting_list, (byte)a);
                    ReservationsHelper.UpdateReservationLots(db, selectedLots, existingReservation);
                    db.SaveChanges();
                }

                //Handle admin's response to reservation request
                if (waiting_list == null)
                {
                    switch (a)
                    {
                        case 0: //Reservation was cancelled
                            return RedirectToAction("Delete", new { id = reservationVM.Reserv_ID });
                        case 1: //Reservation was approved
                            ReservationsHelper.ApproveReservation(reservationVM, existingReservation);
                            return RedirectToAction("Success", new { status = "approved", reserv_id = reservationVM.Reserv_ID });
                        case 2: //Reservation was declined
                            //Remove any permits from the database
                            db.Permits.RemoveRange(db.Permits.Where(p => p.Reserv_ID == reservationVM.Reserv_ID));
                            db.SaveChanges();
                            //Send email to requester to notify that reservation was declined
                            string event_name = "";
                            if (reservationVM.Event != null)
                                event_name = reservationVM.Event;
                            EmailHelper.SendEmail("Reservation for Regis VIP Parking was Declined", reservationVM.RecipientName + ",<br/><br/>You are receiving this email to notify you that your reservation for " + event_name + " " + reservationVM.Date + " " + reservationVM.Start_Time + reservationVM.Start_Ampm + " - " + reservationVM.End_Time + reservationVM.End_Ampm + " was declined. If you have any questions, you may contact <a href='mailto:ruparking@regis.edu'>ruparking@regis.edu.</a><br/><br/>Regis Parking Administration", reservationVM.Requester_Email);
                            return RedirectToAction("Success", new { status = "decline", reserv_id = reservationVM.Reserv_ID });
                    }
                }
               else {
                    //If the user placed himself on a waiting list, email the administrator
                    //Get administrators
                    var admin_results = db.Requesters.Where(i => i.IsAdmin == true);
                    List<string> recipients = new List<string>();
                    foreach (var rec in admin_results)
                        recipients.Add(rec.Email);
                    EmailHelper.SendEmail("Someone Was Placed on the Regis Parking Waiting List", Session["firstname"] + " " + Session["lastname"] + " has submitted a request for " + reservationVM.NumOfSlots + " spaces on " + reservationVM.Date + "between "+reservationVM.Start_Time+reservationVM.Start_Time+" and "+reservationVM.End_Time+reservationVM.End_Ampm+". Currently, the lot is full at this time and the requester has chosen to be placed on the waiting list. You may log into the Regis Parking Reservation system at <a href='http://parking.regis.edu/admin'>http://parking.regis.edu/admin</a> to review this reservation.", recipients);
                    return RedirectToAction("Success",new {status = "waiting list", reserv_id = reservationVM.Reserv_ID });
                }      

                return RedirectToAction("Index");
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservationVM.Category_ID);
            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID", reservationVM.Event_ID);
            ViewBag.GateCode = new SelectList(db.GateCodes, "GateCode1", "GateCode1", reservationVM.GateCode);
            ViewBag.Requester_ID = new SelectList(db.Requesters, "Requester_ID", "Username", reservationVM.Requester_ID);
            ViewBag.Dept_ID = new SelectList(db.Departments, "Dept_ID", "Dept_name", reservationVM.Dept_ID);
            ViewBag.Lots = LotsHelper.PopulateAllowedLotsData(existingReservation);
            return View(reservationVM);
        }

        // GET: Reservations/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            //Make sure the reservation belongs to this user
            if ((int)Session["userID"] != reservation.Requester_ID && (bool)Session["isAdmin"] != true)
                return HttpNotFound();
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            //Make sure the reservation belongs to this user
            if ((int)Session["userID"] != reservation.Requester_ID && (bool)Session["isAdmin"] != true)
                return HttpNotFound();
            //Get administrators
            var admin_results = db.Requesters.Where(i => i.IsAdmin == true);
            List<string> recipients = new List<string>();
            foreach (var rec in admin_results)
                recipients.Add(rec.Email);
            string message = reservation.Requester.Fullname + " has cancelled a reservation for " + reservation.NumOfSlots + " spaces on " + reservation.Start_Time + " - " + reservation.End_Time + ".";
            //Check if there are slots on the waiting list
            var waitinglist_items = db.Reservations.Where(s => s.isWaitingList && ((s.Start_Time <= reservation.Start_Time && s.End_Time > reservation.Start_Time) || (s.Start_Time < reservation.End_Time && s.End_Time >= reservation.End_Time) || (s.Start_Time >= reservation.Start_Time && s.End_Time < reservation.End_Time)));
            foreach (Reservation item in waitinglist_items)
            {
                if (NumSlotsHelper.isReserveable(ReservationsHelper.ViewModelFromReservation(item))) ;
                {
                    message = message + ". There is a possibility that a reservation on the waiting list may fill this spot";
                }
            }
            EmailHelper.SendEmail("Someone Has Cancelled a Reservation for a Parking Spot", message, recipients);
            db.Reservations.Remove(reservation);
            db.SaveChanges();
                     
            
            return RedirectToAction("Index");
        }

        
        public ActionResult Success(string status, string reserv_id)
        {
            //Get the reservation
            var res = db.Reservations.Find(Convert.ToInt32(reserv_id));
            ReservationVM reservation = ReservationsHelper.ViewModelFromReservation(res);
            string message = "";
            string event_info = "";
            if (reservation.Event != null)
                event_info = event_info + reservation.Event + " ";
            event_info = event_info + reservation.Date + " " + reservation.Start_Time + reservation.Start_Ampm + " - " + reservation.End_Time + reservation.End_Ampm;
            switch (status)
            {
                case "waiting list":
                    message = "<p>Thank you for adding your parking lot request for " + event_info + " to the waiting list</p><p>We will review your request and if spaces open up, you may be eligible for parking at your requested times. If this is the case, we will be notifying you via the email address of your account.</p>";
                    break;
                case "request":
                    message = "<p>Thank you for submitting a parking reservation request.</p><p>Your request for " + event_info + " will be reviewed and you should be receiving an email within the next few days.</p><p>If your request is approved, you'll receive a parking permit as an attachment in your email.</p>";
                    break;
                case "decline":
                    message = "<p>This reservation for " + event_info + " has been declined. The requester will be notified of this status.</p>";
                    break;
                case "approved":
                    message = "<p>This reservation for " + event_info + " has been approved! The requester will be notified of this status and will be sent " + reservation.NumOfSlots + " permit(s) and a gate code of " + reservation.GateCode + ".";
                    break;
                default:
                    break;
            }
            ViewData["message"] = message;
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
