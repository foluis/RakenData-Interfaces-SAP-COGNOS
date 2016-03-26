﻿using System;
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
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class SaldoInicialController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: SaldoInicial
        public ActionResult Index(HttpPostedFileBase file,string error = null)
        {
            if(error != null)
            {
                ModelState.AddModelError("Error", error);
            }

            if (file != null && file.ContentLength > 0)
            {
                string errores = CargarSaldoInicial(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores);
                }
            }           

            var saldoInicial = db.SaldoInicial.Include(s => s.AnioFiscal).Include(s => s.CompaniaRFC).Include(s => s.CuentaSAP);
            return View(saldoInicial.ToList());
        }

        public string CargarSaldoInicial(HttpPostedFileBase file)
        {
            SaldoInicial saldoInicial = null;
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
            
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length < 4)
                {
                    errores.AppendLine("No. Registro" + i + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: CUENTA SAP, SOCIEDAD RFC,AÑO FISCAL, SALDO");
                    return errores.ToString();
                }
                //if (errores.Length > 0)
                //{
                   
                //}

                var cuentaSAP = db.CuentaSAP.ToList().FirstOrDefault(a => a.IsActive == true && a.Numero == dato[0]);
                var companiaRFC = db.CompaniaRFC.ToList().FirstOrDefault(a => a.RFC == dato[1]);
                var anioFiscal = db.AnioFiscal.ToList().FirstOrDefault(a => a.Anio == Convert.ToInt16(dato[2].Replace("\r","")));

                if(cuentaSAP == null)
                {
                    errores.AppendLine("No. Registro " + i + " ERROR: LA CUENTA SAP (" + dato[0] + ") NO EXISTE \r\r");
                }

                if (companiaRFC == null)
                {
                    errores.AppendLine("No. Registro " + i + " ERROR: LA COMPANIA RFC (" + dato[1] + ") NO EXISTE <br>");
                }

                if (anioFiscal == null)
                {
                    errores.AppendLine("No. Registro " + i + " ERROR: EL AÑO FISCAL (" + dato[2] + ") NO EXISTE \r");
                }

                if(cuentaSAP == null || companiaRFC == null|| anioFiscal == null)
                {
                    continue;
                }

                saldoInicial = new SaldoInicial()
                {
                    CuentaSAPId = cuentaSAP.Id,
                    CuentaSAPValue = dato[0],
                    CompaniaRFCId = companiaRFC.Id,
                    CompaniaRFCValue = dato[1],
                    AnioFiscalId = anioFiscal.Id,
                    AnioFiscalValue = Convert.ToInt16(dato[2]),
                    Saldo = Convert.ToDecimal(dato[3]),
                    EsCargaMasiva = true

                };


                if (ModelState.IsValid)
                {
                    SaldoInicial saldoInicialExiste = db.SaldoInicial.FirstOrDefault(si => si.CuentaSAPId == saldoInicial.CuentaSAPId &&
                                                                                            si.CompaniaRFCId == saldoInicial.CompaniaRFCId &&
                                                                                            si.AnioFiscalId == saldoInicial.AnioFiscalId);
                    if (saldoInicialExiste == null)
                    {
                        db.SaldoInicial.Add(saldoInicial);
                    }
                    else
                    {
                        saldoInicialExiste.Saldo = saldoInicial.Saldo;                       
                        db.Entry(saldoInicialExiste).State = EntityState.Modified;
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

        // GET: SaldoInicial/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaldoInicial saldoInicial = db.SaldoInicial.Find(id);
            if (saldoInicial == null)
            {
                return HttpNotFound();
            }
            return View(saldoInicial);
        }

        // GET: SaldoInicial/Create
        public ActionResult Create()
        {
            ViewBag.AnioFiscalId = new SelectList(db.AnioFiscal, "Id", "Id");
            ViewBag.CompaniaRFCId = new SelectList(db.CompaniaRFC, "Id", "RFC");
            ViewBag.CuentaSAPId = new SelectList(db.CuentaSAP, "Id", "Numero");
            return View();
        }

        // POST: SaldoInicial/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CuentaSAPId,CompaniaRFCId,AnioFiscalId,Saldo,CuentaSAPValue,CompaniaRFCValue,AnioFiscalValue")] SaldoInicial saldoInicial)
        {
            if (ModelState.IsValid)
            {
                db.SaldoInicial.Add(saldoInicial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AnioFiscalId = new SelectList(db.AnioFiscal, "Id", "Id", saldoInicial.AnioFiscalId);
            ViewBag.CompaniaRFCId = new SelectList(db.CompaniaRFC, "Id", "RFC", saldoInicial.CompaniaRFCId);
            ViewBag.CuentaSAPId = new SelectList(db.CuentaSAP, "Id", "Numero", saldoInicial.CuentaSAPId);
            return View(saldoInicial);
        }

        // GET: SaldoInicial/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaldoInicial saldoInicial = db.SaldoInicial.Find(id);
            if (saldoInicial == null)
            {
                return HttpNotFound();
            }
            ViewBag.AnioFiscalId = new SelectList(db.AnioFiscal, "Id", "Id", saldoInicial.AnioFiscalId);
            ViewBag.CompaniaRFCId = new SelectList(db.CompaniaRFC, "Id", "RFC", saldoInicial.CompaniaRFCId);
            ViewBag.CuentaSAPId = new SelectList(db.CuentaSAP, "Id", "Numero", saldoInicial.CuentaSAPId);
            return View(saldoInicial);
        }

        // POST: SaldoInicial/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CuentaSAPId,CompaniaRFCId,AnioFiscalId,Saldo,CuentaSAPValue,CompaniaRFCValue,AnioFiscalValue")] SaldoInicial saldoInicial)
        {
            if (ModelState.IsValid)
            {
                saldoInicial.EsCargaMasiva = false;
                db.Entry(saldoInicial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AnioFiscalId = new SelectList(db.AnioFiscal, "Id", "Id", saldoInicial.AnioFiscalId);
            ViewBag.CompaniaRFCId = new SelectList(db.CompaniaRFC, "Id", "RFC", saldoInicial.CompaniaRFCId);
            ViewBag.CuentaSAPId = new SelectList(db.CuentaSAP, "Id", "Numero", saldoInicial.CuentaSAPId);
            return View(saldoInicial);
        }

        // GET: SaldoInicial/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaldoInicial saldoInicial = db.SaldoInicial.Find(id);
            if (saldoInicial == null)
            {
                return HttpNotFound();
            }
            return View(saldoInicial);
        }

        // POST: SaldoInicial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SaldoInicial saldoInicial = db.SaldoInicial.Find(id);
            db.SaldoInicial.Remove(saldoInicial);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ActualizarSaldoInicial(string anioFiscal)
        {
            int añoFiscal = Convert.ToInt32(anioFiscal);
            var oAnioFiscal = db.AnioFiscal.FirstOrDefault(a => a.AnioInicio == añoFiscal);
            
            if (oAnioFiscal == null)
            {
                return RedirectToAction("Index", new { error = "El año fiscal seleccionado no existe" });
            }
            else
            {
                List<int?> result;
                result = db.ActualizarSaldosIniciales(añoFiscal).ToList();

                if (result != null && result.Count() > 0 && result.First().Value == 1)
                {
                    return RedirectToAction("Index", new { error = "No hay datos para crear saldo inicial automatico " + anioFiscal });
                }
                else
                {
                    return RedirectToAction("Index", new { error = "Se calculó exitosamente el saldo inicial para el año " + anioFiscal });
                }
            }            
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