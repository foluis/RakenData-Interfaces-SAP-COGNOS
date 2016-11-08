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
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "1")]
    public class SaldoInicialController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        public ActionResult Index(HttpPostedFileBase file, string error = null)
        {
            if (error != null)
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

            var saldoInicial = db.SaldoInicial.Include(s => s.AnioFiscal).Include(s => s.CompaniaCognos).Include(s => s.CuentaCognos);
            return View(saldoInicial.ToList());
        }
        public string CargarSaldoInicial(HttpPostedFileBase file)
        {
            SaldoInicial saldoInicial = null;
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
                if (dato.Length < 4)
                {
                    errores.AppendLine("No. Registro" + i + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: CUENTA COGNOS, SOCIEDAD COGNOS,AÑO FISCAL, SALDO");
                    return errores.ToString();
                }

                string clave = dato[1].Length <= 13 ? dato[1].ToUpper() : dato[1].Substring(0, 13).ToUpper();

                var cuentaCognos = db.CuentaCognos.ToList().FirstOrDefault(a => a.IsActive == true && a.Numero == dato[0]);
                var companiaCognos = db.CompaniaCognos.ToList().FirstOrDefault(a => a.Clave == clave);
                var anioFiscal = db.AnioFiscal.ToList().FirstOrDefault(a => a.Anio == Convert.ToInt16(dato[2].Replace("\r", "")));

                if (cuentaCognos == null)
                {
                    errores.AppendLine("No. Registro " + i + " ERROR: LA CUENTA COGNOS (" + dato[0] + ") NO EXISTE \r\r");
                }

                if (companiaCognos == null)
                {
                    errores.AppendLine("No. Registro " + i + " ERROR: LA COMPANIA COGNOS (" + dato[1] + ") NO EXISTE <br>");
                }

                if (anioFiscal == null)
                {
                    errores.AppendLine("No. Registro " + i + " ERROR: EL AÑO FISCAL (" + dato[2] + ") NO EXISTE \r");
                }

                if (cuentaCognos == null || companiaCognos == null || anioFiscal == null)
                {
                    continue;
                }

                saldoInicial = new SaldoInicial()
                {
                    CuentaCognosId = cuentaCognos.Id,
                    CuentaCognosValue = dato[0],
                    CompaniaCognosId = companiaCognos.Id,
                    CompaniaCognosValue = dato[1],
                    AnioFiscalId = anioFiscal.Id,
                    AnioFiscalValue = Convert.ToInt16(dato[2]),
                    Saldo = Convert.ToDecimal(dato[3]),
                    EsCargaMasiva = true
                };

                if (ModelState.IsValid)
                {
                    SaldoInicial saldoInicialExiste = db.SaldoInicial.FirstOrDefault(si => si.CuentaCognosId == saldoInicial.CuentaCognosId &&
                                                                                            si.CompaniaCognosId == saldoInicial.CompaniaCognosId &&
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

        public ActionResult Create()
        {
            ViewBag.AnioFiscalId = new SelectList(db.AnioFiscal, "Id", "Id");
            ViewBag.CompaniaCognosId = new SelectList(db.CompaniaCognos, "Id", "Clave");
            ViewBag.CuentaCognosId = new SelectList(db.CuentaCognos, "Id", "Numero");
            return View();
        }

        // POST: SaldoInicial/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CuentaCognosId,CompaniaCognosId,AnioFiscalId,Saldo,CuentaCognosValue,CompaniaCognosValue,AnioFiscalValue")] SaldoInicial saldoInicial)
        {
            if (ModelState.IsValid)
            {
                db.SaldoInicial.Add(saldoInicial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AnioFiscalId = new SelectList(db.AnioFiscal, "Id", "Id", saldoInicial.AnioFiscalId);
            ViewBag.CompaniaCognosId = new SelectList(db.CompaniaCognos, "Id", "Clave", saldoInicial.CompaniaCognosId);
            ViewBag.CuentaCognosId = new SelectList(db.CuentaCognos, "Id", "Numero", saldoInicial.CuentaCognosId);
            return View(saldoInicial);
        }

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
            ViewBag.CompaniaCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion", saldoInicial.CompaniaCognosId);
            ViewBag.CuentaCognosId = new SelectList(db.CuentaCognos, "Id", "Numero", saldoInicial.CuentaCognosId);
            return View(saldoInicial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CuentaCognosId,CuentaCognosValue,CompaniaCognosId,CompaniaCognosValue,AnioFiscalId,AnioFiscalValue,Saldo,EsCargaMasiva")] SaldoInicial saldoInicial)
        {
            if (ModelState.IsValid)
            {
                saldoInicial.EsCargaMasiva = false;
                db.Entry(saldoInicial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AnioFiscalId = new SelectList(db.AnioFiscal, "Id", "Id", saldoInicial.AnioFiscalId);
            ViewBag.CompaniaCognosId = new SelectList(db.CompaniaCognos, "Id", "Descripcion", saldoInicial.CompaniaCognosId);
            ViewBag.CuentaCognosId = new SelectList(db.CuentaCognos, "Id", "Numero", saldoInicial.CuentaCognosId);
            return View(saldoInicial);
        }

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SaldoInicial saldoInicial = db.SaldoInicial.Find(id);
            db.SaldoInicial.Remove(saldoInicial);
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
