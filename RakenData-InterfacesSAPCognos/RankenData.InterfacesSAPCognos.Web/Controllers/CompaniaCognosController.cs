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
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CompaniaCognosController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CompaniaCognos/
         [Authorize(Roles = "1")]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string errores = CargeCompaniaCognos(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores);

                }
            }
            return View(db.CompaniaCognos.ToList());
        }

        // Carga masiva de cargue compania cognos
        // return: errores y si no hay devuelve el objeto vacio        
        public string CargeCompaniaCognos(HttpPostedFileBase file)
        {
            CompaniaCognos companiaCognos = null;
            int clave;
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
                    errores.AppendLine("No. Registro" + i + " ERROR: lA ESTRUCTURA DEL ARCHIVO NO ES: CLAVE, DESCRIPCION");

                }
                if (int.TryParse(dato[0], out clave) == false)
                {
                    errores.AppendLine("No. Registro: " + i + " ERROR: LA CLAVE NO ES NUMERICO");
                }

                if (errores.Length > 0)
                {
                    return errores.ToString();

                }
                companiaCognos = new CompaniaCognos()
                {
                    Clave = clave,
                    Descripcion = dato[1]
                };
                if (ModelState.IsValid)
                {
                    db.CompaniaCognos.Add(companiaCognos);
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

        // GET: /CompaniaCognos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaCognos companiacognos = db.CompaniaCognos.Find(id);
            if (companiacognos == null)
            {
                return HttpNotFound();
            }
            return View(companiacognos);
        }

        // GET: /CompaniaCognos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /CompaniaCognos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Clave,Descripcion")] CompaniaCognos companiacognos)
        {
            if (ModelState.IsValid)
            {
                db.CompaniaCognos.Add(companiacognos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(companiacognos);
        }

        // GET: /CompaniaCognos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaCognos companiacognos = db.CompaniaCognos.Find(id);
            if (companiacognos == null)
            {
                return HttpNotFound();
            }
            return View(companiacognos);
        }

        // POST: /CompaniaCognos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Clave,Descripcion")] CompaniaCognos companiacognos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companiacognos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(companiacognos);
        }

        // GET: /CompaniaCognos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaCognos companiacognos = db.CompaniaCognos.Find(id);
            if (companiacognos == null)
            {
                return HttpNotFound();
            }
            return View(companiacognos);
        }

        // POST: /CompaniaCognos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StringBuilder errores = new StringBuilder();
            CompaniaCognos companiacognos = db.CompaniaCognos.Find(id);
            db.CompaniaCognos.Remove(companiacognos);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {                
                ModelState.AddModelError("Error", ManejoErrores.ErrorValidacion(e));
                return View();
            }
            catch (DbUpdateException e)
            {
                errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                ModelState.AddModelError("Error", errores.ToString());
                return View();
            }
            catch (Exception e)
            {
                errores.AppendLine("ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message);
                ModelState.AddModelError("Error", errores.ToString());
                return View();
            }
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
