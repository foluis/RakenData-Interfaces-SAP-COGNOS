using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CompaniaCognosController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CompaniaCognos/
        public ActionResult Index()
        {
            return View(db.CompaniaCognos.ToList());
        }

        // GET: /CompaniaCognos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaCognos companiacognos = db.CompaniaCognos.Find(id);
            if (companiacognos == null)
            {
                return HttpNotFound();
            }
            return View(companiacognos);
        }

        // GET: /CompaniaCognos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /CompaniaCognos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Clave,Descripcion")] CompaniaCognos companiacognos)
        {
            if (ModelState.IsValid)
            {
                db.CompaniaCognos.Add(companiacognos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(companiacognos);
        }

        // GET: /CompaniaCognos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaCognos companiacognos = db.CompaniaCognos.Find(id);
            if (companiacognos == null)
            {
                return HttpNotFound();
            }
            return View(companiacognos);
        }

        // POST: /CompaniaCognos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Clave,Descripcion")] CompaniaCognos companiacognos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companiacognos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(companiacognos);
        }

        // GET: /CompaniaCognos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaCognos companiacognos = db.CompaniaCognos.Find(id);
            if (companiacognos == null)
            {
                return HttpNotFound();
            }
            return View(companiacognos);
        }

        // POST: /CompaniaCognos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompaniaCognos companiacognos = db.CompaniaCognos.Find(id);
            db.CompaniaCognos.Remove(companiacognos);
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
