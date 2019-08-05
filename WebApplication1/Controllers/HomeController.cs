using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            List<NameAndCount> ClassesPerBranch = db.Classes.GroupBy(s => s.Branch).Select(s => new NameAndCount { Name = s.Select(x => x.Branch.Address).FirstOrDefault(), Count = s.Count() }).ToList();
            HomeGraphs graphs = new HomeGraphs();
            graphs.ClassesPerBranch = ClassesPerBranch;
            return View(graphs);
        }

        public ActionResult About()
        {
            ViewBag.Message = "About Us";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }
    }
}