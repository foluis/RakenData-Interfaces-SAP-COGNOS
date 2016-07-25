﻿using System;
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
using System.Collections.Generic;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "1")]
    public class CuentaSAPController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();
       
        public ActionResult Index(HttpPostedFileBase file)
        {
            try
            {                
                var cuentasap = db.CuentaSAP
                    .OrderBy(cs => cs.Numero)
                    .Include(c => c.CuentaCognos1)
                    .Where(cc => cc.IsActive == true);

                if (file != null && file.ContentLength > 0)
                {
                    string errores = CargeMasivoCuentaSAP(file);
                    if (errores.Length > 0)
                    {
                        ModelState.AddModelError("Error", errores);
                    }
                }

                return View(cuentasap.ToList());
            }
            catch (Exception ex)
            {
                string error =ex.ToString();
                throw;
            }           
        }
     
        public string CargeMasivoCuentaSAP(HttpPostedFileBase file)
        {
            CuentaSAP cuentasap = null;            
            int tipoCuentaSAP = 0;          
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
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length != 4)
                {
                    errores.AppendLine("No. Registro" + (i + 1) + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: numero,descripcion,cuentaCognos,TipoCuentaSAP,esOpen,CuentaCargo,CuentaAbono");
                    continue;
                }
                else
                {
                    string cuentaCognos = dato[2].Replace("\r", string.Empty);

                    CuentaCognos cuentaCognosExiste = db.CuentaCognos.FirstOrDefault(cc => cc.Numero == cuentaCognos);

                    if (cuentaCognosExiste == null)
                    {                        
                        errores.AppendLine($"No. Registro {i + 1} ERROR: LA CLAVE DE LA CUENTA COGNOS ({dato[2]}) NO EXISTE. ");
                        continue;
                    }

                    if (int.TryParse(dato[3], out tipoCuentaSAP) == false)
                    {
                        errores.AppendLine("No. Registro: " + (i + 1) + " ERROR: EL TIPO DE CUENTA SAP NO ES NUMERICO");
                        continue;
                    }                  

                    cuentasap = new CuentaSAP()
                    {
                        Numero = dato[0],
                        Descripcion = dato[1].Length <= 35 ? dato[1].ToUpper() : dato[1].Substring(0, 35).ToUpper(),
                        CuentaCognos = cuentaCognosExiste.Id,
                        IsActive = true,
                        TipoCuentaSAP = tipoCuentaSAP
                    };

                    CuentaSAP cuentaSapExiste = db.CuentaSAP.FirstOrDefault(cc => cc.Numero == cuentasap.Numero);
                    if (cuentaSapExiste == null)
                    {
                        cuentasap.IsActive = true;
                        db.CuentaSAP.Add(cuentasap);
                    }
                    else
                    {
                        cuentaSapExiste.Descripcion = cuentasap.Descripcion;
                        cuentaSapExiste.CuentaCognos = cuentasap.CuentaCognos;
                        cuentaSapExiste.TipoCuentaSAP = cuentasap.TipoCuentaSAP;                     
                        cuentaSapExiste.IsActive = true;
                        db.Entry(cuentaSapExiste).State = EntityState.Modified;
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

        public static class NullableInt
        {
            public static bool TryParse(string text, out int? outValue)
            {
                int parsedValue;
                bool success = int.TryParse(text, out parsedValue);
                outValue = success ? (int?)parsedValue : null;
                return success;
            }
        }

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

        public ActionResult Create()
        {
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos.Where(cc => cc.IsActive == true).OrderBy(cc => cc.Numero), "Id", "Numero");         
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Numero,Descripcion,CuentaCognos,IsActive,TipoCuentaSAP")] CuentaSAP cuentasap)
        {
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);           
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);

            if (ModelState.IsValid)
            {
                CuentaSAP cuentaSapExiste = db.CuentaSAP.FirstOrDefault(cc => cc.Numero == cuentasap.Numero);
                if (cuentaSapExiste == null)
                {
                    cuentasap.Descripcion = cuentasap.Descripcion.ToUpper();                    
                    cuentasap.IsActive = true;
                    db.CuentaSAP.Add(cuentasap);
                }
                else
                {
                    cuentaSapExiste.Descripcion = cuentasap.Descripcion.ToUpper();
                    cuentaSapExiste.CuentaCognos = cuentasap.CuentaCognos;
                    cuentaSapExiste.TipoCuentaSAP = cuentasap.TipoCuentaSAP;                
                    cuentaSapExiste.IsActive = true;
                    db.Entry(cuentaSapExiste).State = EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la cuenta");
                    return View();
                }
                catch (DbUpdateException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la cuenta");
                    return View();
                }
                catch (Exception e)
                {
                    Log.WriteLog(ManejoErrores.ErrorExepcion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la cuenta");
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View(cuentasap);
        }

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
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos.Where(cc => cc.IsActive == true).OrderBy(cc => cc.Numero), "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            //ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCargo);
            //ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaAbono);

            return View(cuentasap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Numero,Descripcion,CuentaCognos,IsActive,TipoCuentaSAP,EsOpen,CuentaCargo,CuentaAbono")] CuentaSAP cuentasap)
        {
            StringBuilder errores = new StringBuilder();
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);           

            if (ModelState.IsValid)
            {
                cuentasap.Descripcion = cuentasap.Descripcion.ToUpper();   
                cuentasap.IsActive = true;
                db.Entry(cuentasap).State = EntityState.Modified;
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
            return View(cuentasap);
        }

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentaSAP cuentasap = db.CuentaSAP.Find(id);
            cuentasap.IsActive = false;
            db.Entry(cuentasap).State = EntityState.Modified;
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
