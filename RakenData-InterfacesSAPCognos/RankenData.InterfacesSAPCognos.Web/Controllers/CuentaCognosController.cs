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

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CuentaCognosController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();


        [HttpPost]
        public ActionResult Upload()
        {
            CuentaCognos cuentaCognos = null;
            int anexoid;
            int i = 0;
            StringBuilder errores = new StringBuilder();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    BinaryReader b = new BinaryReader(file.InputStream);
                    byte[] binData = b.ReadBytes((int)file.InputStream.Length);
                    string result = System.Text.Encoding.UTF8.GetString(binData);
                    var records = result.Split('\n');
                    foreach (var record in records)
                    {
                        i++;
                        var dato = record.Split(',');
                        if (dato.Length < 3)
                        {
                            errores.AppendLine("No. Registro" + i + "ERROR: lA ESTRUCTURA DEL ARCHIVO NO ES: NUMERO - DESCRIPCION- ANEXO ID");
                            //TODO: IMPLEMENTAR EL ERROR
                            // ERROR lA ESTRUCTURA DEL ARCHIVO NO ES: NUMERO - DESCRIPCION- ANEXO ID
                        }
                        if (int.TryParse(dato[2], out anexoid) == false)
                        {
                            errores.AppendLine("No. Registro" + i + "ERROR: EL ID DEL ANEXO NO ES NUMERICO");

                            //TODO: IMPLEMENTAR EL ERROR
                            // ERROR EL ID DEL ANEXO NO ES NUMERICO
                        }
                        cuentaCognos = new CuentaCognos() { Numero = dato[0], Descripcion = dato[1], AnexoId = anexoid, IsActive = true };
                        if (ModelState.IsValid)
                        {
                            db.CuentaCognos.Add(cuentaCognos);
                            db.SaveChanges();
                        }
                    }
                }
            }
            if (errores.Length > 0)
            {
                //prueba de sincronizacion 2

                //prueba 888

                //prueba 3

                //TODO: IMPLEMENTAR EL ERROR
                
                // ERROR EL ID DEL ANEXO NO ES NUMERICO
            }
            return RedirectToAction("Index");
        }
        // GET: /CuentaCognos/
        public ActionResult Index()
        {
            var cuentacognos = db.CuentaCognos.Include(c => c.Anexo);
            return View(cuentacognos.ToList());
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
