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
    public class RequestersController : Controller
    {
        private VIPPARKINGEntities1 db = new VIPPARKINGEntities1();

        // GET: Requesters
        public ActionResult Index()
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
            var requesters = db.Requesters.Include(r => r.Department).OrderBy(r => r.Firstname);
            return View(requesters.ToList());
        }

        // GET: Requesters/Details/5
        public ActionResult Details(int? id)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requester requester = db.Requesters.Find(id);
            if (requester == null)
            {
                return HttpNotFound();
            }
            return View(requester);
        }

       
        // GET: Requesters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requester requester = db.Requesters.Find(id);
            if (requester == null)
            {
                return HttpNotFound();
            }
            //Get reservations
            var reservations = db.Reservations.Where(r => r.Requester_ID == id);
            ViewBag.reservations = reservations;
            return View(requester);
        }

        // POST: Requesters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Requester_ID,Username,Password,Firstname,Lastname,Dept_ID,Email,IsAdmin,Fullname")] Requester requester)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requester).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Dept_ID = new SelectList(db.Departments, "Dept_ID", "Dept_name", requester.Dept_ID);
            return View(requester);
        }

        // GET: Requesters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requester requester = db.Requesters.Find(id);
            if (requester == null)
            {
                return HttpNotFound();
            }
            return View(requester);
        }

        // POST: Requesters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Requester requester = db.Requesters.Find(id);
            db.Requesters.Remove(requester);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // POST: Get/Lock/5
        [Authorize]
        public ActionResult Lock(int? id)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requester requester = db.Requesters.Find(id);
            requester.IsLocked = true;

            db.SaveChanges();

            return RedirectToAction("Edit", new { id = id });
        }

        // GET: Requesters/Unlock/5
        
        public ActionResult Unlock(int? id)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requester requester = db.Requesters.Find(id);
            requester.IsLocked = false;

            db.SaveChanges();

            return RedirectToAction("Edit", new { id = id });
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
