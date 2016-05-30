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
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "1")]
    public class CompaniaCognosController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CompaniaCognos/
        //[Authorize(Roles = "1")]
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
            StringBuilder errores = new StringBuilder();

            string extension = Path.GetExtension(file.FileName);
            if (extension != ".txt")
            {
                errores.AppendLine("El Archivo debe ser un archivo plano de texto con extención .txt");
                return errores.ToString();
            }

            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = System.Text.Encoding.UTF8.GetString(binData);
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length != 2)
                {
                    errores.AppendLine("No. Registro" + (i + 1) + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: CLAVE, DESCRIPCION");                    
                }
                else
                {
                    string descripcion = dato[1].Replace("\r", string.Empty).ToUpper();
                    descripcion = dato[1].Length <= 35 ? descripcion : descripcion.Substring(0, 35);

                    companiaCognos = new CompaniaCognos()
                    {
                        Clave =  dato[0].Length <= 13 ? dato[0].ToUpper() : dato[0].Substring(0, 13).ToUpper(),
                        Descripcion = descripcion
                    };

                    CompaniaCognos companiaCognosExiste = db.CompaniaCognos.FirstOrDefault(cc => cc.Clave == companiaCognos.Clave);

                    if (companiaCognosExiste == null)
                    {
                        db.CompaniaCognos.Add(companiaCognos);
                    }
                    else
                    {
                        companiaCognosExiste.Descripcion = companiaCognos.Descripcion;
                        db.Entry(companiaCognosExiste).State = EntityState.Modified;
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
                companiacognos.Descripcion = companiacognos.Descripcion.ToUpper();
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
