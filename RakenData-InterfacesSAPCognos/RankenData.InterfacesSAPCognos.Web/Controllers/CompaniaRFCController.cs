using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CompaniaRFCController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CompaniaRFC/
        public ActionResult Index()
        {
            var companiarfc = db.CompaniaRFC.Include(c => c.CompaniaCognos1);
            return View(companiarfc.ToList());
        }

        // GET: /CompaniaRFC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Create
        public ActionResult Create()
        {
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            return View();
        }

        // POST: /CompaniaRFC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,RFC,Descripcion,CompaniaCognos")] CompaniaRFC companiarfc)
        {
            if (ModelState.IsValid)
            {
                db.CompaniaRFC.Add(companiarfc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            return View(companiarfc);
        }

        // POST: /CompaniaRFC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,RFC,Descripcion,CompaniaCognos")] CompaniaRFC companiarfc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companiarfc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            return View(companiarfc);
        }

        // POST: /CompaniaRFC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            db.CompaniaRFC.Remove(companiarfc);
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
