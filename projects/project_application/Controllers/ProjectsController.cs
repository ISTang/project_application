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
    /// <summary>
    /// Class that encapsulates most common parameters sent by DataTables plugin
    /// </summary>
    public class jQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }
    }

    [Authorize]
    public class ProjectsController : Controller
    {
        private UsersContext usersContext = new UsersContext();
        private ProjectsContext db = new ProjectsContext();

        //
        // GET: /Projects/Manage

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            var str = "";
            foreach (var item in db.ProjectTypes.ToList())
            {
                str += str != "" ? "," : "";
                str += string.Format("'{0}':'{1}'", item.ID, item.Name);
            }
            ViewBag.ProjectTypesJson = "{" + str + "}";

            str = "";
            foreach (var item in db.UserDepartments.ToList())
            {
                str += str != "" ? "," : "";
                str += string.Format("'{0}':'{1}'", item.DepartmentId, item.DepartmentName);
            }
            ViewBag.DepartmentNamesJson = "{" + str + "}";

            str = "";
            foreach (var item in db.ProjectStages.ToList())
            {
                str += str != "" ? "," : "";
                str += string.Format("'{0}':'{1}'", item.ID, item.Name);
            }
            ViewBag.ProjectStagesJson = "{" + str + "}";

            return View();
        }

        //
        // GET: /Projects/AjaxHandler

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AjaxHandler(jQueryDataTableParamModel param)
        {
            // 查询
            var allProjects = from p in db.BasicProjects
                              from t in db.ProjectTypes
                              from d in db.UserDepartments
                              from s in db.ProjectStages
                              where p.Type.ID == t.ID &&
                               p.Department.DepartmentId == d.DepartmentId &&
                               p.Stage.ID == s.ID
                              select new ProjectModel
                              {
                                  ID = p.ID,
                                  Number = p.Number,
                                  Name = p.Name,
                                  TypeId = t.ID,
                                  TypeName = t.Name,
                                  DepartmentId = d.DepartmentId,
                                  DepartmentName = d.DepartmentName,
                                  StageId = s.ID,
                                  StageName = s.Name,
                                  Memo = p.Memo
                              };

            // 过滤
            IEnumerable<ProjectModel> filteredProjects;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredProjects = allProjects.Where(p => p.Number.Contains(param.sSearch)
                                     ||
                          p.Name.Contains(param.sSearch)
                                     ||
                                     p.TypeName.Contains(param.sSearch) ||
                                     p.DepartmentName.Contains(param.sSearch) ||
                                     p.StageName.Contains(param.sSearch) ||
                                     p.Memo.Contains(param.sSearch));
            }
            else
            {
                filteredProjects = allProjects;
            }

            // 排序
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ProjectModel, string> orderingFunction = (p => sortColumnIndex == 1 ? p.Number :
                                                                sortColumnIndex == 2 ? p.Name :
                                                                sortColumnIndex == 3 ? p.TypeName :
                                                                sortColumnIndex == 4 ? p.DepartmentName :
                                                                sortColumnIndex == 5 ? p.StageName :
                                                                p.Memo);
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredProjects = filteredProjects.OrderBy(orderingFunction);
            else
                filteredProjects = filteredProjects.OrderByDescending(orderingFunction);


            // 分页
            var displayedProjects = filteredProjects
                        .Skip(param.iDisplayStart)
                        .Take(param.iDisplayLength);

            int count = param.iDisplayStart + 1;
            var result = from p in displayedProjects
                         select new[] { Convert.ToString(p.ID), Convert.ToString(count++), p.Number, p.Name, p.TypeName, p.DepartmentName, p.StageName, p.Memo };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = allProjects.Count(),
                iTotalDisplayRecords = filteredProjects.Count(),
                aaData = result
            },
            JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Projects/Create

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            return View(new BasicProject());
        }

        //
        // POST: /Projects/Create

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(BasicProject project)
        {
            if (!ModelState.IsValidField("Type"))
            {
                var typeId = int.Parse(Request.Params["Type"]);
                var type = db.ProjectTypes.First(d => d.ID == typeId);
                if (type == null)
                {
                    ModelState.AddModelError("Type", "项目类型模型转换失败");
                }
                else
                {
                    project.Type = type;
                    ModelState.Remove("Type");
                }
            }

            if (!ModelState.IsValidField("Department"))
            {
                var departmentId = int.Parse(Request.Params["Department"]);
                var department = db.UserDepartments.First(d => d.DepartmentId == departmentId);
                if (department == null)
                {
                    ModelState.AddModelError("Department", "部门模型转换失败");
                }
                else
                {
                    project.Department = department;
                    ModelState.Remove("Department");
                }
            }

            if (!ModelState.IsValidField("Stage"))
            {
                var stageId = int.Parse(Request.Params["Stage"]);
                var stage = db.ProjectStages.First(d => d.ID == stageId);
                if (stage == null)
                {
                    ModelState.AddModelError("Stage", "项目类型模型转换失败");
                }
                else
                {
                    project.Stage = stage;
                    ModelState.Remove("Stage");
                }
            }

            if (ModelState.IsValid)
            {
                if (db.BasicProjects.Any(p => p.Name.ToLower().Equals(project.Name.ToLower())))
                {
                    ModelState.AddModelError("Name", "该项目名称已经存在");
                }
                else
                {
                    db.BasicProjects.Add(project);
                    db.SaveChanges();
                    return RedirectToAction("Manage");
                }
            }

            return View(project);
        }

        //
        // POST: /Projects/Update

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public String Update(int id, string value, int? rowId,
               int? columnPosition, int? columnId, string columnName)
        {
            var projects = db.BasicProjects;

            var project = projects.FirstOrDefault(p => p.ID == id);
            if (project == null)
            {
                return "ID 为 " + id + " 的项目不存在";
            }
            switch (columnPosition)
            {
                case 1:
                    if (project.Number == value)
                        return value;
                    project.Number = value;
                    RestoreObjectFields(project);
                    break;
                case 2:
                    if (project.Name == value)
                        return value;
                    if (projects.Any(p => p.Name.ToLower().Equals(value.ToLower())))
                        return "名字为 \"" + value + "\" 的项目已经存在";
                    project.Name = value;
                    RestoreObjectFields(project);
                    break;
                case 3:
                    var intValue = int.Parse(value);
                    if ((from p in db.BasicProjects
                         from t in db.ProjectTypes
                         where p.ID == project.ID && p.Type.ID == t.ID
                         select t).Single().ID == intValue)
                        return value;
                    if (!db.ProjectTypes.Any(t => t.ID == intValue))
                    {
                        return "不存在项目类型ID \"" + value + "\"";
                    }
                    project.Type = db.ProjectTypes.Single(t => t.ID == intValue);
                    break;
                case 4:
                    intValue = int.Parse(value);
                    if ((from p in db.BasicProjects
                         from d in db.UserDepartments
                         where p.ID == project.ID && p.Department.DepartmentId == d.DepartmentId
                         select d).Single().DepartmentId == intValue)
                        return value;
                    if (!db.UserDepartments.Any(d => d.DepartmentId == intValue))
                    {
                        return "不存在部门ID \"" + value + "\"";
                    }
                    project.Department = db.UserDepartments.Single(d => d.DepartmentId == intValue);
                    break;
                case 5:
                    intValue = int.Parse(value);
                    if ((from p in db.BasicProjects
                         from s in db.ProjectStages
                         where p.ID == project.ID && p.Stage.ID == s.ID
                         select s).Single().ID == intValue)
                        return value;
                    if (!db.ProjectStages.Any(s => s.ID == intValue))
                    {
                        return "不存在项目阶段ID \"" + value + "\"";
                    }
                    project.Stage = db.ProjectStages.Single(s => s.ID == intValue);
                    break;
                case 6:
                    if (project.Memo == value)
                        return value;
                    project.Memo = value;
                    RestoreObjectFields(project);
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
        // GET: /Projects/Delete/5

        [Authorize(Roles = "Admin")]
        public String Delete(int id)
        {
            try
            {
                var project = db.BasicProjects.FirstOrDefault(p => p.ID == id);
                if (project == null)
                    return "项目ID " + id + "不存在";
                db.BasicProjects.Remove(project);
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

        //
        // GET: /Projects/Search/

        public ActionResult Search(string number, string name, int? type, int? department, int? stage)
        {
            var bp = new BasicProject();
            ViewData["Type"] = bp.GetProjectTypeList();
            ViewData["Department"] = bp.GetUserDepartmentList();
            ViewData["Stage"] = bp.GetProjectStageList();

            if (number == null && name == null && type == null && department == null && stage == null)
            {
                return View(new List<ProjectModel>());
            }

            // 查询符合条件的项目
            var projects = from p in db.BasicProjects
                           from t in db.ProjectTypes
                           from d in db.UserDepartments
                           from s in db.ProjectStages
                           where p.Type.ID == t.ID &&
                            p.Department.DepartmentId == d.DepartmentId &&
                            p.Stage.ID == s.ID
                           select new ProjectModel
                           {
                               Number = p.Number,
                               Name = p.Name,
                               TypeId = t.ID,
                               TypeName = t.Name,
                               DepartmentId = d.DepartmentId,
                               DepartmentName = d.DepartmentName,
                               StageId = s.ID,
                               StageName = s.Name
                           };
            if (!String.IsNullOrEmpty(number))
            {
                projects = projects.Where(s => s.Number.Contains(number));
            }
            if (!String.IsNullOrEmpty(name))
            {
                projects = projects.Where(s => s.Name.Contains(name));
            }
            if (type != null)
            {
                projects = projects.Where(s => s.TypeId == type);
            }
            if (department != null)
            {
                projects = projects.Where(s => s.DepartmentId == department);
            }
            if (stage != null)
            {
                projects = projects.Where(s => s.StageId == stage);
            }
            //

            var projects2 = projects.ToList();

            int count = 1;
            foreach (var project in projects2)
            {
                project.Sequence = count++;
            }
            return View(projects2);
        }

        //
        // GET: /Projects/Browse/

        public ActionResult Browse(String stage)
        {
            if (stage == null)
            {
                return View(new List<ProjectModel>());
            }

            // 查询符合条件的项目
            var projects = from p in db.BasicProjects
                           from t in db.ProjectTypes
                           from d in db.UserDepartments
                           from s in db.ProjectStages
                           where p.Type.ID == t.ID &&
                            p.Department.DepartmentId == d.DepartmentId &&
                            p.Stage.ID == s.ID
                           select new ProjectModel
                           {
                               Number = p.Number,
                               Name = p.Name,
                               TypeName = t.Name,
                               DepartmentName = d.DepartmentName,
                               StageName = s.Name
                           };
            if (!string.IsNullOrEmpty(stage))
            {
                projects = projects.Where(x => x.StageName == stage);
            }


            var projects2 = projects.ToList();

            int count = 1;
            foreach (var project in projects2)
            {
                project.Sequence = count++;
            }
            return View(projects2);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void RestoreObjectFields(BasicProject project)
        {
            project.Type = (from p in db.BasicProjects from t in db.ProjectTypes where p.ID == project.ID && p.Type.ID == t.ID select t).Single();
            project.Department = (from p in db.BasicProjects from d in db.UserDepartments where p.ID == project.ID && p.Department.DepartmentId == d.DepartmentId select d).Single();
            project.Stage = (from p in db.BasicProjects from s in db.ProjectStages where p.ID == project.ID && p.Stage.ID == s.ID select s).Single();
        }

    }
}