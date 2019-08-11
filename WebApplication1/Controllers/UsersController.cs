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
using System.Web.Routing;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return RedirectToAction("Index", new RouteValueDictionary(
                                                        new { controller = "Home" }));
            }

            List<ApplicationUser> users;
            if (TempData["search"] == null)
            {
                users = db.Users.ToList();
            }
            else
            {
                users = TempData["search"] as List<ApplicationUser>;
                TempData["search"] = null;
            }

            List<UserWithRoleViewModel> usersWithRoles = new List<UserWithRoleViewModel>(); 
            foreach(var usr in users)
            {
                var userWithRole = new UserWithRoleViewModel();
                userWithRole.Id = usr.Id;
                userWithRole.Email = usr.Email;
                userWithRole.UserName = usr.UserName;
                userWithRole.Classes = usr.Classes;
                userWithRole.Name = usr.Name;
                userWithRole.Address = usr.Address;
                userWithRole.PhoneNumber = usr.PhoneNumber;
                userWithRole.RoleNames = (from userRole in usr.Roles
                                          join role in db.Roles on userRole.RoleId equals role.Id
                                          select role.Name).ToList();

                // Don't show admin user
                if (userWithRole.RoleNames[0] != RoleNames.ROLE_ADMINISTRATOR)
                {
                    usersWithRoles.Add(userWithRole);
                }
            }

            return View(usersWithRoles);
        }

        // GET: Users/UsersList
        public JsonResult UsersList()
        {
            List<String> names = db.Users.Select(x => x.Name).ToList();

            //return users;
            return Json(names, JsonRequestBehavior.AllowGet);
        }



        //POST : Users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(String userSearch)
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (userSearch == null)
            {
                TempData["search"] = new List<ApplicationUser>();
            }
            else
            {
                TempData["search"] = db.Users.Where(s => s.Name.Contains(userSearch)).Include(s => s.Classes).Include(s => s.Roles).ToList();
                System.Diagnostics.Debug.WriteLine((TempData["search"] as List<ApplicationUser>).Count());
            }

            return RedirectToAction("Index");
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                if (!(User.Identity.GetUserId().Equals(id)))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Password,Name,Address,PhoneNumber")] ApplicationUser user)
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                user.UserName = user.Email;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.IsAdmin = false;
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                if (!(User.Identity.GetUserId().Equals(id)))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                ViewBag.IsAdmin = true;
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var RoleList = new List<String>();

            RoleList.Add(RoleNames.ROLE_ADMINISTRATOR);

            RoleList.Add(RoleNames.ROLE_TRAINEE);

            RoleList.Add(RoleNames.ROLE_TRAINER);

            ViewBag.Roles = RoleList;
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Name,Address,PhoneNumber")] ApplicationUser user, String Role)
        {
            List<String> allRoles = new List<String>();
            allRoles.Add(RoleNames.ROLE_ADMINISTRATOR);
            allRoles.Add(RoleNames.ROLE_TRAINEE);
            allRoles.Add(RoleNames.ROLE_TRAINER);

            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                if (!(User.Identity.GetUserId().Equals(user.Id)))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            bool validRole = false;
            foreach (var role in allRoles)
            {
                if (Role.Equals(role))
                {
                    validRole = true;
                }
            }

            if (!validRole)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                ApplicationUser usr = db.Users.Find(user.Id);
                usr.UserName = user.Email;
                usr.Email = user.Email;
                usr.Address = user.Address;
                usr.PhoneNumber = user.PhoneNumber;
         
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                IList<String> currRoles = manager.GetRoles(usr.Id);
                
                foreach (var roleName in currRoles)
                {
                    foreach (var roleToRemove in allRoles)
                    {
                        if (roleName.Equals(roleToRemove))
                        {
                            manager.RemoveFromRole(usr.Id, roleToRemove);
                        }
                    }
                }

                manager.AddToRole(usr.Id, Role);

                db.Entry(usr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }


        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);  
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);

            // Find reqiured user role
            var usrRole = (from userRole in user.Roles
                                                     join role in db.Roles on userRole.RoleId equals role.Id
                                                     select role.Name).ToList()[0];
            if (usrRole == RoleNames.ROLE_TRAINEE)
            {
                var classes = db.Classes.ToList();
                foreach (var curClass in classes)
                {
                    curClass.Trainees.Remove(user);
                }
            }
            else
            {
                var classes = db.Classes.ToList();
                foreach (var curClass in classes)
                {
                    if (curClass.TrainerID == id)
                    {
                        db.Classes.Remove(curClass);
                    }
                }
            }
            db.Users.Remove(user);
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

        // GET: Users/RegisterToClass/5
        public ActionResult RegisterToClass(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                if (!(User.Identity.GetUserId().Equals(id)))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var classes = db.Classes.ToList();

            return View(new RegisterToClassViewModel() { User = user, Classes = classes });
        }

        // POST: Users/RegisterToClass/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterToClass(string id, string registerOrCancel, int? classId)
        {
            if (id == null || classId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!System.Web.HttpContext.Current.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR))
            {
                if (!(User.Identity.GetUserId().Equals(id)))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            var userToRegister = db.Users.Find(id);
           
            if (userToRegister == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var requestedClass = db.Classes.Find(classId);
            if (requestedClass == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool returnToDetails = false;
            if (registerOrCancel.Equals("cancel"))
            {
                userToRegister.Classes.Remove(requestedClass);
                requestedClass.Trainees.Remove(userToRegister);
                returnToDetails = true;
            }
            else if (registerOrCancel.Equals("register"))
            {
                userToRegister.Classes.Add(requestedClass);
                requestedClass.Trainees.Add(userToRegister);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Entry(userToRegister).State = EntityState.Modified;
            db.Entry(requestedClass).State = EntityState.Modified;
            db.SaveChanges();

            if (returnToDetails)
            {
                return RedirectToAction("Details", new RouteValueDictionary(
                                                        new { controller = "Users", action = "Main", id = id }));
            }

            return RedirectToAction("Details", new RouteValueDictionary(
                                                                    new { controller = "Classes", action = "Main", id = classId }));
        }
    }
}
