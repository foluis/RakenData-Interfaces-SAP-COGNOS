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
    public class AnexoController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /Anexo/
         //[Authorize(Roles = "1")]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string errores = CargeAnexo(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores);
                }
            }
            return View(db.Anexo.ToList().Where(a=> a.IsActive == true));
        }

        // Carga masiva de cargue compania RFC
        // return: errores y si no hay devuelve el objeto vacio        
        public string CargeAnexo(HttpPostedFileBase file)
        {
            Anexo anexo = null;
            StringBuilder errores = new StringBuilder();

            string extension = Path.GetExtension(file.FileName);
            if (extension != ".txt")
            {
                errores.AppendLine("El Archivo debe ser un archivo plano de texto con extencion .txt");
                return errores.ToString();
            }

            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = System.Text.Encoding.UTF8.GetString(binData);
            bool modificable = false;
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length < 2)
                {
                    errores.AppendLine("No. Registro" + i + " ERROR: lA ESTRUCTURA DEL ARCHIVO NO ES: CLAVE, DESCRIPCION");
                }

                if (bool.TryParse(dato[2], out modificable) == false)
                {
                    errores.AppendLine("No. Registro" + i + " ERROR: MODIFICABLE DEBE TENER EL VALOR (TRUE O FALSE)");
         
                }
                if (errores.Length > 0)
                {
                    return errores.ToString();

                }                

                anexo = new Anexo()
                {
                    Clave = dato[0],
                    Descripcion = dato[1].Length <= 35 ? dato[1].ToUpper(): dato[1].Substring(0,35).ToUpper(),
                    Modificable = modificable
                   
                };
                if (ModelState.IsValid)
                {
                    Anexo anexoExiste = db.Anexo.FirstOrDefault(cc => cc.Clave == anexo.Clave);
                    if (anexoExiste == null)
                    {
                        anexo.IsActive = true;
                        db.Anexo.Add(anexo);
                    }
                    else
                    {
                        anexoExiste.Descripcion = anexo.Descripcion;
                        anexoExiste.Modificable = anexo.Modificable;
                        anexoExiste.IsActive = true;
                        db.Entry(anexoExiste).State = EntityState.Modified;
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
        public ActionResult Create([Bind(Include = "id,Clave,Descripcion,IsActive,Modificable")] Anexo anexo)
        {
            StringBuilder errores = new StringBuilder();
            if (ModelState.IsValid)
            {
                Anexo anexoExiste = db.Anexo.FirstOrDefault(cc => cc.Clave == anexo.Clave);
                if (anexoExiste == null)
                {
                    anexo.IsActive = true;
                    db.Anexo.Add(anexo);
                }
                else
                {
                    anexoExiste.Descripcion = anexo.Descripcion;
                    anexoExiste.Modificable = anexo.Modificable;
                    anexoExiste.IsActive = true;
                    db.Entry(anexoExiste).State = EntityState.Modified;
                }                

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear el anexo");
                    return View();
                }
                catch (DbUpdateException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear el anexo");
                    return View();
                }
                catch (Exception e)
                {
                    Log.WriteLog(ManejoErrores.ErrorExepcion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear el anexo");
                    return View();
                }
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
        public ActionResult Edit([Bind(Include = "id,Clave,Descripcion,IsActive,Modificable")] Anexo anexo)
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
