using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyBlogSefer.Models;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity.Infrastructure;

namespace My_Blog_MVC.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int page = 1)
        {
            return View(db.Posts.OrderByDescending(v => v.id_post).ToPagedList(page, 3));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Post post = db.Posts.Include(s => s.Imgs).SingleOrDefault(s => s.id_post == id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}