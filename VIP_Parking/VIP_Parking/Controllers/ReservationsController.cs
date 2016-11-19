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
                var reservations = db.Reservations.Include(r => r.Event).Where(r => r.Requester_ID == s && r.End_Time >= DateTime.Now && !r.isWaitingList).OrderBy(r => r.Start_Time);
                return View(reservations.ToList());
            }
            catch(Exception e)
            {
                return RedirectToAction("Logoff", "Login");
            }
      }
        public ActionResult Success(string waiting_list)
        {
            string message = "";
            if (waiting_list == "true")
                message = "<p>Thank you for adding your parking lot request to the waiting list</p><p>We will review your request and if spaces open up, you may be eligible for parking at your requested times. If this is the case, we will be notifying you via the email address of your account.</p>";
            else
                message = "<p>Thank you for submitting a parking reservation request.</p><p>Your request will be reviewed and you should be receiving an email within the next few days.</p><p>If your request is approved, you'll receive a parking permit as an attachment in your email.</p>";
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
            if ((int)Session["userID"] != reservation.Requester_ID)
                return HttpNotFound();

            //Set variables for template status label
            TempData["status"] = "Not Approved";
            TempData["status_class"] = "red";

            if (reservation.Approved == true)
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

            //If the reservation form validates
            if (ModelState.IsValid)
            {
                DateTime start_time, end_time;
                int event_id = 0;
                int gatecode = 0;

                //Make sure the num of slots the user is asking for does not exceed the number of slots available. If it does, display the number of slots available.
                bool isReserveable = NumSlotsHelper.isReserveable(reservation);

                //Get the event. If one doesn't exist create one.
                if (reservation.Event != null && isReserveable)
                {
                    var e = db.Events.Where(i => i.Event_Name.ToLower() == reservation.Event.ToLower());
                    if (e.Count() == 0)
                    {
                        var eventrec = new VIP_Parking.Models.Database.Event
                        {
                            Event_Name = reservation.Event,
                            Event_Start_Time = reservation.Date,
                            Event_End_Time = reservation.Date.AddHours(23),
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
                var g = db.GateCodes.Where(i => i.StartDate <= reservation.Date && i.EndDate >= reservation.Date).SingleOrDefault();
                if (g != null)
                    gatecode = g.GateCode1;

                //Format the start and end times
                string start_temp = reservation.Date.ToString("yyyy-MM-dd") + " " + reservation.Start_Time + " " + reservation.Start_Ampm;
                string end_temp = reservation.Date.ToString("yyyy-MM-dd") + " " + reservation.End_Time + " " + reservation.End_Ampm;
                start_time = DateTime.ParseExact(start_temp, "yyyy-MM-dd h:mm tt", null);
                end_time = DateTime.ParseExact(end_temp, "yyyy-MM-dd h:mm tt", null);
                if (!isReserveable && waiting_list == null)
                {
                    ModelState.AddModelError("ErrorNumSlots", "You have exceeded the number of available spaces. The number of avaiable spaces is: " + (NumSlotsHelper.getSlotsInLot() - NumSlotsHelper.getSlotsTaken(start_time, end_time)) + ". You may check back later to see if any spaces have opened up. <button id='waiting_list' name='waiting_list' value='waiting_list' type='submit'>Place me on a waiting list</button> ");
                    return View(reservation);
                }

                //Create the reservation
                Reservation r = new Reservation
                {
                    Requester_ID = (int)Session["userID"],
                    RecipientName = reservation.RecipientName,
                    RecipientEmail = reservation.RecipientEmail,
                    NumOfSlots = reservation.NumOfSlots,
                    Category_ID = reservation.Category_ID,
                    GateCode = gatecode,
                    Start_Time = start_time,
                    End_Time = end_time,
                    Dept_ID = reservation.Dept_ID,
                    Approved = false
                };
                if (event_id != 0)
                    r.Event_ID = event_id;

                //If this is a waiting list request
                if (waiting_list != null)
                    r.isWaitingList = true;
                db.Reservations.Add(r);
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
                    EmailHelper.SendEmail("Someone Has Submitted a Reservation for a Parking Spot", Session["firstname"] + " " + Session["lastname"] + " has submitted a request for " + reservation.NumOfSlots + " spaces on " + reservation.Date.ToString("MM/dd/yyyy") + ". You may log into the Regis Parking Reservation system at <a href='http://parking.regis.edu/admin'>http://parking.regis.edu/admin</a> to review this reservation.", recipients);

                    //Email requestor notifying him or her that a reservation has been made and needs reviewing
                    EmailHelper.SendEmail("Comfirmation for Regis Parking Lot Reservation", Session["firstname"] + ",<br/><br/>You are receiving this email to verify that we have received your submission for a parking request for " + reservation.NumOfSlots + " spaces on " + reservation.Date.ToString("MM/dd/yyyy") + ". Please give us 24 hours to review your reservation to determine if we will be able to accommodate your request.<br/><br/>Sincerely, <br/>Regis Parking Administration", (string)Session["email"]);
                    return RedirectToAction("Success");
                }
                else {
                    //If the user placed himself on a waiting list, email the administrator
                    EmailHelper.SendEmail("Someone Was Placed on the Regis Parking Waiting List", Session["firstname"] + " " + Session["lastname"] + " has submitted a request for " + reservation.NumOfSlots + " spaces on " + reservation.Date.ToString("MM/dd/yyyy") + "between "+reservation.Start_Time+reservation.Start_Time+" and "+reservation.End_Time+reservation.End_Ampm+". Currently, the lot is full at this time and the requester has chosen to be placed on the waiting list. You may log into the Regis Parking Reservation system at <a href='http://parking.regis.edu/admin'>http://parking.regis.edu/admin</a> to review this reservation.", recipients);
                    return RedirectToAction("Success",new { waiting_list = "true" });
                }
                
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "Administrator")]
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
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservation.Category_ID);
            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID", reservation.Event_ID);
            ViewBag.GateCode = new SelectList(db.GateCodes, "GateCode1", "GateCode1", reservation.GateCode);
            ViewBag.ParkingSpotID = new SelectList(db.ParkingSpots, "ParkingSpot_ID", "Location", reservation.ParkingSpotID);
            ViewBag.Requester_ID = new SelectList(db.Requesters, "Requester_ID", "Username", reservation.Requester_ID);
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Department_name", reservation.Dept_ID);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Reserv_ID,Requester_ID,RecipientName,NumOfSlots,RecipientEmail,Category_ID,ParkingSpotID,Event_ID,GateCode,Start_Time,End_Time,Dept_ID")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservation.Category_ID);
            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID", reservation.Event_ID);
            ViewBag.GateCode = new SelectList(db.GateCodes, "GateCode1", "GateCode1", reservation.GateCode);
            ViewBag.ParkingSpotID = new SelectList(db.ParkingSpots, "ParkingSpot_ID", "Location", reservation.ParkingSpotID);
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
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
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
