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

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CuentaCognosController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CuentaCognos/file cargue masivo
        public ActionResult Index(HttpPostedFileBase file)
        {
            var cuentacognos = db.CuentaCognos.Include(c => c.Anexo);
            if (file != null && file.ContentLength > 0)
            {
                StringBuilder errores = CargeMasivoCuentaCognos(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores.ToString());

                }
            }
            return View(cuentacognos.ToList());
        }

        // Carga masiva de cuentas cognos
        // return: errores y si no hay devuelve el objeto vacio        
        [HttpPost]
        public StringBuilder CargeMasivoCuentaCognos(HttpPostedFileBase file)
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
                if (ModelState.IsValid)
                {
                    db.CuentaCognos.Add(cuentaCognos);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                        return errores;

                    }
                    catch (DbUpdateException e)
                    {
                        errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                        return errores;
                    }
                    catch (Exception e)
                    {
                        errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                        return errores;
                    }
                }
            }
            return errores;
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
            if (ModelState.IsValid)
            {
                db.CuentaCognos.Add(cuentacognos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AnexoId = new SelectList(db.Anexo, "id", "Clave", cuentacognos.AnexoId);
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

            db.CuentaCognos.Remove(cuentacognos);
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
