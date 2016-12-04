
using System.Web.Mvc;
using VIP_Parking.Models.Database;
using System.Linq;
using VIP_Parking.ViewModels;
using System;
using System.Collections.Generic;

namespace VIP_Parking.Controllers
{
    public class ReportsController : Controller
    {
        private VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
        
        // GET: History
        
        public ActionResult Index()
        {
           // if ((bool)Session["isAdmin"] != true)
             //   return HttpNotFound();
           
            //Get report information
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title");
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Dept_name", Session["Dept_ID"]);
            return View();
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult Index(ReportsVM reportsVM)
        {
            //Build Select Lists
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Title", reportsVM.Category_ID);
            ViewBag.Dept_ID = new SelectList(db.Departments.OrderBy(x => x.Dept_name), "Dept_ID", "Dept_name", reportsVM.Dept_ID);

            if (ModelState.IsValid)
            {
                DateTime startdate = DateTime.ParseExact(reportsVM.StartDate, "MM/dd/yyyy", null);
                DateTime enddate = DateTime.ParseExact(reportsVM.EndDate, "MM/dd/yyyy", null);

                //Get Departments Data
                var department_results = db.Reservations.Where(r => r.CreationDate >= startdate && r.CreationDate <= enddate);
                if (reportsVM.Dept_ID != null)
                    department_results = department_results.Where(r => r.Dept_ID == reportsVM.Dept_ID);
                List<ReportsVM> departments = department_results.GroupBy(r => r.Department.Dept_name).Select(g => new ReportsVM { DeptName = g.Key, DeptCount = g.Count() }).OrderBy(r => r.DeptName).ToList();
                ViewBag.department_results = departments;

                //Get Category Data
                var category_results = db.Reservations.Where(r => r.CreationDate >= startdate && r.CreationDate <= enddate);
                if (reportsVM.Category_ID != null)
                    category_results = category_results.Where(r => r.Category_ID == reportsVM.Category_ID);
                List<ReportsVM> categories = category_results.GroupBy(r => r.Category.Title).Select(g => new ReportsVM { CategoryName = g.Key, CategoryCount = g.Count() }).OrderBy(r => r.CategoryName).ToList();

                ViewBag.category_results = categories;
            }
            return View(reportsVM);
        }
    }
}