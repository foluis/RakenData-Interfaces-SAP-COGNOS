using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using System.Data.Entity.Validation;
using System.Text;
using System.Data.Entity.Infrastructure;
using Ranken.ISC.FileManager.WriteFiles;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class ArchivoProcesadoDetalleController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /ArchivoProcesadoDetalle/
        public ActionResult Index(string id)
        {
            int idArchivo;
            List<ArchivoProcesadoDetalle> archivoprocesadodetalle;
            if (int.TryParse(id, out idArchivo))
            {
                archivoprocesadodetalle = db.ArchivoProcesadoDetalle.Include(a => a.ArchivoProcesado).Where(ap => ap.Id == idArchivo).ToList();
            }
            else
            {
                archivoprocesadodetalle = db.ArchivoProcesadoDetalle.Include(a => a.ArchivoProcesado).ToList();
            }

            TempData["archivoprocesadodetalle"] = archivoprocesadodetalle;
            return View(archivoprocesadodetalle);
        }

        // GET: /Generar archivo CSV/

        public ActionResult GenerarArchivo()
        {
            List<ArchivoProcesadoDetalle> archivoprocesadodetalle = (List<ArchivoProcesadoDetalle>)TempData["archivoprocesadodetalle"];
            string ruta = db.AdministracionAplicacion.Where(aa => aa.Id == 3).FirstOrDefault().Nombre;
            CSV_Writer csvWriter = new CSV_Writer();

            db.ArchivoProcesado.Find(archivoprocesadodetalle.First().Id);
            List<ArchivoResultado> lstArchivoResultado = archivoprocesadodetalle.ConvertAll(
                ap => new ArchivoResultado()
                {
                    Account = ap.Account,
                    AccountName = ap.AccountName,
                    Actuality = ap.Actuality,
                    Amount = ap.Amount.ToString(),
                    Company = ap.Company,
                    CounterCompany = ap.CounterCompany,
                    Dim1 = ap.Dim1,
                    Dim2 = ap.Dim2,
                    Dim3 = ap.Dim3,
                    Form = ap.Form,
                    ITOpex = ap.ITOpex,
                    Period = ap.Period,
                    Retrieve = ap.Retrieve,
                    TransactionAmount = ap.TransactionAmount.ToString(),
                    TransactionCurrency = ap.TransactionCurrency,
                    Variance = ap.Variance
                });
            csvWriter.StartWritingArchivoBalance("Descripcion 1", "2015", "11", ruta, lstArchivoResultado);
            return View();
        }

        // GET: /ArchivoProcesadoDetalle/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoProcesadoDetalle archivoprocesadodetalle = db.ArchivoProcesadoDetalle.Find(id);
            if (archivoprocesadodetalle == null)
            {
                return HttpNotFound();
            }
            return View(archivoprocesadodetalle);
        }

        // GET: /ArchivoProcesadoDetalle/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoProcesadoDetalle archivoprocesadodetalle = db.ArchivoProcesadoDetalle.Find(id);
            if (archivoprocesadodetalle == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArchivoProcesadoId = new SelectList(db.ArchivoProcesado, "Id", "Id");
            return View(archivoprocesadodetalle);
        }

        // POST: /ArchivoProcesadoDetalle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ArchivoProcesadoId,TipoArchivoCreacionId,Company,Period,Actuality,Account,CounterCompany,Dim1,Dim2,Dim3,ITOpex,Amount,TransactionCurrency,TransactionAmount,Form,AccountName,Retrieve,Variance")] ArchivoProcesadoDetalle archivoprocesadodetalle)
        {
            if (ModelState.IsValid)
            {
                db.ArchivoProcesadoDetalle.Add(archivoprocesadodetalle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArchivoProcesadoId = new SelectList(db.ArchivoProcesado, "Id", "Id", archivoprocesadodetalle.ArchivoProcesadoId);
            return View(archivoprocesadodetalle);
        }

        // GET: /ArchivoProcesadoDetalle/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoProcesadoDetalle archivoprocesadodetalle = db.ArchivoProcesadoDetalle.Find(id);
            if (archivoprocesadodetalle == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArchivoProcesadoId = new SelectList(db.ArchivoProcesado, "Id", "Id", archivoprocesadodetalle.ArchivoProcesadoId);
            return View(archivoprocesadodetalle);
        }

        // POST: /ArchivoProcesadoDetalle/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ArchivoProcesadoId,TipoArchivoCreacionId,Company,Period,Actuality,Account,CounterCompany,Dim1,Dim2,Dim3,ITOpex,Amount,TransactionCurrency,TransactionAmount,Form,AccountName,Retrieve,Variance")] ArchivoProcesadoDetalle archivoprocesadodetalle)
        {
            StringBuilder errores = new StringBuilder();
            if (ModelState.IsValid)
            {
                db.Entry(archivoprocesadodetalle).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                    ModelState.AddModelError("Error", errores.ToString());
                    return View();
                }
                catch (DbUpdateException e)
                {
                    errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                    ModelState.AddModelError("Error", errores.ToString());
                    return View();
                }
                catch (Exception e)
                {
                    errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                    ModelState.AddModelError("Error", errores.ToString());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ViewBag.ArchivoProcesadoId = new SelectList(db.ArchivoProcesado, "Id", "Id", archivoprocesadodetalle.ArchivoProcesadoId);
            return View(archivoprocesadodetalle);
        }

        // GET: /ArchivoProcesadoDetalle/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoProcesadoDetalle archivoprocesadodetalle = db.ArchivoProcesadoDetalle.Find(id);
            if (archivoprocesadodetalle == null)
            {
                return HttpNotFound();
            }
            return View(archivoprocesadodetalle);
        }

        // POST: /ArchivoProcesadoDetalle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ArchivoProcesadoDetalle archivoprocesadodetalle = db.ArchivoProcesadoDetalle.Find(id);
            db.ArchivoProcesadoDetalle.Remove(archivoprocesadodetalle);
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
