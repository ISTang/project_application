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
    public class ProjectTypesController : Controller
    {
        private ProjectsContext db = new ProjectsContext();

        //
        // GET: /ProjectTypes/Manage

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            var projectTypes = (from t in db.ProjectTypes
                            select new ProjectTypeModel
                            {
                                ID = t.ID,
                                Name = t.Name
                            }).ToList();

            int count = 1;
            foreach (var type in projectTypes)
            {
                type.Sequence = count++;
            }
            return View(projectTypes);
        }

        //
        // GET: /ProjectTypes/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ProjectTypes/Create

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(ProjectType projectType)
        {
            if (ModelState.IsValid)
            {
                if (db.ProjectTypes.Any(t => t.Name.ToLower().Equals(projectType.Name.ToLower())))
                {
                    ModelState.AddModelError("Name", "该项目类型已经存在");
                }
                else
                {
                    db.ProjectTypes.Add(projectType);
                    db.SaveChanges();
                    return RedirectToAction("Manage");
                }
            }

            return View(projectType);
        }

        //
        // POST: /ProjectTypes/Update

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public String Update(int id, string value, int? rowId,
               int? columnPosition, int? columnId, string columnName)
        {
            var projectTypes = db.ProjectTypes;

            var projectType = projectTypes.FirstOrDefault(t => t.ID == id);
            if (projectType == null)
            {
                return "ID 为 " + id + " 的项目类型不存在";
            }
            switch (columnPosition)
            {
                case 1:
                    if (projectType.Name == value)
                        return value;
                    if (columnPosition == 1 && projectTypes.Any(t => t.Name.ToLower().Equals(value.ToLower())))
                        return "名字为 \"" + value + "\" 的项目类型已经存在";
                    projectType.Name = value;
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
        // GET: /ProjectTypes/Delete/5

        [Authorize(Roles = "Admin")]
        public String Delete(int id)
        {
            try
            {
                var projectType = db.ProjectTypes.FirstOrDefault(t => t.ID == id);
                if (projectType == null)
                    return "项目类型ID " + id + "不存在";
                db.ProjectTypes.Remove(projectType);
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