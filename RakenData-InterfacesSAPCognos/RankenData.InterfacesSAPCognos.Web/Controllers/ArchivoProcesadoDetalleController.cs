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
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using Ranken.ISC.FileManager;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "4")]
    public class ArchivoProcesadoDetalleController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /ArchivoProcesadoDetalle/
        public ActionResult Index(string id, int? tipoArchivo, string error = null)
        {
            int idArchivo;
            List<ArchivoProcesadoDetalle> archivoprocesadodetalle;
            if (!int.TryParse(id, out idArchivo))
            {
                ModelState.AddModelError("Error", "El id de la tabla no es numerico");
                return View();
            }
            if (idArchivo == -1)
            {
                ModelState.AddModelError("Error", "No existen datos actualmente.");
                return View();
            }

            ////Habilita el boton editar si el archivo no se ha generado nunca
            archivoprocesadodetalle = db.ArchivoProcesadoDetalle.Include(a => a.ArchivoProcesado).Where(ap => ap.ArchivoProcesadoId == idArchivo).ToList();
            if (db.ArchivoProcesado.Find(idArchivo).ArchivoGenerado)
            {
                foreach (ArchivoProcesadoDetalle archivoProcesoD in archivoprocesadodetalle)
                {
                    archivoProcesoD.EsModificable = false;
                }
            }
            else
            {
                //Habilita el boton editar dependiento si el anexo permite modificar
                foreach (ArchivoProcesadoDetalle archivoProcesoD in archivoprocesadodetalle)
                {
                    CuentaCognos cuentaCognos = db.CuentaCognos.FirstOrDefault(cc => cc.Numero == archivoProcesoD.Account);
                    if (cuentaCognos != null && cuentaCognos.Anexo.Modificable == false)
                    {
                        archivoProcesoD.EsModificable = false;
                    }
                    else
                    {
                        archivoProcesoD.EsModificable = true;
                    }
                }
            }

            if (error != null)
            {
                ModelState.AddModelError("Error", error);
            }
            TempData["id"] = id;
            TempData["tipoArchivo"] = tipoArchivo;
            TempData["archivoprocesadodetalle"] = archivoprocesadodetalle;
            return View(archivoprocesadodetalle);
        }

        // GET: /Generar archivo CSV/

        public ActionResult GenerarArchivo()
        {
            string id = TempData["id"].ToString();
            int tipoArchivo = (int)TempData["tipoArchivo"];

            List<ArchivoProcesadoDetalle> archivoprocesadodetalle = (List<ArchivoProcesadoDetalle>)TempData["archivoprocesadodetalle"];
            if (archivoprocesadodetalle.Count == 0)
            {
                return RedirectToAction("Index", new { id = id, tipoArchivo = tipoArchivo, error = "No hay información para generar el archivo" });
            }

            //string ruta = HttpContext.Current.Server.MapPath(@"\Log");
            string ruta = Server.MapPath(@"\ArchivosCreados");


            CSV_Writer csvWriter = new CSV_Writer();

            ArchivoProcesado archivoProcesado = db.ArchivoProcesado.Find(archivoprocesadodetalle.First().ArchivoProcesadoId);
            List<ArchivoResultado> lstArchivoResultado = archivoprocesadodetalle.ConvertAll(
                ap => new ArchivoResultado()
                {
                    Account = ap.Account,
                    AccountName = ap.AccountName,
                    Actuality = ap.Actuality,                               
                    Amount = string.Format("{0:n0}", ap.Amount).Replace(",",""),
                    Company = archivoProcesado.CompaniaCognos.Clave.ToString(),
                    CounterCompany = ap.CounterCompany,
                    Dim1 = ap.Dim1,
                    Dim2 = ap.Dim2,
                    Dim3 = ap.Dim3,
                    Form = ap.Form,
                    ITOpex = ap.ITOpex,
                    Period = ap.Period,
                    Retrieve = ap.Retrieve,                    
                    TransactionAmount = string.Format("{0:n0}", ap.TransactionAmount).Replace(",", ""),
                    TransactionCurrency = ap.TransactionCurrency,
                    Variance = ap.Variance
                });

            try
            {
                OperationResult archivoCreado = csvWriter.StartWritingArchivoBalance(archivoProcesado.CompaniaCognos.Clave.ToString(), archivoProcesado.Anio.ToString(), archivoProcesado.Periodo.ToString(), tipoArchivo, ruta, lstArchivoResultado);
                if (archivoCreado.IdError == 0)//Archivo Creado
                {
                    archivoProcesado.ArchivoGenerado = true;
                    archivoProcesado.FechaArchivoGenerado = DateTime.Now;
                    db.Entry(archivoProcesado).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else // Archivo con errore
                {
                    ModelState.AddModelError("Error", "El archivo no se pudo crear.");
                    Log.WriteLog("Valide los permisos de la carpeta", EnumTypeLog.Error, true);
                    return RedirectToAction("Index", new { id = id, tipoArchivo = tipoArchivo, error = "El archivo no se pudo crear." });
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString(), EnumTypeLog.Error, true);                
            }            
           
            return RedirectToAction("Index", new { id = id, tipoArchivo = tipoArchivo, error = "El archivo se creó satisfactoriamente." });
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
        public ActionResult Create()
        {
            return View();

        }

        // POST: /ArchivoProcesadoDetalle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Account,Amount,TransactionAmount")] ArchivoProcesadoDetalle archivoprocesadodetalle)
        {
            int tipoArchivo = (int)TempData["tipoArchivo"];
            List<ArchivoProcesadoDetalle> lstArchivoprocesadodetalle = (List<ArchivoProcesadoDetalle>)TempData["archivoprocesadodetalle"];

            ArchivoProcesadoDetalle archivoprocesadodetalleTemplate = lstArchivoprocesadodetalle.First();
            //Mapeo
            archivoprocesadodetalle.ArchivoProcesadoId = archivoprocesadodetalleTemplate.ArchivoProcesadoId;
            archivoprocesadodetalle.Company = archivoprocesadodetalleTemplate.Company;
            archivoprocesadodetalle.Period = archivoprocesadodetalleTemplate.Period;
            archivoprocesadodetalle.Actuality = archivoprocesadodetalleTemplate.Actuality;
            archivoprocesadodetalle.CounterCompany = archivoprocesadodetalleTemplate.CounterCompany;
            archivoprocesadodetalle.Dim1 = archivoprocesadodetalleTemplate.Dim1;
            archivoprocesadodetalle.Dim2 = archivoprocesadodetalleTemplate.Dim2;
            archivoprocesadodetalle.Dim3 = archivoprocesadodetalleTemplate.Dim3;
            archivoprocesadodetalle.ITOpex = archivoprocesadodetalleTemplate.ITOpex;
            archivoprocesadodetalle.TransactionCurrency = archivoprocesadodetalleTemplate.TransactionCurrency;
            archivoprocesadodetalle.Form = archivoprocesadodetalleTemplate.Form;
            archivoprocesadodetalle.AccountName = archivoprocesadodetalleTemplate.AccountName;
            archivoprocesadodetalle.Retrieve = archivoprocesadodetalleTemplate.Retrieve;
            archivoprocesadodetalle.Variance = archivoprocesadodetalleTemplate.Variance;
            if (ModelState.IsValid)
            {
                db.ArchivoProcesadoDetalle.Add(archivoprocesadodetalle);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la cuenta");
                    return View();
                }
                catch (DbUpdateException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la cuenta");
                    return View();
                }
                catch (Exception e)
                {
                    Log.WriteLog(ManejoErrores.ErrorExepcion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la cuenta");
                    return View();
                }
                return RedirectToAction("Index", new { id = archivoprocesadodetalle.ArchivoProcesadoId, tipoArchivo = tipoArchivo });
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

            CuentaCognos cuentaCognos = db.CuentaCognos.FirstOrDefault(cc => cc.Numero == archivoprocesadodetalle.Account);

            if (cuentaCognos != null && cuentaCognos.Anexo.Modificable == false)
            {
                ModelState.AddModelError("Error", "No se puede Editar, debido el Anexo de la cuenta Cognos no permite modificación");
                return View(archivoprocesadodetalle);
            }
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
            int tipoArchivo = (int)TempData["tipoArchivo"];
            HistorialArchivoProcesadoDetalle historial = new HistorialArchivoProcesadoDetalle();
            if (ModelState.IsValid)
            {
                db.Entry(archivoprocesadodetalle).State = EntityState.Modified;
                try
                {
                    historial.Account = archivoprocesadodetalle.Account;
                    historial.ArchivoProcesadoDetalleId = archivoprocesadodetalle.Id;
                    historial.Amount = archivoprocesadodetalle.Amount;
                    historial.TransactionAmount = archivoprocesadodetalle.TransactionAmount;
                    //TODO: implementar cuando se tengal el usuario
                    historial.UsuarioId = 1;
                    historial.FechaModificacion = DateTime.Now;
                    historial.TipoModificacionId = (int)EnumTipoModificacion.Actualización;
                    db.HistorialArchivoProcesadoDetalle.Add(historial);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    ModelState.AddModelError("Error", ManejoErrores.ErrorValidacion(e));
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
                return RedirectToAction("Index", new { id = archivoprocesadodetalle.ArchivoProcesadoId, tipoArchivo = tipoArchivo });
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
