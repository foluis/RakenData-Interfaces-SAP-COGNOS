﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.IO;
using Ranken.ISC.FileManager.ReadFiles;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CuentaRelacionadaController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: CuentaRelacionada
        public ActionResult Index(HttpPostedFileBase file)
        {
            var cuentaRelacionada = db.CuentaRelacionada
                .Include(comCog => comCog.CompaniaCognos).Where(comCog => comCog.IsActive == true)
                .Include(cs => cs.CuentaSAP).Where(cs => cs.IsActive == true);

            if (file != null && file.ContentLength > 0)
            {
                string errores = CargueMasivoCuentaRelacionada(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores);                    
                }
            }

            return View(cuentaRelacionada.ToList());
        }

        public string CargueMasivoCuentaRelacionada(HttpPostedFileBase file)
        {
            CuentaRelacionada cuentaR = null;
            CuentaSAP cuentaSAPExiste = null;
            CompaniaCognos companiaCognosExistente = null;
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
                if (dato.Length != 3)
                {
                    errores.AppendLine("No. Registro" + (i + 1) + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: Cuenta SAP,Cta. Relacionada,Sociedad Cognos");
                    continue;
                }
                else
                {
                    string numeroCuentaSAP = dato[0].Replace("\r", string.Empty).Trim();
                    string numeroCuentaRelacionada = dato[1].Replace("\r", string.Empty).Trim();
                    string numeroSociedadCognos = dato[2].Replace("\r", string.Empty).Trim();
                    int claveSociedadCognos;
                    bool conversion = int.TryParse(numeroSociedadCognos, out claveSociedadCognos);

                    if (!conversion)
                    {
                        errores.AppendLine($"No. Registro {i + 1} ERROR: EL NUMERO ({dato[2]}) DE LA SOCIEDAD COGNOS NO ES NUMERICO.");
                        continue;
                    }

                    CuentaRelacionada cuentaRelacionadaExistente = db.CuentaRelacionada.FirstOrDefault(cc => cc.NumeroCuentaRelacionada == numeroCuentaRelacionada);

                    if (cuentaRelacionadaExistente == null)
                    {
                        cuentaSAPExiste = db.CuentaSAP.FirstOrDefault(cc => cc.Numero == numeroCuentaSAP);                        

                        if(cuentaSAPExiste == null)
                        {
                            errores.AppendLine($"No. Registro {i + 1} ERROR: LA CUENTA SAP CON EL NUMERO ({dato[2]}) NO EXISTE. ");
                            continue;
                        }

                        companiaCognosExistente = db.CompaniaCognos.FirstOrDefault(cc => cc.Clave == claveSociedadCognos);

                        if (companiaCognosExistente == null)
                        {
                            errores.AppendLine($"No. Registro {i + 1} ERROR: LA COMPAÑIA COGNOS CON EL NUMERO ({dato[2]}) NO EXISTE. ");
                            continue;
                        }

                        cuentaR = new CuentaRelacionada()
                        {
                            CompaniaCognos = companiaCognosExistente,
                            CuentaSAP = cuentaSAPExiste,
                            NumeroCuentaRelacionada = numeroCuentaRelacionada,
                            IsActive = true,
                        };

                        db.CuentaRelacionada.Add(cuentaR);
                    }
                    else
                    {
                        cuentaSAPExiste = db.CuentaSAP.FirstOrDefault(cc => cc.Numero == numeroCuentaSAP);

                        if (cuentaSAPExiste == null)
                        {
                            errores.AppendLine($"No. Registro {i + 1} ERROR: LA CUENTA SAP CON EL NUMERO ({dato[2]}) NO EXISTE. ");
                            continue;
                        }

                        companiaCognosExistente = db.CompaniaCognos.FirstOrDefault(cc => cc.Clave == claveSociedadCognos);

                        if (companiaCognosExistente == null)
                        {
                            errores.AppendLine($"No. Registro {i + 1} ERROR: LA COMPAÑIA COGNOS CON EL NUMERO ({dato[2]}) NO EXISTE. ");
                            continue;
                        }

                        cuentaRelacionadaExistente.CompaniaCognos = companiaCognosExistente;
                        cuentaRelacionadaExistente.CuentaSAP = cuentaSAPExiste;
                        cuentaRelacionadaExistente.NumeroCuentaRelacionada = numeroCuentaRelacionada;
                        cuentaRelacionadaExistente.IsActive = true;
                        db.Entry(cuentaRelacionadaExistente).State = EntityState.Modified;
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

        // GET: CuentaRelacionada/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaRelacionada cuentaRelacionada = db.CuentaRelacionada.Find(id);
            if (cuentaRelacionada == null)
            {
                return HttpNotFound();
            }
            return View(cuentaRelacionada);
        }

        // GET: CuentaRelacionada/Create
        public ActionResult Create()
        {
            ViewBag.SociedadCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            ViewBag.CuentaSAPId = new SelectList(db.CuentaSAP, "Id", "Numero");
            return View();
        }

        // POST: CuentaRelacionada/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NumeroCuentaRelacionada,CuentaSAPId,SociedadCognosId,IsActive")] CuentaRelacionada cuentaRelacionada)
        {
            if (ModelState.IsValid)
            {
                CuentaRelacionada cuentaRelacionadaExistente = db.CuentaRelacionada.FirstOrDefault(cc => cc.NumeroCuentaRelacionada == cuentaRelacionada.NumeroCuentaRelacionada);
                if (cuentaRelacionadaExistente == null)
                {
                    cuentaRelacionada.IsActive = true;
                    db.CuentaRelacionada.Add(cuentaRelacionada);
                }
                else
                {
                    cuentaRelacionadaExistente.NumeroCuentaRelacionada = cuentaRelacionada.NumeroCuentaRelacionada;
                    cuentaRelacionadaExistente.SociedadCognosId = cuentaRelacionada.SociedadCognosId;
                    cuentaRelacionadaExistente.CuentaSAPId = cuentaRelacionada.CuentaSAPId;
                    cuentaRelacionadaExistente.IsActive = true;
                    db.Entry(cuentaRelacionadaExistente).State = EntityState.Modified;
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

            ViewBag.SociedadCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion", cuentaRelacionada.SociedadCognosId);
            ViewBag.CuentaSAPId = new SelectList(db.CuentaSAP, "Id", "Numero", cuentaRelacionada.CuentaSAPId);
            return View(cuentaRelacionada);
        }

        // GET: CuentaRelacionada/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaRelacionada cuentaRelacionada = db.CuentaRelacionada.Find(id);
            if (cuentaRelacionada == null)
            {
                return HttpNotFound();
            }
            ViewBag.SociedadCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion", cuentaRelacionada.SociedadCognosId);
            ViewBag.CuentaSAPId = new SelectList(db.CuentaSAP, "Id", "Numero", cuentaRelacionada.CuentaSAPId);
            return View(cuentaRelacionada);
        }

        // POST: CuentaRelacionada/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NumeroCuentaRelacionada,CuentaSAPId,SociedadCognosId,IsActive")] CuentaRelacionada cuentaRelacionada)
        {
            StringBuilder errores = new StringBuilder();
            if (ModelState.IsValid)
            {
                cuentaRelacionada.IsActive = true;
                db.Entry(cuentaRelacionada).State = EntityState.Modified;
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
            ViewBag.SociedadCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion", cuentaRelacionada.SociedadCognosId);
            ViewBag.CuentaSAPId = new SelectList(db.CuentaSAP, "Id", "Numero", cuentaRelacionada.CuentaSAPId);
            return View(cuentaRelacionada);
        }

        // GET: CuentaRelacionada/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaRelacionada cuentaRelacionada = db.CuentaRelacionada.Find(id);
            if (cuentaRelacionada == null)
            {
                return HttpNotFound();
            }
            return View(cuentaRelacionada);
        }

        // POST: CuentaRelacionada/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentaRelacionada cuentaRelacionada = db.CuentaRelacionada.Find(id);
            cuentaRelacionada.IsActive = false;
            //db.CuentaRelacionada.Remove(cuentaRelacionada);
            db.Entry(cuentaRelacionada).State = EntityState.Modified;
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
