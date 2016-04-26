using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using System.Web;
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
    public class CompaniaRFCController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /CompaniaRFC/
        //[Authorize(Roles = "1")]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var companiarfc = db.CompaniaRFC.Include(c => c.CompaniaCognos1);
            if (file != null && file.ContentLength > 0)
            {
                string errores = CargeCompaniaRFC(file);
                if (errores.Length > 0)
                {
                    ModelState.AddModelError("Error", errores);                
                }
            }
            return View(companiarfc.ToList());
        }

        // Carga masiva de cargue compania RFC
        // return: errores y si no hay devuelve el objeto vacio        
        public string CargeCompaniaRFC(HttpPostedFileBase file)
        {
            CompaniaRFC companiaRFC = null;
            int companiaCognos;
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
                    errores.AppendLine("No. Registro" + (i + 1) + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: RFC,Descripcion,IdCompaniaCognos");
                    //Log.WriteLog("No. Registro" +  (i + 1) + " ERROR: LA ESTRUCTURA DEL ARCHIVO NO ES: RFC,Descripcion,IdCompaniaCognos", EnumTypeLog.Error, true);
                    //continue;
                }
                else
                {
                    string claveCompaniaCognos = dato[2].Replace("\r", string.Empty);
                    int claveCompaniaCognosId= 0;

                    if (int.TryParse(claveCompaniaCognos, out companiaCognos))
                    {
                        claveCompaniaCognosId = companiaCognos;
                    }

                    CompaniaCognos companiaCognosExiste = db.CompaniaCognos.FirstOrDefault(cc => cc.Clave == claveCompaniaCognosId);

                    if (companiaCognosExiste == null)
                    {
                        errores.AppendLine("No. Registro " + (i + 1) + " ERROR: LA CLAVE DE LA COMPANIA COGNOS NO EXISTE. ");
                        //continue;
                    }
                    else
                    {
                        //if (int.TryParse(companiaCognosExiste.id, out companiaCognos) == false)
                        //{
                        //    errores.AppendLine("No. Registro: " + i + " ERROR: EL ID DE LA COMPANIA COGNOS NO ES NUMERICO");
                        //}

                        //if (errores.Length > 0)
                        //{
                        //    return errores.ToString();

                        //}

                        string descripcion = dato[1].Replace("\r", string.Empty).ToUpper();
                        descripcion = dato[1].Length <= 35 ? descripcion : descripcion.Substring(0, 35);

                        companiaRFC = new CompaniaRFC()
                        {
                            RFC = dato[0],
                            Descripcion = descripcion,
                            CompaniaCognos = companiaCognosExiste.Id
                        };

                        CompaniaRFC companiaRFCExiste = db.CompaniaRFC.FirstOrDefault(cc => cc.RFC == companiaRFC.RFC);

                        if (companiaRFCExiste == null)
                        {
                            db.CompaniaRFC.Add(companiaRFC);
                        }
                        else
                        {
                            companiaRFCExiste.Descripcion = companiaRFC.Descripcion;
                            companiaRFCExiste.CompaniaCognos = companiaRFC.CompaniaCognos;
                            db.Entry(companiaRFCExiste).State = EntityState.Modified;
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
            }

            return errores.ToString();
        }

        // GET: /CompaniaRFC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Create
        public ActionResult Create()
        {
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion");
            return View();
        }

        // POST: /CompaniaRFC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RFC,Descripcion,CompaniaCognos")] CompaniaRFC companiarfc)
        {
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            if (ModelState.IsValid)
            {
                var existCompania = db.CompaniaRFC.Select(crfc => crfc.CompaniaCognos == companiarfc.CompaniaCognos).FirstOrDefault();
                if (existCompania)
                {
                    ModelState.AddModelError("Error", "Ex: La compañia Cognos, ya se encuentra asociada a otra vuenta RFC");
                    ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
                    return View(companiarfc);
                }

                companiarfc.RFC = companiarfc.RFC.ToUpper();
                companiarfc.Descripcion = companiarfc.Descripcion.ToUpper();
                db.CompaniaRFC.Add(companiarfc);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la Compañia");
                    return View();
                }
                catch (DbUpdateException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la Compañia");
                    return View();
                }
                catch (Exception e)
                {
                    Log.WriteLog(ManejoErrores.ErrorExepcion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo crear la Compañia");
                    return View();
                };
                return RedirectToAction("Index");
            }
    
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            return View(companiarfc);
        }

        // POST: /CompaniaRFC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RFC,Descripcion,CompaniaCognos")] CompaniaRFC companiarfc)
        {
            if (ModelState.IsValid)
            {
                companiarfc.Descripcion = companiarfc.Descripcion.ToUpper();
                db.Entry(companiarfc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompaniaCognos = new SelectList(db.CompaniaCognos, "Id", "Descripcion", companiarfc.CompaniaCognos);
            return View(companiarfc);
        }

        // GET: /CompaniaRFC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            if (companiarfc == null)
            {
                return HttpNotFound();
            }
            return View(companiarfc);
        }

        // POST: /CompaniaRFC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompaniaRFC companiarfc = db.CompaniaRFC.Find(id);
            db.CompaniaRFC.Remove(companiarfc);
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
