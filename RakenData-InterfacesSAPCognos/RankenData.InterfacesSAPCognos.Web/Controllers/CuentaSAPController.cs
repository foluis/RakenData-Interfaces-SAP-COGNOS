using System;
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
    public class CuentaSAPController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CuentaSAP/
        //[Authorize(Roles="1")]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var cuentasap = db.CuentaSAP.Include(c => c.CuentaCognos1).Include(c => c.TipoCuentaSAP1).Where(cc => cc.IsActive == true);

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

        // Carga masiva de cuentas SAP 
        // return: errores y si no hay devuelve el objeto vacio        
        public string CargeMasivoCuentaSAP(HttpPostedFileBase file)
        {
            CuentaSAP cuentasap = null;            
            int tipoCuentaSAP = 0;
            int? cuentaCargo = null;
            int? cuentaAbono = null;
            bool? esOpen = null;
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
                if (dato.Length != 7)
                {
                    errores.AppendLine("No. Registro" + (i + 1) + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: numero,descripcion,cuentaCognos,TipoCuentaSAP,esOpen,CuentaCargo,CuentaAbono");
                }
                else
                {
                    //if (int.TryParse(dato[2], out cuentaCognos) == false)
                    //{
                    //    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA CUENTA COGNOS NO ES NUMERICO");
                    //}

                    string cuentaCognos = dato[2].Replace("\r", string.Empty);

                    CuentaCognos cuentaCognosExiste = db.CuentaCognos.FirstOrDefault(cc => cc.Numero == cuentaCognos);

                    if (cuentaCognosExiste == null)
                    {
                        errores.AppendLine("No. Registro " + (i + 1) + " ERROR: LA CLAVE DE LA CUENTA COGNOS NO EXISTE. ");
                        //continue;
                    }


                    if (int.TryParse(dato[3], out tipoCuentaSAP) == false)
                    {
                        errores.AppendLine("No. Registro: " + (i + 1) + " ERROR: EL TIPO DE CUENTA SAP NO ES NUMERICO");
                    }


                    if (dato[4] == "NULL")
                    {
                        esOpen = null;
                    }
                    else if (string.IsNullOrWhiteSpace(dato[4]))
                    {
                        esOpen = null;
                    }
                    else
                    {
                        if (dato[4] == "TRUE" || dato[4] == "1")
                        {
                            esOpen = true;
                        }
                        else if (dato[4] == "FALSE" || dato[4] == "0")
                        {
                            esOpen = false;
                        }
                        else
                        {
                            errores.AppendLine("No. Registro: " + (i + 1) + " ERROR: EL CAMPO OPEN NO ES BOOL (TRUE FALSE)");
                        }

                        //if (bool.TryParse(dato[4], out esOpen) == false)
                        //{
                        //    errores.AppendLine("No. Registro: " + i + " ERROR: EL CAMPO OPEN NO ES BOOL (TRUE FALSE)");
                        //}
                    }

                    string dato5 = dato[5];
                    if (dato5 == "NULL")
                    {
                        cuentaCargo = null;
                    }
                    else if (string.IsNullOrWhiteSpace(dato5))
                    {
                        cuentaCargo = null;
                    }
                    else
                    {
                        //cuentaCargo = dato[5].Replace("\r", string.Empty);

                        CuentaCognos cuentaCargoExiste = db.CuentaCognos.FirstOrDefault(cc => cc.Numero == dato5);

                        if (cuentaCargoExiste == null)
                        {
                            errores.AppendLine("No. Registro " + (i + 1) + " ERROR: LA CLAVE DE LA CUENTA COGNOS (CUENTA CARGO) NO EXISTE. ");
                            //continue;
                        }
                        else
                        {
                            cuentaCargo = cuentaCargoExiste.Id;
                        }
                    }

                    //else if (NullableInt.TryParse(dato[5], out cuentaCargo) == false)
                    //{
                    //    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA CUENTA CARGO NO ES NUMERICO");
                    //}


                    //if (int.TryParse(dato[5], out cuentaCargo) == false)
                    //{
                    //    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA CUENTA CARGO NO ES NUMERICO");
                    //}

                    string dato6 = dato[6].Replace("\r", "");
                    if (dato6 == "NULL")
                    {
                        cuentaAbono = null;
                    }
                    else if (string.IsNullOrWhiteSpace(dato6))
                    {
                        cuentaAbono = null;
                    }
                    else
                    {
                        //else if (NullableInt.TryParse(dato[6].Replace("\r", ""), out cuentaAbono) == false)
                        //{cuentaAbono
                        //    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA CUENTA ABONO NO ES NUMERICO");
                        //}

                        CuentaCognos cuentaAbonoExiste = db.CuentaCognos.FirstOrDefault(cc => cc.Numero == dato6);

                        if (cuentaAbonoExiste == null)
                        {
                            errores.AppendLine("No. Registro " + (i + 1) + " ERROR: LA CLAVE DE LA CUENTA COGNOS (CUENTA CARGO) NO EXISTE. ");
                            //continue;
                        }
                        else
                        {
                            cuentaAbono = cuentaAbonoExiste.Id;
                        }
                    }

                    if (errores.Length > 0)
                    {
                        return errores.ToString();
                    }

                    cuentasap = new CuentaSAP()
                    {
                        Numero = dato[0],
                        Descripcion = dato[1],
                        CuentaCognos = cuentaCognosExiste.Id,
                        IsActive = true,
                        TipoCuentaSAP = tipoCuentaSAP,
                        EsOpen = esOpen,
                        CuentaCargo = cuentaCargo,
                        CuentaAbono = cuentaAbono
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
                        cuentaSapExiste.EsOpen = cuentasap.EsOpen;
                        cuentaSapExiste.CuentaCargo = cuentasap.CuentaCargo;
                        cuentaSapExiste.CuentaAbono = cuentasap.CuentaAbono;
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

        // GET: /CuentaSAP/Details/5
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

        // GET: /CuentaSAP/Create
        public ActionResult Create()
        {
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero");
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre");
            return View();
        }

        // POST: /CuentaSAP/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Numero,Descripcion,CuentaCognos,IsActive,TipoCuentaSAP,EsOpen,CuentaCargo,CuentaAbono")] CuentaSAP cuentasap)
        {
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);

            if (ModelState.IsValid)
            {
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
                    cuentaSapExiste.EsOpen = cuentasap.EsOpen;
                    cuentaSapExiste.CuentaCargo = cuentasap.CuentaCargo;
                    cuentaSapExiste.CuentaAbono = cuentasap.CuentaAbono;
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

        // GET: /CuentaSAP/Edit/5
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
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCargo);
            ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaAbono);

            return View(cuentasap);
        }

        // POST: /CuentaSAP/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Numero,Descripcion,CuentaCognos,IsActive,TipoCuentaSAP,EsOpen,CuentaCargo,CuentaAbono")] CuentaSAP cuentasap)
        {
            StringBuilder errores = new StringBuilder();
            ViewBag.CuentaCognos = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCognos);
            ViewBag.TipoCuentaSAP = new SelectList(db.TipoCuentaSAP, "id", "Nombre", cuentasap.TipoCuentaSAP);
            ViewBag.CuentaCargo = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaCargo);
            ViewBag.CuentaAbono = new SelectList(db.CuentaCognos, "Id", "Numero", cuentasap.CuentaAbono);

            if (ModelState.IsValid)
            {
                if (cuentasap.TipoCuentaSAP == 1)
                {
                    cuentasap.EsOpen = null;
                    cuentasap.CuentaCargo = null;
                    cuentasap.CuentaAbono = null;
                }

                if (cuentasap.EsOpen == null || !cuentasap.EsOpen.Value)
                {
                    cuentasap.CuentaCargo = null;
                    cuentasap.CuentaAbono = null;
                }
                else if (cuentasap.CuentaCargo == null || cuentasap.CuentaAbono == null)//cuentasap.EsOpen = true
                {
                    ModelState.AddModelError("Error", "Por favor ingrese una cuenta cognos");
                    return View();
                }
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

        // GET: /CuentaSAP/Delete/5
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

        // POST: /CuentaSAP/Delete/5
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
