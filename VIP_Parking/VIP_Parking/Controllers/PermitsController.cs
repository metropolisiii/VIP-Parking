﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VIP_Parking.Helpers;
using VIP_Parking.Models.Database;

namespace VIP_Parking.Controllers
{
    public class PermitsController : Controller
    {
        private VIPPARKINGEntities1 db = new VIPPARKINGEntities1();

        // GET: Permits
        public ActionResult Index()
        {
            var permits = db.Permits.Include(p => p.Reservation);
            return View(permits.ToList());
        }

        // GET: Permits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permit permit = db.Permits.Find(id);
            if (permit == null)
            {
                return HttpNotFound();
            }
            return View(permit);
        }

        // GET: Permits/Create
        public ActionResult Create()
        {
            ViewBag.Reserv_ID = new SelectList(db.Reservations, "Reserv_ID", "RecipientName");
            return View();
        }

        // POST: Permits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PermitCode,Reserv_ID")] Permit permit)
        {
            if (ModelState.IsValid)
            {
                db.Permits.Add(permit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Reserv_ID = new SelectList(db.Reservations, "Reserv_ID", "RecipientName", permit.Reserv_ID);
            return View(permit);
        }

        // GET: Permits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permit permit = db.Permits.Find(id);
            if (permit == null)
            {
                return HttpNotFound();
            }
            ViewBag.Reserv_ID = new SelectList(db.Reservations, "Reserv_ID", "RecipientName", permit.Reserv_ID);
            return View(permit);
        }

        // POST: Permits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PermitCode,Reserv_ID")] Permit permit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Reserv_ID = new SelectList(db.Reservations, "Reserv_ID", "RecipientName", permit.Reserv_ID);
            return View(permit);
        }

        // GET: Permits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permit permit = db.Permits.Find(id);
            if (permit == null)
            {
                return HttpNotFound();
            }
            return View(permit);
        }

        // POST: Permits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Permit permit = db.Permits.Find(id);
            db.Permits.Remove(permit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Permits/Send/5
        [Authorize]
        public ActionResult Send(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permit permit = db.Permits.Find(id);
            if (permit == null)
            {
                return HttpNotFound();
            }
            //Send the permit information to the user
            string message = Session["firstname"] + ", <br/><br/> Attached to this email is the informaton for the Regis VIP Parking permit you requested for ";
            if (permit.Reservation.Event != null)
                message += permit.Reservation.Event.Event_Name + " ";
            message += permit.Reservation.Start_Time.ToString("MMMM dd") + " " + permit.Reservation.Start_Time.ToString("h:mm tt") + " - " + permit.Reservation.End_Time.ToString("h:mm tt") + ". <br/><br/>Please be sure to print this attachment and place it in your vehicle in a spot that can easily be seen and scanned.<br/><br/>The gate code for this lot will be: " + permit.Reservation.GateCode;

            //Get Permit Attachment
            var attachment = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files/");
            attachment = attachment + "qrcode_" + id + ".gif";

            EmailHelper.SendEmail("Your Regis VIP Parking Permit", message, permit.Reservation.Requester.Email, attachment);
            return View(permit);
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