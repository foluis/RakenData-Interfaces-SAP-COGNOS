using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using System.IO;
using System.Text;
using Ranken.ISC.FileManager.ReadFiles;
using System.Data.Entity.Infrastructure;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using System.Data.Entity.Validation;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CuentaCognosController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CuentaCognos/file cargue masivo
        //[Authorize(Roles="1")]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var cuentacognos = db.CuentaCognos.Include(c => c.Anexo).Where(cc => cc.IsActive == true);
            if (file != null && file.ContentLength > 0)
            {
                string errores = CargeMasivoCuentaCognos(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores);

                }
            }
            return View(cuentacognos.ToList());
        }

        // Carga masiva de cuentas cognos
        // return: errores y si no hay devuelve el objeto vacio        
        [HttpPost]
        public string CargeMasivoCuentaCognos(HttpPostedFileBase file)
        {
            CuentaCognos cuentaCognos = null;
            int anexoid;

            StringBuilder errores = new StringBuilder();

            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = System.Text.Encoding.UTF8.GetString(binData);
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length < 3)
                {
                    errores.AppendLine("No. Registro " + i + " ERROR: lA ESTRUCTURA DEL ARCHIVO NO ES: NUMERO, DESCRIPCION, ANEXO ID");

                }
                if (int.TryParse(dato[2], out anexoid) == false)
                {
                    errores.AppendLine("No. Registro " + i + " ERROR: EL ID DEL ANEXO NO ES NUMERICO");
                }
                cuentaCognos = new CuentaCognos()
                {
                    Numero = dato[0],
                    Descripcion = dato[1],
                    AnexoId = anexoid,
                    IsActive = true
                };

                CuentaCognos cuentaExiste = db.CuentaCognos.FirstOrDefault(cc => cc.Numero == cuentaCognos.Numero);

                if (cuentaExiste == null)
                {
                    cuentaExiste.IsActive = true;
                    db.CuentaCognos.Add(cuentaExiste);
                }
                else
                {
                    cuentaExiste.Descripcion = cuentaCognos.Descripcion;
                    cuentaExiste.AnexoId = cuentaCognos.AnexoId;
                    cuentaExiste.IsActive = true;
                    db.Entry(cuentaExiste).State = EntityState.Modified;
                }


                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                    return "No se pudo cargar el archivo";
                }
                catch (DbUpdateException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                    return "No se pudo cargar el archivo";
                }
                catch (Exception e)
                {
                    Log.WriteLog(ManejoErrores.ErrorExepcion(e), EnumTypeLog.Error, true);
                    return "No se pudo cargar el archivo";
                }

            }
            return errores.ToString();
        }


        // GET: /CuentaCognos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaCognos cuentacognos = db.CuentaCognos.Find(id);
            if (cuentacognos == null)
            {
                return HttpNotFound();
            }
            return View(cuentacognos);
        }

        // GET: /CuentaCognos/Create
        public ActionResult Create()
        {
            ViewBag.AnexoId = new SelectList(db.Anexo, "id", "Clave");
            return View();
        }

        // POST: /CuentaCognos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Numero,Descripcion,AnexoId,IsActive")] CuentaCognos cuentacognos)
        {
            ViewBag.AnexoId = new SelectList(db.Anexo, "id", "Clave", cuentacognos.AnexoId);
            if (ModelState.IsValid)
            {
                CuentaCognos cuentaExiste = db.CuentaCognos.FirstOrDefault(cc => cc.Numero == cuentacognos.Numero);
                if (cuentaExiste == null)
                {
                    cuentacognos.IsActive = true;
                    db.CuentaCognos.Add(cuentacognos);
                }
                else
                {
                    cuentaExiste.Descripcion = cuentacognos.Descripcion;
                    cuentaExiste.AnexoId = cuentacognos.AnexoId;
                    cuentaExiste.IsActive = true;
                    db.Entry(cuentaExiste).State = EntityState.Modified;
                }

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
                return RedirectToAction("Index");

            }

           
            return View(cuentacognos);
        }

        // GET: /CuentaCognos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaCognos cuentacognos = db.CuentaCognos.Find(id);

            if (cuentacognos == null)
            {
                return HttpNotFound();
            }



            ViewBag.AnexoId = new SelectList(db.Anexo, "id", "Clave", cuentacognos.AnexoId);
            return View(cuentacognos);
        }

        // POST: /CuentaCognos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Numero,Descripcion,AnexoId,IsActive")] CuentaCognos cuentacognos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentacognos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AnexoId = new SelectList(db.Anexo, "id", "Clave", cuentacognos.AnexoId);
            return View(cuentacognos);
        }

        // GET: /CuentaCognos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaCognos cuentacognos = db.CuentaCognos.Find(id);
            if (cuentacognos == null)
            {
                return HttpNotFound();
            }

            return View(cuentacognos);
        }

        // POST: /CuentaCognos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentaCognos cuentacognos = db.CuentaCognos.Find(id);
            var cuentaSAP = db.CuentaSAP.Select(cf => cf.CuentaCognos == cuentacognos.Id).FirstOrDefault();
            if (cuentaSAP)
            {
                ModelState.AddModelError("Error: ", "Primero debe desasignar las cuentas SAP asociadas");
                return View();
            }

            cuentacognos.IsActive = false;
            db.Entry(cuentacognos).State = EntityState.Modified;
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
