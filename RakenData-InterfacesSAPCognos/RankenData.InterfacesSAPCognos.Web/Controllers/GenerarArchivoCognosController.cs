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
         //[Authorize(Roles = "3")]
        public ActionResult GenerarArchivo()
        {
            enArchivoCargaCongnos archivo = new enArchivoCargaCongnos();
            ViewBag.LstIdCompaniasCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCreacion, "Id", "Nombre");

            List<Redondear> lstRedondeo= new List<Redondear>();
           lstRedondeo.Add(new Redondear(){ Id= 1, Nombre="Sin redondeo"});
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
            List<int?> id;

            foreach (var companiaCognos in archivoCargaCongnos.LstIdCompaniasCognos)
            {
                foreach (var tipoArchivo in archivoCargaCongnos.TipoArchivo)
                {
                    switch (tipoArchivo)
                    {
                        case 1:
                            //todo: id de usuario quemado para los 3
                            id = db.CreateArchivoBalance(companiaCognos.ToString(), archivoCargaCongnos.Periodo, archivoCargaCongnos.Anio, tipoArchivo.ToString(), 1, archivoCargaCongnos.Redondeos).ToList();
                            if (id != null && id.Count() > 0 && id.First().Value == -1)
                            {
                                ModelState.AddModelError("Error", "No hay datos para procesar archivo");
                                return View();
                            }
                            break;
                        case 2:
                            id = db.CreateArchivoResultados(companiaCognos.ToString(), archivoCargaCongnos.Periodo, archivoCargaCongnos.Anio, tipoArchivo.ToString(), 1, archivoCargaCongnos.Redondeos).ToList();
                            if (id != null && id.Count() > 0 && id.First().Value == -1)
                            {
                                ModelState.AddModelError("Error", "No hay datos para procesar archivo");
                                return View();
                            }
                            break;
                        case 3:
                            id = db.CreateArchivoIntercompanias(companiaCognos.ToString(), archivoCargaCongnos.Periodo, archivoCargaCongnos.Anio, tipoArchivo.ToString(), 1, archivoCargaCongnos.Redondeos).ToList();
                            if (id != null && id.Count() >0 && id.First().Value == -1)
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


            return RedirectToAction("Index", "ArchivoProcesado");
        }       
    }
}
