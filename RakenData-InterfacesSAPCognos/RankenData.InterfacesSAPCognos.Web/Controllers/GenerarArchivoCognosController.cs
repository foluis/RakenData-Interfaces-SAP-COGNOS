using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class GenerarArchivoCognosController : Controller
    {

        private EntitiesRakenData db = new EntitiesRakenData();
        //
        // GET: /GenerarArchivoCognos/
        public ActionResult GenerarArchivo()
        {
            enArchivoCargaCongnos archivo = new enArchivoCargaCongnos();
            ViewBag.LstIdCompaniasCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            var tiposArchivos = new[] {
                new { Id = "1", Descripcion = "Generación Archivo Balance" },
                new { Id = "2", Descripcion = "Generación Archivo Resultados" } 
            };

            ViewBag.TipoArchivo = new SelectList(tiposArchivos, "Id", "Descripcion");
            return View(archivo);
        }

        [HttpPost, ActionName("GenerarArchivo")]
        [ValidateAntiForgeryToken]
        public ActionResult BotonGenerarArchivo(enArchivoCargaCongnos archivoCargaCongnos)
        {
            ViewBag.LstIdCompaniasCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            List<int?> id = db.CreateArchivoBalance(archivoCargaCongnos.LstIdCompaniasCognos.First().ToString(), archivoCargaCongnos.Periodo, archivoCargaCongnos.Anio, archivoCargaCongnos.TipoArchivo, 1).ToList();

            return RedirectToAction("Index", "ArchivoProcesadoDetalle", new { id = id.FirstOrDefault() });
        }

        //
        // GET: /GenerarArchivoCognos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /GenerarArchivoCognos/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /GenerarArchivoCognos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GenerarArchivoCognos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /GenerarArchivoCognos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GenerarArchivoCognos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /GenerarArchivoCognos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
