﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ClassesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Classes
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                ViewBag.UserID = user.GetUserId();

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


            List<Class> classes;
            if (TempData["search"] == null)
            {
                classes = db.Classes.Include(a => a.Branch).Include(a => a.Trainer).ToList();
            }
            else
            {
                classes = TempData["search"] as List<Class>;
                TempData["Search"] = null;
            }

            //System.Diagnostics.Debug.WriteLine("Indexing" + classes[0].Name + classes.Count());
            return View(classes);
        }

        // GET: Classes/Search
        public ActionResult Search()
        {
            var branches = db.Branches.ToList();
            var classes = db.Classes.ToList();
            var trainerRoleQuery = from role in db.Roles
                              where role.Name == RoleNames.ROLE_TRAINER
                              select role;

            var trainerRole = trainerRoleQuery.FirstOrDefault();

            var trainers = db.Users.Where(x => x.Roles.Count() > 0 && x.Roles.Any(y => y.RoleId == trainerRole.Id)).ToList();

            List < String > classNames = new List<String>();

            foreach(var gymClass in classes)
            {
                bool toAdd = true;

                foreach (var name in classNames)
                {
                    if (name.Equals(gymClass.Name))
                    {
                        toAdd = false;
                    }
                }

                if (toAdd)
                    classNames.Add(gymClass.Name);
            }
            
            return View(new ClassSearchViewModel() { Branches = branches, ClassNames = classNames, Trainers = trainers});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search (int [] branchIds, String [] classNames, String [] trainersId)
        {
            System.Diagnostics.Debug.WriteLine(trainersId[0]);
            if (branchIds == null || classNames == null || trainersId == null)
            {
                TempData["Search"] = new List<Class>();
            }
            else
            {
                var matchingByBranch = from cls in db.Classes
                                       join p in branchIds on cls.BranchId equals p
                                       select cls;

                var matchingByClassName = from cls in matchingByBranch
                                           join p in classNames on cls.Name equals p
                                            select cls;

                var matchingByTrainerId = from cls in matchingByClassName
                                          join p in trainersId on cls.TrainerID equals p
                                          select cls;

                var matching = matchingByTrainerId.Include(u => u.Trainer).Include(u => u.Branch).ToList();

                TempData["search"] = matching;
            }
            return Json(new { result = "Redirect", url = Url.Action("Index", "Classes") });
        }

        // GET: Classes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Classes/Create
        public ActionResult Create()
        {
            if (System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_TRAINER))
            {
               var specific = db.Users.Find(User.Identity.GetUserId());
               ViewBag.BranchId = specific;
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
               var role = db.Roles.Where(x => x.Name == RoleNames.ROLE_TRAINER).SingleOrDefault();
               ViewBag.TrainerID = new SelectList(db.Users.Where(x => x.Roles.Any(r => r.RoleId == role.Id)), "Id", "Name");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.BranchId = new SelectList(db.Branches, "Id", "Address");
            
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Time,TrainerID,BranchId")] Class @class)
        {
            if (System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_TRAINER))
            {
                if (!@class.TrainerID.Equals(User.Identity.GetUserId()))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                db.Classes.Add(@class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branches, "Id", "Address", @class.BranchId);
            ViewBag.TrainerID = new SelectList(db.Users, "Id", "UserName", @class.TrainerID);
            return View(@class);
        }

        // GET: Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branches, "Id", "Address", @class.BranchId);
            ViewBag.TrainerID = new SelectList(db.Users, "Id", "UserName", @class.TrainerID);
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Time,TrainerID,BranchId")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@class).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branches, "Id", "Address", @class.BranchId);
            ViewBag.TrainerID = new SelectList(db.Users, "Id", "UserName", @class.TrainerID);
            return View(@class);
        }

        // GET: Classes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class @class = db.Classes.Find(id);
            db.Classes.Remove(@class);
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

        // GET: Classes/MyClasses
        public ActionResult MyClasses()
        {
            string userName;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                userName = user.Name;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<Class> classes;

            var allClasses = db.Classes.Include(a => a.Branch).Include(a => a.Trainer);
            classes = allClasses.Where(a => a.Trainees.Any(n => n.UserName == userName)).ToList();
            return View(classes);
        }

        private Boolean isAdminUser()
        {
            return System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR);
        }
    }
}
