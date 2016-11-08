using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using System.Data.Entity.Core.Objects;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "2")]
    public class ArchivoCargaController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        public ActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            if (startDate != null && endDate == null)
            {
                var archivocarga = db.ArchivoCarga.Where(s => DbFunctions.TruncateTime(s.Fecha) == startDate.Value);
                return View(archivocarga.ToList());
            }
            if (startDate != null && endDate != null)
            {
                var archivocarga = db.ArchivoCarga.Where(s => DbFunctions.TruncateTime(s.Fecha) >= startDate.Value && DbFunctions.TruncateTime(s.Fecha) <= endDate.Value);
                return View(archivocarga.ToList());
            }
          
             var archivocarga1 = db.ArchivoCarga.Include(a => a.TipoArchivoCarga1).Include(a => a.User);
             return View(archivocarga1.ToList());
        }
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoCarga archivocarga = db.ArchivoCarga.Find(id);
            if (archivocarga == null)
            {
                return HttpNotFound();
            }
            return View(archivocarga);
        }
        
        public ActionResult Create()
        {
            ViewBag.TipoArchivoCarga = new SelectList(db.TipoArchivoCarga, "Id", "Nombre");
            ViewBag.Usuario = new SelectList(db.User, "Id", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Nombre,Identificador,Fecha,TipoArchivoCarga,Anio_Col3,Mes_Col4,Usuario")] ArchivoCarga archivocarga)
        {
            if (ModelState.IsValid)
            {
                db.ArchivoCarga.Add(archivocarga);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TipoArchivoCarga = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", archivocarga.TipoArchivoCarga);
            ViewBag.Usuario = new SelectList(db.User, "Id", "Username", archivocarga.Usuario);
            return View(archivocarga);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoCarga archivocarga = db.ArchivoCarga.Find(id);
            if (archivocarga == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoArchivoCarga = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", archivocarga.TipoArchivoCarga);
            ViewBag.Usuario = new SelectList(db.User, "Id", "Username", archivocarga.Usuario);
            return View(archivocarga);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Nombre,Identificador,Fecha,TipoArchivoCarga,Anio_Col3,Mes_Col4,Usuario")] ArchivoCarga archivocarga)
        {
            if (ModelState.IsValid)
            {
                db.Entry(archivocarga).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TipoArchivoCarga = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", archivocarga.TipoArchivoCarga);
            ViewBag.Usuario = new SelectList(db.User, "Id", "Username", archivocarga.Usuario);
            return View(archivocarga);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoCarga archivocarga = db.ArchivoCarga.Find(id);
            if (archivocarga == null)
            {
                return HttpNotFound();
            }
            return View(archivocarga);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {           
            db.EliminarArchivoCarga(id);
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
