using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using project_application.Models;

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
        public ActionResult Create(ApplyProject applyproject)
        {
            if (ModelState.IsValid)
            {
                db.ApplyProjectSet.Add(applyproject);
                db.SaveChanges();
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
        public ActionResult Edit(ApplyProject applyproject)
        {
            if (ModelState.IsValid)
            {
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