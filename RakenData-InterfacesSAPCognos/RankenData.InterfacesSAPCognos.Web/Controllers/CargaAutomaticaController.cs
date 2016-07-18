using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using System.Data.Entity.Validation;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System.Data.Entity.Infrastructure;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "2")]
    public class CargaAutomaticaController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();
      
        public ActionResult Index()
        {
            var cargaautomatica = db.CargaAutomatica.Include(c => c.TipoArchivoCarga);
            return View(cargaautomatica.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CargaAutomatica cargaautomatica = db.CargaAutomatica.Find(id);
            if (cargaautomatica == null)
            {
                return HttpNotFound();
            }
            return View(cargaautomatica);
        }

        public ActionResult Create()
        {
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,FechaProgramada,RutaArchivo,TipoArchivo,Email")] CargaAutomatica cargaautomatica)
        {
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaautomatica.TipoArchivo);
            if (ModelState.IsValid)
            {
                string currentUser = User.Identity.Name;
                int currentUserId = 1;
                if (!string.IsNullOrEmpty(currentUser))
                {
                    User user = db.User.FirstOrDefault(a => a.Username == currentUser);
                    if (user != null)
                    {
                        currentUserId = user.Id;
                    }
                }

                cargaautomatica.Usuario = currentUserId; 
                db.CargaAutomatica.Add(cargaautomatica);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo realizar la programacion automatica");
                    return View();
                }
                catch (DbUpdateException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo realizar la programacion automatica");
                    return View();
                }
                catch (Exception e)
                {
                    Log.WriteLog(ManejoErrores.ErrorExepcion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo realizar la programacion automatica");
                    return View();
                }
                return RedirectToAction("Index");
            }
            
            return View(cargaautomatica);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CargaAutomatica cargaAutomatica = db.CargaAutomatica.Find(id);
            if (cargaAutomatica == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaAutomatica.TipoArchivo);
            return View(cargaAutomatica);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,FechaProgramada,RutaArchivo,TipoArchivo,Email")] CargaAutomatica cargaautomatica)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cargaautomatica).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaautomatica.TipoArchivo);
            return View(cargaautomatica);
        }
                
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CargaAutomatica cargaautomatica = db.CargaAutomatica.Find(id);
            if (cargaautomatica == null)
            {
                return HttpNotFound();
            }
            return View(cargaautomatica);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CargaAutomatica cargaautomatica = db.CargaAutomatica.Find(id);
            db.CargaAutomatica.Remove(cargaautomatica);
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
