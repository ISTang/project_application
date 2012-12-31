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
    public class ConstructionProgressesController : Controller
    {
        private ProjectsContext db = new ProjectsContext();

        //
        // GET: /ConstructionProgresses/Manage

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            var constructionProgresss = (from t in db.ConstructionProgresses
                            select new ConstructionProgressModel
                            {
                                ID = t.ID,
                                Name = t.Name,
                                Order = t.Order
                            }).ToList();

            int count = 1;
            foreach (var type in constructionProgresss)
            {
                type.Sequence = count++;
            }
            return View(constructionProgresss);
        }

        //
        // GET: /ConstructionProgresses/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ConstructionProgresses/Create

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(ConstructionProgress constructionProgress)
        {
            if (ModelState.IsValid)
            {
                if (db.ConstructionProgresses.Any(t => t.Name.ToLower().Equals(constructionProgress.Name.ToLower())))
                {
                    ModelState.AddModelError("Name", "该建设进度已经存在");
                }
                else
                {
                    db.ConstructionProgresses.Add(constructionProgress);
                    db.SaveChanges();
                    return RedirectToAction("Manage");
                }
            }

            return View(constructionProgress);
        }

        //
        // POST: /ConstructionProgresses/Update

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public String Update(int id, string value, int? rowId,
               int? columnPosition, int? columnId, string columnName)
        {
            var constructionProgresss = db.ConstructionProgresses;

            var constructionProgress = constructionProgresss.FirstOrDefault(t => t.ID == id);
            if (constructionProgress == null)
            {
                return "ID 为 " + id + " 的建设进度不存在";
            }
            switch (columnPosition)
            {
                case 1:
                    if (constructionProgress.Name == value)
                        return value;
                    if (constructionProgresss.Any(t => t.Name.ToLower().Equals(value.ToLower())))
                        return "名字为 \"" + value + "\" 的建设进度已经存在";
                    constructionProgress.Name = value;
                    break;
                case 2:
                    if (constructionProgress.Order == int.Parse(value))
                        return value;
                    int intProgress = int.Parse(value);
                    if (constructionProgresss.Any(t => t.Order == intProgress))
                        return "序号为 \"" + value + "\" 的建设进度已经存在";
                    constructionProgress.Order = int.Parse(value);
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
        // GET: /ConstructionProgresses/Delete/5

        [Authorize(Roles = "Admin")]
        public String Delete(int id)
        {
            try
            {
                var constructionProgress = db.ConstructionProgresses.FirstOrDefault(t => t.ID == id);
                if (constructionProgress == null)
                    return "建设进度ID " + id + "不存在";
                db.ConstructionProgresses.Remove(constructionProgress);
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