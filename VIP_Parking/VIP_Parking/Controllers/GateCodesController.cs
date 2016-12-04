﻿using System;
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
    public class GateCodesController : Controller
    {
        private VIPPARKINGEntities1 db = new VIPPARKINGEntities1();

        // GET: GateCodes
        public ActionResult Index()
        {
            return View(db.GateCodes.ToList());
        }

        // GET: GateCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GateCode gateCode = db.GateCodes.Find(id);
            if (gateCode == null)
            {
                return HttpNotFound();
            }
            return View(gateCode);
        }

        // GET: GateCodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GateCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GateCode1,StartDate,EndDate")] GateCode gateCode)
        {
            if (ModelState.IsValid)
            {
                db.GateCodes.Add(gateCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gateCode);
        }

        // GET: GateCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GateCode gateCode = db.GateCodes.Find(id);
            if (gateCode == null)
            {
                return HttpNotFound();
            }
            return View(gateCode);
        }

        // POST: GateCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GateCode1,StartDate,EndDate")] GateCode gateCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gateCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gateCode);
        }

        // GET: GateCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GateCode gateCode = db.GateCodes.Find(id);
            if (gateCode == null)
            {
                return HttpNotFound();
            }
            return View(gateCode);
        }

        // POST: GateCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GateCode gateCode = db.GateCodes.Find(id);
            db.GateCodes.Remove(gateCode);
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