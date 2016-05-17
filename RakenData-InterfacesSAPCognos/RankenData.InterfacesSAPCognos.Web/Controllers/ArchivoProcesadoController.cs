﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using System.Data.Entity.Validation;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using System.Data.Entity.Infrastructure;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "4")]
    public class ArchivoProcesadoController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /ArchivoProcesado/
          //[Authorize(Roles = "3")]
        public ActionResult Index()
        {
            var archivoprocesado = db.ArchivoProcesado.Include(a => a.CompaniaCognos).Include(a => a.TipoArchivoCreacion).Include(a => a.User);
            return View(archivoprocesado.ToList());
        }



        // GET: /ArchivoProcesado/ConsultarDetalle/5
        public ActionResult ConsultarDetalle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoProcesado archivoprocesado = db.ArchivoProcesado.Find(id);
            if (archivoprocesado == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index", "ArchivoProcesadoDetalle", new { id = id, tipoArchivo = archivoprocesado.TipoArchivoCreacionId });           
        }

        // GET: /ArchivoProcesado/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoProcesado archivoprocesado = db.ArchivoProcesado.Find(id);
            if (archivoprocesado == null)
            {
                return HttpNotFound();
            }
            return View(archivoprocesado);
        }

        // GET: /ArchivoProcesado/Create
        public ActionResult Create()
        {
            ViewBag.CompaniaCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            ViewBag.TipoArchivoCreacionId = new SelectList(db.TipoArchivoCreacion, "Id", "Nombre");
            ViewBag.UsuarioId = new SelectList(db.User, "Id", "Username");
            return View();
        }

        // POST: /ArchivoProcesado/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,CompaniaCognosId,Periodo,Anio,TipoArchivoCreacionId,ArchivoGenerado,FechaArchivoGenerado,FechaProcesoArchivo,UsuarioId")] ArchivoProcesado archivoprocesado)
        {
            if (ModelState.IsValid)
            {
                db.ArchivoProcesado.Add(archivoprocesado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompaniaCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion", archivoprocesado.CompaniaCognosId);
            ViewBag.TipoArchivoCreacionId = new SelectList(db.TipoArchivoCreacion, "Id", "Nombre", archivoprocesado.TipoArchivoCreacionId);
            ViewBag.UsuarioId = new SelectList(db.User, "Id", "Username", archivoprocesado.UsuarioId);
            return View(archivoprocesado);
        }

        // GET: /ArchivoProcesado/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoProcesado archivoprocesado = db.ArchivoProcesado.Find(id);
            if (archivoprocesado == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompaniaCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion", archivoprocesado.CompaniaCognosId);
            ViewBag.TipoArchivoCreacionId = new SelectList(db.TipoArchivoCreacion, "Id", "Nombre", archivoprocesado.TipoArchivoCreacionId);
            ViewBag.UsuarioId = new SelectList(db.User, "Id", "Username", archivoprocesado.UsuarioId);
            return View(archivoprocesado);
        }

        // POST: /ArchivoProcesado/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,CompaniaCognosId,Periodo,Anio,TipoArchivoCreacionId,ArchivoGenerado,FechaArchivoGenerado,FechaProcesoArchivo,UsuarioId")] ArchivoProcesado archivoprocesado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(archivoprocesado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompaniaCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion", archivoprocesado.CompaniaCognosId);
            ViewBag.TipoArchivoCreacionId = new SelectList(db.TipoArchivoCreacion, "Id", "Nombre", archivoprocesado.TipoArchivoCreacionId);
            ViewBag.UsuarioId = new SelectList(db.User, "Id", "Username", archivoprocesado.UsuarioId);
            return View(archivoprocesado);
        }

        // GET: /ArchivoProcesado/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoProcesado archivoprocesado = db.ArchivoProcesado.Find(id);
            if (archivoprocesado == null)
            {
                return HttpNotFound();
            }
            return View(archivoprocesado);
        }

        // POST: /ArchivoProcesado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //ArchivoProcesado archivoprocesado = db.ArchivoProcesado.Find(id);
            //db.ArchivoProcesado.Remove(archivoprocesado);

            db.EliminarArchivoProcesado(id);

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                ModelState.AddModelError("Error", "No se pudo eliminar el archivo");
                return View();
            }
            catch (DbUpdateException e)
            {
                Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                ModelState.AddModelError("Error", "No se pudo eliminar el archivo");
                return View();
            }
            catch (Exception e)
            {
                Log.WriteLog(ManejoErrores.ErrorExepcion(e), EnumTypeLog.Error, true);
                ModelState.AddModelError("Error", "No se pudo eliminar el archivo");
                return View();
            }
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
