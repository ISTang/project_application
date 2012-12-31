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
    public class ProjectStagesController : Controller
    {
        private ProjectsContext db = new ProjectsContext();

        //
        // GET: /ProjectStages/Manage

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            var projectStages = (from t in db.ProjectStages
                            select new ProjectStageModel
                            {
                                ID = t.ID,
                                Name = t.Name,
                                Order = t.Order
                            }).ToList();

            int count = 1;
            foreach (var type in projectStages)
            {
                type.Sequence = count++;
            }
            return View(projectStages);
        }

        //
        // GET: /ProjectStages/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ProjectStages/Create

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(ProjectStage projectStage)
        {
            if (ModelState.IsValid)
            {
                if (db.ProjectStages.Any(t => t.Name.ToLower().Equals(projectStage.Name.ToLower())))
                {
                    ModelState.AddModelError("Name", "该项目阶段已经存在");
                }
                else
                {
                    db.ProjectStages.Add(projectStage);
                    db.SaveChanges();
                    return RedirectToAction("Manage");
                }
            }

            return View(projectStage);
        }

        //
        // POST: /ProjectStages/Update

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public String Update(int id, string value, int? rowId,
               int? columnPosition, int? columnId, string columnName)
        {
            var projectStages = db.ProjectStages;

            var projectStage = projectStages.FirstOrDefault(t => t.ID == id);
            if (projectStage == null)
            {
                return "ID 为 " + id + " 的项目阶段不存在";
            }
            switch (columnPosition)
            {
                case 1:
                    if (projectStage.Name == value)
                        return value;
                    if (projectStages.Any(t => t.Name.ToLower().Equals(value.ToLower())))
                        return "名字为 \"" + value + "\" 的项目阶段已经存在";
                    projectStage.Name = value;
                    break;
                case 2:
                    if (projectStage.Order == int.Parse(value))
                        return value;
                    int intStage = int.Parse(value);
                    if (projectStages.Any(t => t.Order == intStage))
                        return "序号为 \"" + value + "\" 的项目阶段已经存在";
                    projectStage.Order = int.Parse(value);
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
        // GET: /ProjectStages/Delete/5

        [Authorize(Roles = "Admin")]
        public String Delete(int id)
        {
            try
            {
                var projectStage = db.ProjectStages.FirstOrDefault(t => t.ID == id);
                if (projectStage == null)
                    return "项目阶段ID " + id + "不存在";
                db.ProjectStages.Remove(projectStage);
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