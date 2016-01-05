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

                    DAT_Reader datReader = new DAT_Reader();
                    MEXSALCTA[] MEXSALCTA=datReader.StartReading_MEXSALCTA(result);

                    /*
                     * 
                     * 
                     insert tabla archivocarga
                      
                     * [Nombre]  = file name
      ,[Identificador] = MEXSALCTA (balance) o MEX_SALINT  (intercompañias)
      ,[Fecha] = fecha del sistema
      ,[TipoArchivoCarga]= id de la tabla TipoArchivoCarga lo puedo hacer con un enumerable
      ,[Anio_Col3] = public int Anio es del primer registro;
      ,[Mes_Col4] = public int Mes es del primer registro;
      ,[Usuario] = el que este logeado en la app
                     * 
                     * for MEXSALCTA[]
                            insert tabla archivo carga detalle
                         *  ,[ArchivoCarga] = id del que acabe de crear en archivo carga
                         *  if mexsalcta => [CopaniaRelacionada]= null
                     *  exit for
                     *  llamar a un store procedure validar que la info esta bien esta devuelve un obj tabla
                     */
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


                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                  
                }
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

            var cuentaSAP = db.CuentaSAP.Select(cf => cf.CuentaCognos == cuentacognos.Id).First();
            if (cuentaSAP == false)
            {
              
                    ModelState.AddModelError("Error", "Ex: This login failed");
                    return View();               
                //TODO: mostrar error: Primero debe desasignar las cuentas SAP asociadas
            }
            return View(cuentacognos);
        }

        // POST: /CuentaCognos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentaCognos cuentacognos = db.CuentaCognos.Find(id);
            var cuentaSAP = db.CuentaSAP.Select(cf => cf.CuentaCognos == cuentacognos.Id).First();
            if (cuentaSAP)
            {
                ModelState.AddModelError("Error", "Ex: Primero debe desasignar las cuentas SAP asociadas");
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
