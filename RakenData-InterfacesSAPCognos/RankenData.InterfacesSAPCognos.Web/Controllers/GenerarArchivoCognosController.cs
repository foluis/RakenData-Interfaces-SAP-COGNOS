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
   
            return View(archivo);
        }

        [HttpPost, ActionName("GenerarArchivo")]
        [ValidateAntiForgeryToken]
        public ActionResult BotonGenerarArchivo(enArchivoCargaCongnos archivoCargaCongnos)
        {
            ViewBag.LstIdCompaniasCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            List<int?> id = db.CreateArchivoBalance(archivoCargaCongnos.LstIdCompaniasCognos.First().ToString(), archivoCargaCongnos.Periodo, archivoCargaCongnos.Anio, archivoCargaCongnos.TipoArchivo.ToString(), 1).ToList();
          
            return RedirectToAction("Index", "ArchivoProcesadoDetalle", new { id = id.FirstOrDefault(), tipoArchivo = archivoCargaCongnos.TipoArchivo });
        }       
    }
}
