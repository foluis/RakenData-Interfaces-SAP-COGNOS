using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class SaldosInicialesController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: SaldosIniciales
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
            return View(db.SaldoInicial.ToList());
        }

        // Carga masiva de cargue compania RFC
        // return: errores y si no hay devuelve el objeto vacio        
        public string CargeAnexo(HttpPostedFileBase file)
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
            bool modificable = false;
            var records = result.Split('\n');
            DAT_Reader datReader = new DAT_Reader();

            for (int i = 1; i < records.Count(); i++)
            {
                var dato = records[i].Split(',');
                if (dato.Length < 4)
                {
                    errores.AppendLine("No. Registro" + i + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: CUENTA SAP, SOCIEDAD RFC,AÑO FISCAL, SALDO");
                }               
                if (errores.Length > 0)
                {
                    return errores.ToString();
                }

                var cuentaSAP = db.CuentaSAP.ToList().Where(a => a.IsActive == true && a.Numero == dato[0]);
                var companiaRFC = db.CompaniaRFC.ToList().Where(a => a.RFC == dato[1]);
                var anioFiscal = db.AnioFiscal.ToList().Where(a => a.Anio == Convert.ToInt16(dato[2]));

                saldoInicial = new SaldoInicial()
                {
                    CuentaSAPValue = dato[0],
                    CompaniaRFCValue = dato[1],
                    AnioFiscalValue = Convert.ToInt16(dato[2]),
                    Saldo = Convert.ToDecimal(dato[3])

                };
                if (ModelState.IsValid)
                {
                    SaldoInicial saldoInicialExiste = db.SaldoInicial.FirstOrDefault(cc => cc.AnioFiscalValue == saldoInicial.AnioFiscalValue);
                    if (saldoInicialExiste == null)
                    {                        
                        db.SaldoInicial.Add(saldoInicial);
                    }
                    else
                    {
                        //saldoInicialExiste.Descripcion = saldoInicial.Descripcion;
                        //saldoInicialExiste.Modificable = saldoInicial.Modificable;
                        //saldoInicialExiste.IsActive = true;
                        //db.Entry(saldoInicialExiste).State = EntityState.Modified;
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

        // GET: SaldosIniciales/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SaldosIniciales/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SaldosIniciales/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SaldosIniciales/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SaldosIniciales/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SaldosIniciales/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SaldosIniciales/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
