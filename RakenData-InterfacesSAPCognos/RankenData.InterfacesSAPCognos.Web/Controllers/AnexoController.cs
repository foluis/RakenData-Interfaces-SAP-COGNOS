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
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class AnexoController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /Anexo/
        public ActionResult Index(HttpPostedFileBase file)
        {           
            if (file != null && file.ContentLength > 0)
            {
                StringBuilder errores = CargeAnexo(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores.ToString());
                }
            }
            return View(db.Anexo.ToList().Where(a=> a.IsActive == true));
        }

        // Carga masiva de cargue compania RFC
        // return: errores y si no hay devuelve el objeto vacio        
        public StringBuilder CargeAnexo(HttpPostedFileBase file)
        {
            Anexo anexo = null;
            StringBuilder errores = new StringBuilder();

            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = System.Text.Encoding.UTF8.GetString(binData);
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length < 1)
                {
                    errores.AppendLine("No. Registro" + i + " ERROR: lA ESTRUCTURA DEL ARCHIVO NO ES: CLAVE, DESCRIPCION");
                }
                if (errores.Length > 0)
                {
                    return errores;

                }
                anexo = new Anexo()
                {
                    Clave = dato[0],
                    Descripcion = dato[1],
                   
                };
                if (ModelState.IsValid)
                {
                    db.Anexo.Add(anexo);
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

        // GET: /Anexo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Anexo anexo = db.Anexo.Find(id);
            if (anexo == null)
            {
                return HttpNotFound();
            }
            return View(anexo);
        }

        // GET: /Anexo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Anexo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Clave,Descripcion,IsActive")] Anexo anexo)
        {
            if (ModelState.IsValid)
            {
                db.Anexo.Add(anexo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(anexo);
        }

        // GET: /Anexo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Anexo anexo = db.Anexo.Find(id);
            if (anexo == null)
            {
                return HttpNotFound();
            }
            return View(anexo);
        }

        // POST: /Anexo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Clave,Descripcion,IsActive")] Anexo anexo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(anexo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(anexo);
        }

        // GET: /Anexo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Anexo anexo = db.Anexo.Find(id);
            if (anexo == null)
            {
                return HttpNotFound();
            }
            return View(anexo);
        }

        // POST: /Anexo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Anexo anexo = db.Anexo.Find(id);
            db.Entry(anexo).State = EntityState.Modified;
            anexo.IsActive = false;           
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
