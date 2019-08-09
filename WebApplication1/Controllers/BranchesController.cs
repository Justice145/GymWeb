using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1
{
    public class BranchesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Branches
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;

                ViewBag.displayMenu = "No";

                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }



            List<Branch> branches;
            if (TempData["search"] == null)
            {
                branches = db.Branches.Include(a => a.Classes).ToList();
            }
            else
            {
                branches = TempData["search"] as List<Branch>;
                TempData["Search"] = null;
            }

            return View(branches);
        }

        // GET: Branches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: Branches/Create
        public ActionResult Create()
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Address,PhoneNumber,WeekDayOpen,WeekDayClose,FridayOpen,FridayClose,SaturdayOpen,SaturdayClose")] Branch branch)
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR)) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                db.Branches.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(branch);
        }

        // GET: Branches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Address,PhoneNumber,WeekDayOpen,WeekDayClose,FridayOpen,FridayClose,SaturdayOpen,SaturdayClose")] Branch branch)
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(branch);
        }

        // Get:: Branches/Search
        public ActionResult Search()
        {
            var classes = db.Classes.ToList();
            List<String> classNames = new List<String>();

            foreach (var classInstance in classes)
            {
                bool toAdd = true;

                foreach (var className in classNames)
                { 

                    if (className.Equals(classInstance.Name))
                    {
                        toAdd = false;
                        break;
                    }

                }

                if (toAdd)
                {
                    classNames.Add(classInstance.Name);
                }
            }

            return View(new BranchSearchViewModel() { ClassNames = classNames });
        }

        //POST: Branches/Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(String [] classNames, String wdOpen, String wdClose, String fridayOpen,
                                    String fridayClose, String saturdayOpen, String saturdayClose)
        {
            //Wrong input
            if (wdOpen == null || wdClose == null || fridayOpen == null 
                || fridayClose == null || saturdayOpen == null || saturdayClose == null)
            {
                TempData["Search"] = new List<Branch>();
            }
            else
            {
                TimeSpan wdOpenParse = DateTime.Parse(wdOpen).TimeOfDay;
                TimeSpan wdCloseParse = DateTime.Parse(wdClose).TimeOfDay;
                TimeSpan fridayOpenParse = DateTime.Parse(fridayOpen).TimeOfDay;
                TimeSpan fridayCloseParse = DateTime.Parse(fridayClose).TimeOfDay;
                TimeSpan saturdayOpenParse = DateTime.Parse(saturdayOpen).TimeOfDay;
                TimeSpan saturdayCloseParse = DateTime.Parse(saturdayClose).TimeOfDay;

                var matchingByHours = from brch in db.Branches
                                      where (wdOpenParse >= brch.WeekDayOpen)
                                      && (wdCloseParse <= brch.WeekDayClose)
                                      && (fridayOpenParse >= brch.FridayOpen)
                                      && (fridayCloseParse <= brch.FridayClose)
                                      && (saturdayOpenParse >= brch.SaturdayOpen)
                                      && (saturdayCloseParse <= brch.SaturdayClose)
                                      select brch;

                List<Branch> all;

                //No class requirements
                if (classNames == null)
                {
                    all = matchingByHours.Include(u => u.Classes).ToList();
                }
                //Some class requirements
                else
                {
                    var matchingByClasses = from brch in matchingByHours
                                            where ((from cls in brch.Classes
                                                    join p in classNames on cls.Name equals p
                                                    select cls).Count() == classNames.Count())
                                            select brch;

                    all = matchingByClasses.Include(u => u.Classes).ToList();
                }

                TempData["search"] = all;
            }
            return Json(new { result = "Redirect", url = Url.Action("Index", "Branches") });
        }

        // GET: Branches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Branch branch = db.Branches.Find(id);
            db.Branches.Remove(branch);
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

        private Boolean isAdminUser()
        {
            return System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR);
        }
    }
}
