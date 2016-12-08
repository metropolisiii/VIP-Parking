using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
<<<<<<< HEAD
using System.Globalization;
=======
using System.Data.SqlClient;
using System.Configuration;
>>>>>>> 95990dcf491b2bfeafdc18095611af2a4fed215c
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using VIP_Parking.Models.Database;

namespace VIP_Parking.Controllers
{
    public class GateCodesController : Controller
    {
        private VIPPARKINGEntities1 db = new VIPPARKINGEntities1();

        // GET: GateCodes
        [Authorize]
        public ActionResult Index()
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
            return View(db.GateCodes.ToList());
        }

        // GET: GateCodes/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
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
        [Authorize]
        public ActionResult Create()
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
            return View();
        }

        // POST: GateCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GateCode1,StartDate,EndDate")] GateCode gateCode)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
            if (ModelState.IsValid)
            {
                db.GateCodes.Add(gateCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gateCode);
        }

        // GET: GateCodes/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GateCode1,StartDate,EndDate")] GateCode gateCode)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
            if (ModelState.IsValid)
            {
                db.Entry(gateCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gateCode);
        }

        // GET: GateCodes/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
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
        public ActionResult Upload()
        {
            return View();
        }
        //POST: GateCodes/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    Excel.IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = Excel.ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    reader.IsFirstRowAsColumnNames = true;
                    
                    DataSet result = reader.AsDataSet();
                    string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
                   "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
                   "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt",
                   "M/d/yyyy h:mm", "M/d/yyyy h:mm",
                   "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm"};
                    //Go through each row in the dataset and check to see if the row already exists in the database. If it does, update it. If not, insert it

                    foreach (DataTable table in result.Tables)
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            DateTime dateValue;
                            //Make sure item is a date
                            if (DateTime.TryParseExact(dr.ItemArray[0].ToString(),formats, new CultureInfo("en-US"), DateTimeStyles.None, out dateValue)){
                                GateCode gc = new GateCode();
                                Console.WriteLine(dateValue);
                                //Check if row already exists in database
                                var dt = db.GateCodes.Where(d => d.StartDate >= dateValue && d.StartDate <= dateValue).SingleOrDefault();
                                if (dt == null)
                                { //Insert into database
                                    gc.StartDate = dateValue;
                                    gc.EndDate = dateValue.AddDays(7);
                                    gc.GateCode1 = Int32.Parse(dr.ItemArray[2].ToString());
                                    db.GateCodes.Add(gc);
                                    db.SaveChanges();
                                }
                                else //Update database
                                {
                                    dt.StartDate = dateValue;
                                    dt.EndDate = dateValue.AddDays(7);
                                    dt.GateCode1 = Int32.Parse(dr.ItemArray[2].ToString());
                                    db.Entry(dt).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    reader.Close();
                    return View(result.Tables[0]);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }

        // POST: GateCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if ((bool)Session["isAdmin"] != true)
                return HttpNotFound();
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
