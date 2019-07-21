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
            return View(new ClassSearchViewModel() { Branches = branches });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search (int? branchId)
        {

            if (branchId == null)
            {
                System.Diagnostics.Debug.WriteLine("null");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var requestedBranch = db.Branches.Find(branchId);
            var requestedBranchId = requestedBranch.Id;

            var matching = from cls in db.Classes
                                       where cls.BranchId == requestedBranchId
                                       select cls;

            var all = matching.ToList();
            System.Diagnostics.Debug.WriteLine(all.ToString());

            TempData["search"] = all;

            return RedirectToAction("Index");
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
            ViewBag.BranchId = new SelectList(db.Branches, "Id", "Address");
            ViewBag.TrainerID = new SelectList(db.Users1, "Id", "Username");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Time,TrainerID,BranchId")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Classes.Add(@class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branches, "Id", "Address", @class.BranchId);
            ViewBag.TrainerID = new SelectList(db.Users, "Id", "Username", @class.TrainerID);
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
            ViewBag.TrainerID = new SelectList(db.Users, "Id", "Username", @class.TrainerID);
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
            ViewBag.TrainerID = new SelectList(db.Users, "Id", "Username", @class.TrainerID);
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
    }
}
