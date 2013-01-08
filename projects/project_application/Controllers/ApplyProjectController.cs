using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using project_application.Models;
using System.IO;
namespace project_application.Controllers
{
    public class ApplyProjectController : Controller
    {
        private applyContainer db = new applyContainer();

        //
        // GET: /ApplyProject/

        public ActionResult Index()
        {
            return View(db.ApplyProjectSet.ToList());
        }

        //
        // GET: /ApplyProject/Details/5

        public ActionResult Details(int id = 0)
        {
            ApplyProject applyproject = db.ApplyProjectSet.Find(id);
            if (applyproject == null)
            {
                return HttpNotFound();
            }
            return View(applyproject);
        }

        //
        // GET: /ApplyProject/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ApplyProject/Create

        [HttpPost]
        public ActionResult Create(ApplyProject applyproject, HttpPostedFileBase AttachFile = null)
        {
            String FileUrl;

            if (ModelState.IsValid)
            {
                if (AttachFile != null)
                {
                    AttachFile = Request.Files["AttachFile"];
                    if (AttachFile.ContentLength > 0)
                    {
                        FileUrl = "upload/";
                        String Path = Server.MapPath("~/") + FileUrl;
                        String FileName = DateTime.UtcNow.ToString("yyyy" + "MM" + "dd" + "HH" + "mm" + "ss" + "ffffff");
                        FileUrl = FileUrl + FileName;
                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);
                        String extstr = System.IO.Path.GetExtension(AttachFile.FileName);
                        AttachFile.SaveAs(Path + FileName + extstr);
                        applyproject.ProjectAttach = FileUrl + extstr;
                    }
                }
                else
                {
                    applyproject.ProjectAttach = "";
                }
                applyproject.ApplyPeopleDepartment = "实验中心";
                applyproject.ApplyPeopleName = "王凯斯";
                UsersContext xx = new UsersContext();
                db.ApplyProjectSet.Add(applyproject);
                db.SaveChanges();
                string phyPath = HttpContext.Request.MapPath("/");
                OfficeController oc = new OfficeController();
                oc.BuildWord(applyproject, phyPath);
                return RedirectToAction("Index");
            }

            return View(applyproject);
        }

        //
        // GET: /ApplyProject/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ApplyProject applyproject = db.ApplyProjectSet.Find(id);
            if (applyproject == null)
            {
                return HttpNotFound();
            }
            return View(applyproject);
        }

        //
        // POST: /ApplyProject/Edit/5

        [HttpPost]
        public ActionResult Edit(ApplyProject applyproject, HttpPostedFileBase AttachFile = null)
        {
            String FileUrl;
            if (ModelState.IsValid)
            {
                if (AttachFile != null)
                {
                    AttachFile = Request.Files["AttachFile"];
                    if (AttachFile.ContentLength > 0)
                    {
                        FileUrl = "upload/";
                        String Path = Server.MapPath("~/") + FileUrl;
                        String FileName = DateTime.UtcNow.ToString("yyyy" + "MM" + "dd" + "HH" + "mm" + "ss" + "ffffff");
                        FileUrl = FileUrl + FileName;
                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);
                        String extstr = System.IO.Path.GetExtension(AttachFile.FileName);
                        AttachFile.SaveAs(Path + FileName + extstr);
                        applyproject.ProjectAttach = FileUrl + extstr;

                    }
                }

                applyproject.ApplyPeopleDepartment = "实验中心";
                applyproject.ApplyPeopleName = "王凯斯";
                db.Entry(applyproject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applyproject);
        }

        //
        // GET: /ApplyProject/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ApplyProject applyproject = db.ApplyProjectSet.Find(id);
            if (applyproject == null)
            {
                return HttpNotFound();
            }
            return View(applyproject);
        }

        //
        // POST: /ApplyProject/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ApplyProject applyproject = db.ApplyProjectSet.Find(id);
            db.ApplyProjectSet.Remove(applyproject);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}