using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VIP_Parking.Models.Database;

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
                var reservations = db.Reservations.Include(r => r.Event).Where(r => r.Requester_ID == s && r.End_Time >= DateTime.Now).OrderBy(r => r.Start_Time);
                return View(reservations.ToList());
            }
            catch(Exception e)
            {
                return RedirectToAction("Logoff", "Login");
            }
      }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
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

        // GET: Reservations/Create
        public ActionResult Create()
        {
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title");
            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID");
            ViewBag.GateCode = new SelectList(db.GateCodes, "GateCode1", "GateCode1");
            ViewBag.ParkingSpotID = new SelectList(db.ParkingSpots, "ParkingSpot_ID", "Location");
            ViewBag.Requester_ID = new SelectList(db.Requesters, "Requester_ID", "Username");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Reserv_ID,Requester_ID,RecipientName,NumOfSlots,RecipientEmail,Category_ID,ParkingSpotID,Event_ID,GateCode,Start_Time,End_Time")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reservation.Category_ID);
            ViewBag.Event_ID = new SelectList(db.Events, "Event_ID", "Event_ID", reservation.Event_ID);
            ViewBag.GateCode = new SelectList(db.GateCodes, "GateCode1", "GateCode1", reservation.GateCode);
            ViewBag.ParkingSpotID = new SelectList(db.ParkingSpots, "ParkingSpot_ID", "Location", reservation.ParkingSpotID);
            ViewBag.Requester_ID = new SelectList(db.Requesters, "Requester_ID", "Username", reservation.Requester_ID);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
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
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Reserv_ID,Requester_ID,RecipientName,NumOfSlots,RecipientEmail,Category_ID,ParkingSpotID,Event_ID,GateCode,Start_Time,End_Time")] Reservation reservation)
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
