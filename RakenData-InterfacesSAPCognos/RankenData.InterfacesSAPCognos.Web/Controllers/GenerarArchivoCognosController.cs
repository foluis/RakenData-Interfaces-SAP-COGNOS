using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "3")]
    public class GenerarArchivoCognosController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();
    
        public ActionResult GenerarArchivo()
        {
            enArchivoCargaCongnos archivo = new enArchivoCargaCongnos();
            ViewBag.LstIdCompaniasCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCreacion, "Id", "Nombre");

            List<Redondear> lstRedondeo = new List<Redondear>();
            lstRedondeo.Add(new Redondear() { Id = 1, Nombre = "Sin redondeo" });
            lstRedondeo.Add(new Redondear() { Id = 2, Nombre = "Dividido 1.000" });
            lstRedondeo.Add(new Redondear() { Id = 3, Nombre = "Dividido 1.000.000" });
            ViewBag.Redondeos = new SelectList(lstRedondeo, "Id", "Nombre");

            return View(archivo);
        }

        [HttpPost, ActionName("GenerarArchivo")]
        [ValidateAntiForgeryToken]
        public ActionResult BotonGenerarArchivo(enArchivoCargaCongnos archivoCargaCongnos)
        {
            ViewBag.LstIdCompaniasCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCreacion, "Id", "Nombre");

            List<Redondear> lstRedondeo = new List<Redondear>();
            lstRedondeo.Add(new Redondear() { Id = 1, Nombre = "Sin redondeo" });
            lstRedondeo.Add(new Redondear() { Id = 2, Nombre = "Dividido 1.000" });
            lstRedondeo.Add(new Redondear() { Id = 3, Nombre = "Dividido 1.000.000" });
            ViewBag.Redondeos = new SelectList(lstRedondeo, "Id", "Nombre");

            List<int?> id;

            string paramCompaniaCognos = string.Empty;
            int paramPeriodo = archivoCargaCongnos.Periodo;
            int paramAnio = archivoCargaCongnos.Anio;
            string paramTipoArchivo = string.Empty;
            int paramRedondeos = archivoCargaCongnos.Redondeos;
            string currentUser = User.Identity.Name;
            int paramUser = 1;
            string storeProcedure = string.Empty;

            if (!string.IsNullOrEmpty(currentUser))
            {
                User user = db.User.FirstOrDefault(a => a.Username == currentUser);
                if(user!= null)
                {
                    paramUser = user.Id;
                }                
            }

            try
            {
                foreach (var companiaCognos in archivoCargaCongnos.LstIdCompaniasCognos)
                {
                    paramCompaniaCognos = companiaCognos.ToString();                  

                    foreach (var tipoArchivo in archivoCargaCongnos.TipoArchivo)
                    {
                        paramTipoArchivo = tipoArchivo.ToString();

                        switch (tipoArchivo)
                        {
                            case 1:                                
                                storeProcedure = "CreateArchivoBalance";
                                id = db.CreateArchivoBalance(paramCompaniaCognos, paramPeriodo, paramAnio, paramTipoArchivo, paramUser, paramRedondeos).ToList();
                                //id = db.CreateArchivoBalance(companiaCognos.ToString(), archivoCargaCongnos.Periodo, archivoCargaCongnos.Anio, tipoArchivo.ToString(), 1, archivoCargaCongnos.Redondeos).ToList();
                                if (id != null && id.Count() > 0 && id.First().Value == -1)
                                {
                                    ModelState.AddModelError("Error", "No hay datos para procesar archivo");
                                    return View();
                                }
                                break;
                            case 2:
                                storeProcedure = "CreateArchivoResultados";
                                id = db.CreateArchivoResultados(paramCompaniaCognos, paramPeriodo, paramAnio, paramTipoArchivo, paramUser, paramRedondeos).ToList();
                                //id = db.CreateArchivoResultados(companiaCognos.ToString(), archivoCargaCongnos.Periodo, archivoCargaCongnos.Anio, tipoArchivo.ToString(), 1, archivoCargaCongnos.Redondeos).ToList();
                                if (id != null && id.Count() > 0 && id.First().Value == -1)
                                {
                                    ModelState.AddModelError("Error", "No hay datos para procesar archivo");
                                    return View();
                                }
                                break;
                            case 3:
                                storeProcedure = "CreateArchivoIntercompanias";
                                id = db.CreateArchivoIntercompanias(paramCompaniaCognos, paramPeriodo, paramAnio, paramTipoArchivo, paramUser, paramRedondeos).ToList();
                                //id = db.CreateArchivoIntercompanias(companiaCognos.ToString(), archivoCargaCongnos.Periodo, archivoCargaCongnos.Anio, tipoArchivo.ToString(), 1, archivoCargaCongnos.Redondeos).ToList();
                                if (id != null && id.Count() > 0 && id.First().Value == -1)
                                {
                                    ModelState.AddModelError("Error", "No hay datos para procesar archivo");
                                    return View();
                                }
                                break;
                            default:
                                break;  
                        }
                    }
                }

            }
            catch (DbEntityValidationException e)
            {
                Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                ModelState.AddModelError("Error", "No se pudo generar el archivo");
                return View();
            }
            catch (DbUpdateException e)
            {
                Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                ModelState.AddModelError("Error", "No se pudo generar el archivo");
                return View();
            }
            catch (Exception e)
            {                
                string parametros = $"Parametros: storeProcedure: {storeProcedure}, paramCompaniaCognos:{paramCompaniaCognos} paramPeriodo: {paramPeriodo}, paramAnio: {paramAnio}, paramTipoArchivo: {paramTipoArchivo}, paramUser: {paramUser}, paramRedondeos: {paramRedondeos}";
                
                Log.WriteLog(ManejoErrores.ErrorExepcion(parametros,e), EnumTypeLog.Error, true);
                ModelState.AddModelError("Error", "No se pudo generar el archivo");
                return View();
            }

            return RedirectToAction("Index", "ArchivoProcesado");
        }
    }
}
