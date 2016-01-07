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
    public class CargaAutomaticaController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CargaAutomatica/
        public ActionResult Index()
        {
            var cargaautomatica = db.CargaAutomatica.Include(c => c.ArchivoCarga1).Include(c => c.TipoArchivoCarga);
            return View(cargaautomatica.ToList());
        }

        // GET: /CargaAutomatica/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CargaAutomatica cargaautomatica = db.CargaAutomatica.Find(id);
            if (cargaautomatica == null)
            {
                return HttpNotFound();
            }
            return View(cargaautomatica);
        }

        // GET: /CargaAutomatica/Create
        public ActionResult Create()
        {
            ViewBag.ArchivoCarga = new SelectList(db.ArchivoCarga, "Id", "Nombre");
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre");
            return View();
        }

        // POST: /CargaAutomatica/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,FechaProgramada,Usuario,TipoArchivo,ArchivoCarga,FechaFinalizacion")] CargaAutomatica cargaautomatica)
        {
            if (ModelState.IsValid)
            {
                db.CargaAutomatica.Add(cargaautomatica);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArchivoCarga = new SelectList(db.ArchivoCarga, "Id", "Nombre", cargaautomatica.ArchivoCarga);
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaautomatica.TipoArchivo);
            return View(cargaautomatica);
        }

        // GET: /CargaAutomatica/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CargaAutomatica cargaautomatica = db.CargaAutomatica.Find(id);
            if (cargaautomatica == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArchivoCarga = new SelectList(db.ArchivoCarga, "Id", "Nombre", cargaautomatica.ArchivoCarga);
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaautomatica.TipoArchivo);
            return View(cargaautomatica);
        }

        // POST: /CargaAutomatica/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,FechaProgramada,Usuario,TipoArchivo,ArchivoCarga,FechaFinalizacion")] CargaAutomatica cargaautomatica)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cargaautomatica).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArchivoCarga = new SelectList(db.ArchivoCarga, "Id", "Nombre", cargaautomatica.ArchivoCarga);
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaautomatica.TipoArchivo);
            return View(cargaautomatica);
        }

        // GET: /CargaAutomatica/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CargaAutomatica cargaautomatica = db.CargaAutomatica.Find(id);
            if (cargaautomatica == null)
            {
                return HttpNotFound();
            }
            return View(cargaautomatica);
        }

        // POST: /CargaAutomatica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CargaAutomatica cargaautomatica = db.CargaAutomatica.Find(id);
            db.CargaAutomatica.Remove(cargaautomatica);
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
