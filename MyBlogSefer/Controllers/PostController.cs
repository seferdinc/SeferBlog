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
    [Authorize]
    public class PostController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Post/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var posts = from s in db.Posts
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(s => s.title_post.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    posts = posts.OrderByDescending(s => s.title_post);
                    break;
                case "Date":
                    posts = posts.OrderBy(s => s.title_post);
                    break;
                case "date_desc":
                    posts = posts.OrderByDescending(s => s.title_post);
                    break;
                default:  // Name ascending 
                    posts = posts.OrderBy(s => s.preview);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
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

        public ActionResult Create()
        {
            return View();
        }

        
        //Post:  more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title_post, content_post, author,date_post,preview")]Post post, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var image = new Img
                        {
                            name_Img = System.IO.Path.GetFileName(upload.FileName),
                            Img_Type = ImgType.Avatar,
                            img_contentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            image.content = reader.ReadBytes(upload.ContentLength);
                        }
                        post.Imgs = new List<Img> { image };
                    }
                    post.date_post = DateTime.Now;
                    post.author = User.Identity.Name;
                    db.Posts.Add(post);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //for error
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(post);
        }


        // GET: 
        public ActionResult Edit(int? id)
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

        // POST:
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, HttpPostedFileBase upload)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var postToUpdate = db.Posts.Find(id);
            if (TryUpdateModel(postToUpdate, "",
               new string[] { "title_post", "content_post", "preview" }))
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (postToUpdate.Imgs.Any(f => f.Img_Type == ImgType.Avatar))
                        {
                            db.Imgs.Remove(postToUpdate.Imgs.First(f => f.Img_Type == ImgType.Avatar));
                        }
                        var image = new Img
                        {
                            name_Img = System.IO.Path.GetFileName(upload.FileName),
                            Img_Type = ImgType.Avatar,
                            img_contentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            image.content = reader.ReadBytes(upload.ContentLength);
                        }
                        postToUpdate.Imgs = new List<Img> { image };
                    }
                    postToUpdate.date_post = DateTime.Now;
                    postToUpdate.author = User.Identity.Name;
                    db.Entry(postToUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(postToUpdate);
        }

        // GET: Delete
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Post post = db.Posts.Find(id);
                db.Posts.Remove(post);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error 
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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