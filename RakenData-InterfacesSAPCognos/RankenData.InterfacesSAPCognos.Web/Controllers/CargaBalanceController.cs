using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CargaBalanceController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // Cargue balance / resultados
         //[Authorize(Roles = "2")]
        [HttpPost]
        public string CargarBalance()
        {
            string errores = string.Empty;
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    BinaryReader b = new BinaryReader(file.InputStream);
                    byte[] binData = b.ReadBytes((int)file.InputStream.Length);
                    string result = System.Text.Encoding.UTF8.GetString(binData);
                    errores = CargarArchivo.CargarArchivoBD(file.FileName, result, EnumTipoArchivoCarga.Balance);
                }
            }
            return errores;
        }

        //
        // GET: /CargaBalance/
          //[Authorize(Roles = "2")]
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /EliminarBalance/Default1
        public ActionResult EliminarBalance()
        {
            ViewBag.Identificador = new SelectList(db.ArchivoCarga.Where(aa => aa.TipoArchivoCarga == (int)EnumTipoArchivoCarga.Balance), "Id", "Identificador");
            return View();
        }

        [HttpPost, ActionName("EliminarBalance")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarBalanceConfirmed(string Identificador)
        {
            int idArchivo;
            if (int.TryParse(Identificador, out idArchivo))
            {
                db.EliminarArchivoCarga(idArchivo);
                ModelState.AddModelError("Error", "La Eliminación fue Exitosa");
            }
            else 
            {
                ModelState.AddModelError("Error","El id del archivo es invalido");
            }
           
            ViewBag.Identificador = new SelectList(db.ArchivoCarga, "Id", "Identificador");
            return View();
        }
    }
}
