using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {

            string RSSURL = "https://rss.cbssports.com/rss/headlines/";
            WebClient wclient = new WebClient();
            string RSSData = wclient.DownloadString(RSSURL);

            XDocument xml = XDocument.Parse(RSSData);
            var RSSFeedData = (from x in xml.Descendants("item")
                               select new RSSFeed
                               {
                                   Title = ((string)x.Element("title")),
                                   Link = ((string)x.Element("link")),
                                   Description = ((string)x.Element("description")),
                                   PubDate = ((string)x.Element("pubDate"))
                               }).Take(5);
            ViewBag.RSSFeed = RSSFeedData;
            ViewBag.URL = RSSURL;

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
            return View(db.Branches.ToList());
        }

    }

    public class RSSFeed
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }

    }

}