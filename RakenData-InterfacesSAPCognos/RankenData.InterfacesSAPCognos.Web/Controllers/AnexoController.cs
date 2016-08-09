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
using PagedList;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "1")]
    public class AnexoController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        public ActionResult Index(HttpPostedFileBase file, string searchBy, string search = "", int pageIndex = 1, int pageSize = 5)
        {
            if (file != null && file.ContentLength > 0)
            {
                string errores = CargeAnexo(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores);
                }
            }

            //IPagedList<Anexo> anexos;

            //if (searchBy == "Clave")
            //{
            //    anexos = db.Anexo.
            //        Where(a => a.IsActive == true && a.Clave.Contains(search)).
            //        OrderBy(a => a.Clave).ToPagedList(pageIndex, pageSize);
            //}
            //else
            //{
            //    anexos = db.Anexo.
            //        Where(a => a.IsActive == true && a.Descripcion.Contains(search)).
            //        OrderBy(a => a.Clave).ToPagedList(pageIndex, pageSize);
            //}
            /*******************/
            //anexos = db.Anexo.
            //        Where(a => a.IsActive == true).
            //        OrderBy(a => a.Clave).ToPagedList(pageIndex, pageSize);

            //if (searchBy == "Clave")
            //{
            //    return View(anexos.Where(a => a.Clave.Contains(search)));
            //}
            //else
            //{
            //    return View(anexos.Where(a => a.Descripcion.Contains(search)));
            //}

            IQueryable<Anexo> ane;

            var anexos = db.Anexo.
                    Where(a => a.IsActive == true).
                    OrderBy(a => a.Clave);

            if (searchBy == "Clave")
            {
                ane = anexos.Where(a => a.Clave.Contains(search));
            }
            else
            {
                ane = anexos.Where(a => a.Descripcion.Contains(search));
            }

            return View(ane.ToPagedList(pageIndex, pageSize));
        }

        public string CargeAnexo(HttpPostedFileBase file)
        {
            Anexo anexo = null;
            StringBuilder errores = new StringBuilder();

            string extension = Path.GetExtension(file.FileName);
            if (extension.ToLower() != ".txt")
            {
                errores.AppendLine("El Archivo debe ser un archivo plano de texto con extención .txt");
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
                    Descripcion = dato[1].Length <= 35 ? dato[1].ToUpper() : dato[1].Substring(0, 35).ToUpper(),
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

        public ActionResult Create()
        {
            return View();
        }

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
                    anexo.Clave = anexo.Clave.ToUpper();
                    anexo.Descripcion = anexo.Descripcion.ToUpper();
                    anexo.IsActive = true;
                    db.Anexo.Add(anexo);
                }
                else
                {
                    anexoExiste.Clave = anexo.Clave.ToUpper();
                    anexoExiste.Descripcion = anexo.Descripcion.ToUpper();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Clave,Descripcion,IsActive,Modificable")] Anexo anexo)
        {
            if (ModelState.IsValid)
            {
                anexo.Clave = anexo.Clave.ToUpper();
                anexo.Descripcion = anexo.Descripcion.ToUpper();
                db.Entry(anexo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(anexo);
        }

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Anexo anexo = db.Anexo.Find(id);
            var cuentaCognos = db.CuentaCognos.Where(cc => cc.AnexoId == id && cc.IsActive == true).ToList();
            if (cuentaCognos.Count > 0)
            {
                ModelState.AddModelError("Error", "Primero debe desasignar las cuentas cognos asociadas a este anexo");
                return View(anexo);
            }

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
