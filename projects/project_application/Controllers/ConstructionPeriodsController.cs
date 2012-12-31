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
    [Authorize]
    public class ConstructionPeriodsController : Controller
    {
        private ProjectsContext db = new ProjectsContext();

        //
        // GET: /ConstructionPeriods/Manage

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            var constructionPeriods = (from t in db.ConstructionPeriods
                            select new ConstructionPeriodModel
                            {
                                ID = t.ID,
                                Name = t.Name
                            }).ToList();

            int count = 1;
            foreach (var type in constructionPeriods)
            {
                type.Sequence = count++;
            }
            return View(constructionPeriods);
        }

        //
        // GET: /ConstructionPeriods/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ConstructionPeriods/Create

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(ConstructionPeriod constructionPeriod)
        {
            if (ModelState.IsValid)
            {
                if (db.ConstructionPeriods.Any(t => t.Name.ToLower().Equals(constructionPeriod.Name.ToLower())))
                {
                    ModelState.AddModelError("Name", "该建设期限已经存在");
                }
                else
                {
                    db.ConstructionPeriods.Add(constructionPeriod);
                    db.SaveChanges();
                    return RedirectToAction("Manage");
                }
            }

            return View(constructionPeriod);
        }

        //
        // POST: /ConstructionPeriods/Update

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public String Update(int id, string value, int? rowId,
               int? columnPosition, int? columnId, string columnName)
        {
            var constructionPeriods = db.ConstructionPeriods;

            var constructionPeriod = constructionPeriods.FirstOrDefault(t => t.ID == id);
            if (constructionPeriod == null)
            {
                return "ID 为 " + id + " 的建设期限不存在";
            }
            switch (columnPosition)
            {
                case 1:
                    if (constructionPeriod.Name == value)
                        return value;
                    if (constructionPeriods.Any(t => t.Name.ToLower().Equals(value.ToLower())))
                        return "名字为 \"" + value + "\" 的建设期限已经存在";
                    constructionPeriod.Name = value;
                    break;
                default:
                    break;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return value;
        }

        //
        // GET: /ConstructionPeriods/Delete/5

        [Authorize(Roles = "Admin")]
        public String Delete(int id)
        {
            try
            {
                var constructionPeriod = db.ConstructionPeriods.FirstOrDefault(t => t.ID == id);
                if (constructionPeriod == null)
                    return "建设期限ID " + id + "不存在";
                db.ConstructionPeriods.Remove(constructionPeriod);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}