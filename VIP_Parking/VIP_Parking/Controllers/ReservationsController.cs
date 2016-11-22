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
            try {
                var s = (int)Session["userID"];
                var reservations = db.Reservations.Include(r => r.Event).Where(r => r.Start_Time >= DateTime.Now && !r.isWaitingList).OrderBy(r => r.Approved).ThenBy(r => r.Start_Time);

                if (!(bool)Session["isAdmin"]) //Admins get listing of all reservations. Requesters only see their requests.
                {
                    reservations = (IOrderedQueryable<Reservation>)reservations.Where(r => r.Requester_ID == s);
                }
                return View(reservations.ToList());
            }
            catch(Exception e)
            {
                return RedirectToAction("Logoff", "Login");
            }
      }
        public ActionResult Success(string status)
        {
            string message = "";
            switch (status)
            {
                case "waiting list":
                    message = "<p>Thank you for adding your parking lot request to the waiting list</p><p>We will review your request and if spaces open up, you may be eligible for parking at your requested times. If this is the case, we will be notifying you via the email address of your account.</p>";
                    break;
                case "request":
                    message = "<p>Thank you for submitting a parking reservation request.</p><p>Your request will be reviewed and you should be receiving an email within the next few days.</p><p>If your request is approved, you'll receive a parking permit as an attachment in your email.</p>";
                    break;
                case "decline":
                    message = "<p>This reservation has been declined.</p>";
                    break;
                default:
                    message = "";
                    break;
            }
            ViewData["message"] = message;
            return View();
        }
        // GET: Reservations/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);

            //Make sure the reservation belongs to this user
            if ((int)Session["userID"] != reservation.Requester_ID && (bool)Session["isAdmin"] != true)
                return HttpNotFound();

            //Set variables for template status label
            TempData["status"] = "Not Approved";
            TempData["status_class"] = "red";

            if (reservation.Approved == 1)
            {
                TempData["status"] = "Approved!";
                TempData["status_class"] = "green";

                //Get permit id
                var permit = db.Permits.Where(i => i.Reserv_ID == (int)id).SingleOrDefault();
                if (permit != null)
                    TempData["permitID"] = permit.PermitCode;
            }

            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        private void UpdateReservation(Reservation reservation, ReservationVM viewModel, string waiting_list, byte Approve=0)
        {
            bool update = false;
            if (reservation.Reserv_ID != 0)
                update = true;
            //Make sure the num of slots the user is asking for does not exceed the number of slots available. If it does, display the number of slots available.
            bool isReserveable = NumSlotsHelper.isReserveable(viewModel);
            int event_id = 0;
            int gatecode = 0;
            DateTime start_time, end_time;

            reservation.Requester_ID = viewModel.Requester_ID;
            reservation.RecipientName = viewModel.RecipientName;
            reservation.NumOfSlots = viewModel.NumOfSlots;
            reservation.RecipientEmail = viewModel.RecipientEmail;
            reservation.Category_ID = viewModel.Category_ID;
            var date = DateTime.ParseExact(viewModel.Date, "MM/dd/yyyy", null);
            //Get the event. If one doesn't exist create one.
            if (viewModel.Event != null && isReserveable)
            {
                var e = db.Events.Where(i => i.Event_Name.ToLower() == viewModel.Event.ToLower());
                if (e.Count() == 0)
                {
                    var eventrec = new VIP_Parking.Models.Database.Event
                    {
                        Event_Name = viewModel.Event,
                        Event_Start_Time = date,
                        Event_End_Time = date.AddHours(23),
                        Event_Spots_Needed = 0
                    };
                    db.Events.Add(eventrec);
                    db.SaveChanges();
                    event_id = eventrec.Event_ID;
                }
                else
                    event_id = e.FirstOrDefault().Event_ID;
            }
            //Get the current gate code
            if (viewModel.GateCode == 0) {
                var g = db.GateCodes.Where(i => i.StartDate <= date && i.EndDate >= date).SingleOrDefault();
                if (g != null)
                    gatecode = g.GateCode1;
            }
            else
                gatecode = viewModel.GateCode;

            if (event_id != 0)
                reservation.Event_ID = event_id;
            reservation.GateCode = gatecode;
            
            //Format the start and end times
            string start_temp = viewModel.Date + " " + viewModel.Start_Time + " " + viewModel.Start_Ampm;
            string end_temp = viewModel.Date + " " + viewModel.End_Time + " " + viewModel.End_Ampm;
            start_time = DateTime.ParseExact(start_temp, "MM/dd/yyyy h:mm tt", null);
            end_time = DateTime.ParseExact(end_temp, "MM/dd/yyyy h:mm tt", null);

            reservation.Start_Time = start_time;
            reservation.End_Time = end_time;
            reservation.Dept_ID = viewModel.Dept_ID;
            reservation.Approved = Approve;
            //If this is a waiting list request
            if (waiting_list != null)
                reservation.isWaitingList = true;
            reservation.RequesterEmail = viewModel.Requester_Email;
            if (update) {
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
        }

        // GET: Reservations/Create
        [Authorize]
        public ActionResult Create()
        {
#if DEBUG
            Session["firstname"] = "Jason";
            Session["lastname"] = "Kirby";
            Session["email"] = "jason.kirby@cablelabs.com";
            Session["userID"] = 1;
#endif
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title");
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Dept_name", Session["Dept_ID"]);
            ViewBag.Requester_ID = new SelectList(db.Requesters.OrderBy(x => x.Firstname), "Requester_ID", "Fullname", Session["userID"]);
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReservationVM reservation, string waiting_list)
        {
            //Build Select Lists
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservation.Category_ID);
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Dept_name", reservation.Dept_ID);
            ViewBag.Requester_ID = new SelectList(db.Requesters.OrderBy(x => x.Firstname), "Requester_ID", "Fullname", reservation.Requester_ID);

            //If the reservation form validates
            if (ModelState.IsValid)
            {
                var r = new Reservation();
                UpdateReservation(r, reservation, waiting_list);

                //Email administrators notification to administrator notifying him or her that someone has placed a reservation request
                //Get administrators
                var admin_results = db.Requesters.Where(i => i.IsAdmin == true);
                List<string> recipients = new List<string>();
                foreach (var rec in admin_results)
                {
                    recipients.Add(rec.Email);
                }
                if (waiting_list == null)
                {
                    EmailHelper.SendEmail("Someone Has Submitted a Reservation for a Parking Spot", Session["firstname"] + " " + Session["lastname"] + " has submitted a request for " + reservation.NumOfSlots + " spaces on " + reservation.Date + ". You may log into the Regis Parking Reservation system at <a href='http://parking.regis.edu/admin'>http://parking.regis.edu/admin</a> to review this reservation.", recipients);

                    //Email requestor notifying him or her that a reservation has been made and needs reviewing
                    EmailHelper.SendEmail("Comfirmation for Regis Parking Lot Reservation", Session["firstname"] + ",<br/><br/>You are receiving this email to verify that we have received your submission for a parking request for " + reservation.NumOfSlots + " spaces on " + reservation.Date + ". Please give us 24 hours to review your reservation to determine if we will be able to accommodate your request.<br/><br/>Sincerely, <br/>Regis Parking Administration", (string)Session["email"]);
                    return RedirectToAction("Success", new { status = "request" });
                }
                else {
                    //If the user placed himself on a waiting list, email the administrator
                    EmailHelper.SendEmail("Someone Was Placed on the Regis Parking Waiting List", Session["firstname"] + " " + Session["lastname"] + " has submitted a request for " + reservation.NumOfSlots + " spaces on " + reservation.Date + "between "+reservation.Start_Time+reservation.Start_Time+" and "+reservation.End_Time+reservation.End_Ampm+". Currently, the lot is full at this time and the requester has chosen to be placed on the waiting list. You may log into the Regis Parking Reservation system at <a href='http://parking.regis.edu/admin'>http://parking.regis.edu/admin</a> to review this reservation.", recipients);
                    return RedirectToAction("Success",new {status = "waiting list" });
                }
                
            }
            return View(reservation);
        }

        private ReservationVM ViewModelFromReservation(Reservation reservation)
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

        // GET: Reservations/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
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
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservation.Category_ID);
            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID", reservation.Event_ID);
            ViewBag.Requester_ID = new SelectList(db.Requesters, "Requester_ID", "Username", reservation.Requester_ID);
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Dept_name", reservation.Dept_ID);
            return View(ViewModelFromReservation(reservation));
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Reserv_ID,RecipientName,RecipientEmail,Category_ID,Event,Date,Start_Time,Start_Ampm,End_Time,End_Ampm,NumOfSlots,Requester_ID,Requester_Email,Dept_ID,GateCode,Approved")] ReservationVM reservation, string waiting_list, string Approve)
        {
            //Make sure the reservation belongs to this user
            if ((int)Session["userID"] != reservation.Requester_ID && (bool)Session["isAdmin"] != true)
                return HttpNotFound();
            if (ModelState.IsValid)
            {
                byte a = 0;
                if (Approve == "1")
                    a = 1;
                else if (Approve == "2")
                    a = 2;
                var existingReservation = db.Reservations.Find(reservation.Reserv_ID);
                UpdateReservation(existingReservation, reservation, waiting_list, a);
                db.SaveChanges();
                if (a == 2) //Reservation was declined
                {
                    //Send email to requester to notify that reservation was declined
                    string event_name = "";
                    if (reservation.Event != null)
                        event_name = reservation.Event;
                    EmailHelper.SendEmail("Reservation for Regis VIP Parking was Declined", reservation.RecipientName+",<br/><br/>You are receiving this email to notify you that your reservation for "+event_name+" "+reservation.Date+" "+reservation.Start_Time+reservation.Start_Ampm+" - "+reservation.End_Time+reservation.End_Ampm+" was declined. If you have any questions, you may contact <a href='mailto:ruparking@regis.edu'>ruparking@regis.edu.</a><br/><br/>Regis Parking Administration" , reservation.Requester_Email);
                    return RedirectToAction("Success", new { status = "decline" });
                }
                return RedirectToAction("Index");
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservation.Category_ID);
            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID", reservation.Event_ID);
            ViewBag.GateCode = new SelectList(db.GateCodes, "GateCode1", "GateCode1", reservation.GateCode);
            ViewBag.Requester_ID = new SelectList(db.Requesters, "Requester_ID", "Username", reservation.Requester_ID);
            ViewBag.Dept_ID = new SelectList(db.Departments, "Dept_ID", "Dept_name", reservation.Dept_ID);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
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
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            //Make sure the reservation belongs to this user
            if ((int)Session["userID"] != reservation.Requester_ID && (bool)Session["isAdmin"] != true)
                return HttpNotFound();
            db.Reservations.Remove(reservation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
