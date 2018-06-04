using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using capstone6_task_mgr_web_app.Models;

namespace capstone6_task_mgr_web_app.Controllers
{
    public class TasksController : Controller
    {
        private TaskMgrDBEntities db = new TaskMgrDBEntities();

        // GET: Tasks
        public ActionResult Index(string sortBy, string searching, string searchingComp, string searchingDate, string searchingEmail)
        {
            DateTime.TryParse(searchingDate, out DateTime convertedDate);

            var tasks = from t in db.Tasks
                        select t;

            bool convertedComp = Convert.ToBoolean(searchingComp);

            if (!String.IsNullOrEmpty(searching))
            {
                tasks = tasks.Where(t => t.Description.Contains(searching));
            }

            if (!String.IsNullOrEmpty(searchingComp))
            {
                tasks = tasks.Where(t => t.Completed == convertedComp);
            }

            if (!String.IsNullOrEmpty(searchingDate))
            {
                tasks = tasks.Where(t => t.DueDate == convertedDate);
            }

            if (!String.IsNullOrEmpty(searchingEmail))
            {
                tasks = tasks.Where(t => t.User.Email.Contains(searchingEmail));
            }

            ViewBag.SortDueDateParameter = string.IsNullOrEmpty(sortBy) ? "DueDate desc" : "";
            ViewBag.SortDescriptionParameter = sortBy == "Description" ? "Description desc" : "Description";
            ViewBag.SortEmailParameter = sortBy == "Email" ? "Email desc" : "Email";
            ViewBag.SortCompletedParameter = sortBy == "Completed" ? "Completed desc" : "Completed";


            switch (sortBy)
            {
                case "Email desc":
                    tasks = tasks.OrderByDescending(t => t.User.Email);
                    break;
                case "Email":
                    tasks = tasks.OrderBy(t => t.User.Email);
                    break;
                case "Description desc":
                    tasks = tasks.OrderByDescending(t => t.Description);
                    break;
                case "Description":
                    tasks = tasks.OrderBy(t => t.Description);
                    break;
                case "DueDate desc":
                    tasks = tasks.OrderBy(x => x.DueDate.ToString());
                    break;
                case "Completed desc":
                    tasks = tasks.OrderByDescending(x => x.Completed);
                    break;
                case "Completed":
                    tasks = tasks.OrderBy(x => x.Completed);
                    break;
                default:
                    tasks = tasks.OrderByDescending(x => x.DueDate.ToString());
                    break;

            }

            return View(tasks.ToList());
        }

        // GET: Tasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            ViewBag.Owner = new SelectList(db.Users, "UserID", "Email");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskID,Description,DueDate,Completed,Owner")] Task task)
        {
            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Owner = new SelectList(db.Users, "UserID", "Email", task.Owner);
            return View(task);
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            ViewBag.Owner = new SelectList(db.Users, "UserID", "Email", task.Owner);
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskID,Description,DueDate,Completed,Owner")] Task task)
        {
            if (ModelState.IsValid)
            {
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Owner = new SelectList(db.Users, "UserID", "Email", task.Owner);
            return View(task);
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
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
