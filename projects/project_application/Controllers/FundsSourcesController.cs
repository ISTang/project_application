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
    public class FundsSourcesController : Controller
    {
        private ProjectsContext db = new ProjectsContext();

        //
        // GET: /FundsSources/Manage

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            var fundsSources = (from t in db.FundsSources
                            select new FundsSourceModel
                            {
                                ID = t.ID,
                                Name = t.Name
                            }).ToList();

            int count = 1;
            foreach (var type in fundsSources)
            {
                type.Sequence = count++;
            }
            return View(fundsSources);
        }

        //
        // GET: /FundsSources/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FundsSources/Create

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(FundsSource fundsSource)
        {
            if (ModelState.IsValid)
            {
                if (db.FundsSources.Any(t => t.Name.ToLower().Equals(fundsSource.Name.ToLower())))
                {
                    ModelState.AddModelError("Name", "该资金来源已经存在");
                }
                else
                {
                    db.FundsSources.Add(fundsSource);
                    db.SaveChanges();
                    return RedirectToAction("Manage");
                }
            }

            return View(fundsSource);
        }

        //
        // POST: /FundsSources/Update

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public String Update(int id, string value, int? rowId,
               int? columnPosition, int? columnId, string columnName)
        {
            var fundsSources = db.FundsSources;

            var fundsSource = fundsSources.FirstOrDefault(t => t.ID == id);
            if (fundsSource == null)
            {
                return "ID 为 " + id + " 的资金来源不存在";
            }
            switch (columnPosition)
            {
                case 1:
                    if (fundsSource.Name == value)
                        return value;
                    if (fundsSources.Any(t => t.Name.ToLower().Equals(value.ToLower())))
                        return "名字为 \"" + value + "\" 的资金来源已经存在";
                    fundsSource.Name = value;
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
        // GET: /FundsSources/Delete/5

        [Authorize(Roles = "Admin")]
        public String Delete(int id)
        {
            try
            {
                var fundsSource = db.FundsSources.FirstOrDefault(t => t.ID == id);
                if (fundsSource == null)
                    return "资金来源ID " + id + "不存在";
                db.FundsSources.Remove(fundsSource);
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