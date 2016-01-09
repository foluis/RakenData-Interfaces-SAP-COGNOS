using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using System.Web;
using System.Text;
using System.IO;
using Ranken.ISC.FileManager.ReadFiles;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CompaniaRFCController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CompaniaRFC/
        public ActionResult Index(HttpPostedFileBase file)
        {
            var companiarfc = db.CompaniaRFC.Include(c => c.CompaniaCognos1);
            if (file != null && file.ContentLength > 0)
            {
                string errores = CargeCompaniaRFC(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores);

                }
            }     
            return View(companiarfc.ToList());
        }

        // Carga masiva de cargue compania RFC
        // return: errores y si no hay devuelve el objeto vacio        
        public string CargeCompaniaRFC(HttpPostedFileBase file)
        {
            CompaniaRFC companiaRFC = null;
            int companiaCognos;
            StringBuilder errores = new StringBuilder();

            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = System.Text.Encoding.UTF8.GetString(binData);
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length < 2)
                {
                    errores.AppendLine("No. Registro" + i + " ERROR: lA ESTRUCTURA DEL ARCHIVO NO ES: RFC,Descripcion,IdCompaniaCognos");

                }
                if (int.TryParse(dato[2], out companiaCognos) == false)
                {
                    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA COMPANIA COGNOS NO ES NUMERICO");
                }

                if (errores.Length > 0)
                {
                    return errores.ToString();

                }
                companiaRFC = new CompaniaRFC()
                {
                    RFC=dato[0],
                    Descripcion= dato[1],
                    CompaniaCognos = companiaCognos
                };
                if (ModelState.IsValid)
                {
                    db.CompaniaRFC.Add(companiaRFC);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {                      
                        return ManejoErrores.ErrorValidacion(e);
                    }
                    catch (DbUpdateException e)
                    {
                        errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                        return errores.ToString();
                    }
                    catch (Exception e)
                    {
                        errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                        return errores.ToString();
                    }
                }
            }
            return errores.ToString();
        }

        // GET: /CompaniaRFC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Create
        public ActionResult Create()
        {
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            return View();
        }

        // POST: /CompaniaRFC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,RFC,Descripcion,CompaniaCognos")] CompaniaRFC companiarfc)
        {
            if (ModelState.IsValid)
            {
                var existCompania = db.CompaniaRFC.Select(crfc => crfc.CompaniaCognos == companiarfc.CompaniaCognos).FirstOrDefault();
                if (existCompania)
                {
                    ModelState.AddModelError("Error", "Ex: La compañia Cognos, ya se encuentra asociada a otra vuenta RFC");
                    ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
                    return View(companiarfc);
                }

                db.CompaniaRFC.Add(companiarfc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            return View(companiarfc);
        }

        // POST: /CompaniaRFC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,RFC,Descripcion,CompaniaCognos")] CompaniaRFC companiarfc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companiarfc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            return View(companiarfc);
        }

        // POST: /CompaniaRFC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            db.CompaniaRFC.Remove(companiarfc);
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
