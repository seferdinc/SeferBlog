using MyBlogSefer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace My_Blog_MVC.Controllers
{
    public class ImgController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //
        // GET: /File/
        public ActionResult Index(int id)
        {
            var fileToRetrieve = db.Imgs.Find(id);
            return File(fileToRetrieve.content, fileToRetrieve.img_contentType);
        }
    }
}