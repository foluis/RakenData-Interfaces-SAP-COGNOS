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
    public class CuentaSAPController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CuentaSAP/
        public ActionResult Index()
        {
            var cuentasap = db.CuentaSAP.Include(c => c.CuentaCognos1).Include(c => c.TipoCuentaSAP1);
            return View(cuentasap.ToList());
        }

        // GET: /CuentaSAP/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            if (cuentasap == null)
            {
                return HttpNotFound();
            }
            return View(cuentasap);
        }

        // GET: /CuentaSAP/Create
        public ActionResult Create()
        {
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre");
            return View();
        }

        // POST: /CuentaSAP/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Numero,Descripcion,CuentaCognos,IsActive,TipoCuentaSAP,EsOpen,CuentaCargo,CuentaAbono")] CuentaSAP cuentasap)
        {
            if (ModelState.IsValid)
            {
                db.CuentaSAP.Add(cuentasap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);

            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            return View(cuentasap);
        }

        // GET: /CuentaSAP/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            if (cuentasap == null)
            {
                return HttpNotFound();
            }
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            return View(cuentasap);
        }

        // POST: /CuentaSAP/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Numero,Descripcion,CuentaCognos,IsActive,TipoCuentaSAP,EsOpen,CuentaCargo,CuentaAbono")] CuentaSAP cuentasap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentasap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            return View(cuentasap);
        }

        // GET: /CuentaSAP/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            if (cuentasap == null)
            {
                return HttpNotFound();
            }
            return View(cuentasap);
        }

        // POST: /CuentaSAP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            db.CuentaSAP.Remove(cuentasap);
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
