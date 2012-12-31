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
    public class DepartmentsController : Controller
    {
        private ProjectsContext db = new ProjectsContext();

        //
        // GET: /Departments/Manage

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            var departments = (from d in db.UserDepartments
                            select new DepartmentModel
                            {
                                ID = d.DepartmentId,
                                Name = d.DepartmentName
                            }).ToList();

            int count = 1;
            foreach (var department in departments)
            {
                department.Sequence = count++;
            }
            return View(departments);
        }

        //
        // GET: /Departments/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Departments/Create

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(UserDepartment userDepartment)
        {
            if (ModelState.IsValid)
            {
                if (db.UserDepartments.Any(d => d.DepartmentName.ToLower().Equals(userDepartment.DepartmentName.ToLower())))
                {
                    ModelState.AddModelError("DepartmentName", "该部门名称已经存在");
                }
                else
                {
                    db.UserDepartments.Add(userDepartment);
                    db.SaveChanges();
                    return RedirectToAction("Manage");
                }
            }

            return View(userDepartment);
        }

        //
        // POST: /Departments/Update

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public String Update(int id, string value, int? rowId,
               int? columnPosition, int? columnId, string columnName)
        {
            var departments = db.UserDepartments;

            var department = departments.FirstOrDefault(d => d.DepartmentId == id);
            if (department == null)
            {
                return "ID 为 " + id + " 的部门不存在";
            }
            switch (columnPosition)
            {
                case 1:
                    if (department.DepartmentName == value)
                        return value;
                    if (departments.Any(d => d.DepartmentName.ToLower().Equals(value.ToLower())))
                        return "名字为 \"" + value + "\" 的部门已经存在";
                    department.DepartmentName = value;
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
        // GET: /Departments/Delete/5

        [Authorize(Roles = "Admin")]
        public String Delete(int id)
        {
            try
            {
                var department = db.UserDepartments.FirstOrDefault(d => d.DepartmentId == id);
                if (department == null)
                    return "部门ID " + id + "不存在";
                db.UserDepartments.Remove(department);
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