using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using System.Text;
using System.IO;
using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CuentaSAPController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CuentaSAP/
        public ActionResult Index(HttpPostedFileBase file)
        {
            var cuentasap = db.CuentaSAP.Include(c => c.CuentaCognos1).Include(c => c.TipoCuentaSAP1);
            if (file != null && file.ContentLength > 0)
            {
                StringBuilder errores = CargeMasivoCuentaSAP(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores.ToString());
 
                }
            }          
                       
            return View(cuentasap.ToList());
        }

        // Carga masiva de cuentas cognos 
        // return: errores y si no hay devuelve el objeto vacio        
        public StringBuilder CargeMasivoCuentaSAP(HttpPostedFileBase file)
        {
            CuentaSAP cuentaSAP = null;
            int cuentaCognos, tipoCuentaSAP, cuentaCargo, cuentaAbono;
            bool esOpen;
            StringBuilder errores = new StringBuilder();
            
            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = System.Text.Encoding.UTF8.GetString(binData);
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length < 5)
                {
                    errores.AppendLine("No. Registro" + i + " ERROR: lA ESTRUCTURA DEL ARCHIVO NO ES: NUMERO, DESCRIPCION, ANEXO ID");

                }
                if (int.TryParse(dato[2], out cuentaCognos) == false)
                {
                    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA CUENTA COGNOS NO ES NUMERICO");
                }
                if (int.TryParse(dato[3], out tipoCuentaSAP) == false)
                {
                    errores.AppendLine("No. Registro: " + i + " ERROR: EL TIPO DE CUENTA SAP NO ES NUMERICO");
                }
                if (bool.TryParse(dato[4], out esOpen) == false)
                {
                    errores.AppendLine("No. Registro: " + i + " ERROR: EL CAMPO OPEN NO ES BOOL (TRUE FALSE)");
                }
                if (int.TryParse(dato[5], out cuentaCargo) == false)
                {
                    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA CUENTA CARGO NO ES NUMERICO");
                }
                if (int.TryParse(dato[6], out cuentaAbono) == false)
                {
                    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA CUENTA ABONO NO ES NUMERICO");
                }
                if (errores.Length > 0)
                {
                    return errores;

                }
                cuentaSAP = new CuentaSAP()
                {
                    Numero = dato[0],
                    Descripcion = dato[1],
                    CuentaCognos = cuentaCognos,
                    IsActive = true,
                    TipoCuentaSAP = tipoCuentaSAP,
                    EsOpen = esOpen,
                    CuentaCargo = cuentaCargo,
                    CuentaAbono = cuentaAbono,
                };
                if (ModelState.IsValid)
                {
                    db.CuentaSAP.Add(cuentaSAP);
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

        // GET: /CuentaSAP/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            if (cuentasap == null)
            {
                return HttpNotFound();
            }
            return View(cuentasap);
        }

        // GET: /CuentaSAP/Create
        public ActionResult Create()
        {
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre");
            return View();
        }

        // POST: /CuentaSAP/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Numero,Descripcion,CuentaCognos,IsActive,TipoCuentaSAP,EsOpen,CuentaCargo,CuentaAbono")] CuentaSAP cuentasap)
        {
            if (ModelState.IsValid)
            {
                db.CuentaSAP.Add(cuentasap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);

            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            return View(cuentasap);
        }

        // GET: /CuentaSAP/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            if (cuentasap == null)
            {
                return HttpNotFound();
            }
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            return View(cuentasap);
        }

        // POST: /CuentaSAP/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Numero,Descripcion,CuentaCognos,IsActive,TipoCuentaSAP,EsOpen,CuentaCargo,CuentaAbono")] CuentaSAP cuentasap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentasap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            return View(cuentasap);
        }

        // GET: /CuentaSAP/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            if (cuentasap == null)
            {
                return HttpNotFound();
            }
            return View(cuentasap);
        }

        // POST: /CuentaSAP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            db.CuentaSAP.Remove(cuentasap);
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
