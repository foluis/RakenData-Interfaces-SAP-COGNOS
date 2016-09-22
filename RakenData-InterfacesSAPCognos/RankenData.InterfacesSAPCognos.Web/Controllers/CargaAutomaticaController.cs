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
using RankenData.InterfacesSAPCognos.Web.Models.ViewModels;
using System.Globalization;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "2")]
    public class CargaAutomaticaController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        public ActionResult Index()
        {
            var cargaautomatica = db.CargaAutomatica.Include(c => c.TipoArchivoCarga).ToList();

            return View(cargaautomatica);
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

            return View(new CargaAutomaticaViewModel
            {
                RutaArchivo = AppDomain.CurrentDomain.BaseDirectory + "ArchivosCargaAutomatica"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult Create(CargaAutomaticaViewModel cargaAutomatica)
        {
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaAutomatica.TipoArchivo);
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

                cargaAutomatica.UsuarioId = currentUserId;

                CargaAutomatica ca = new CargaAutomatica();

                ca.FechaProgramada = Convert.ToDateTime(cargaAutomatica.FechaProgramadaFormateada);
                ca.RutaArchivo = AppDomain.CurrentDomain.BaseDirectory + "ArchivosCargaAutomatica";
                ca.UsuarioId = currentUserId;
                ca.TipoArchivo = cargaAutomatica.TipoArchivo;
                ca.Email = cargaAutomatica.Email;
                ca.WasLoaded = false;

                cargaAutomatica.RutaArchivo = AppDomain.CurrentDomain.BaseDirectory + "ArchivosCargaAutomatica";

                db.CargaAutomatica.Add(ca);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo realizar la programación automatica");
                    return View(cargaAutomatica);
                }
                catch (DbUpdateException e)
                {
                    Log.WriteLog(ManejoErrores.ErrorValidacionDb(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo realizar la programación automatica, valide que la fecha no exista actualmente");
                    return View(cargaAutomatica);
                }
                catch (Exception e)
                {
                    Log.WriteLog(ManejoErrores.ErrorExepcion(e), EnumTypeLog.Error, true);
                    ModelState.AddModelError("Error", "No se pudo realizar la programación automatica");
                    return View(cargaAutomatica);
                }
                return RedirectToAction("Index");
            }                       

            return View(cargaAutomatica);
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

            string FechaProgramadaString = cargaAutomatica.FechaProgramada.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            CargaAutomaticaViewModel cargaArutomaticaViewModel = new CargaAutomaticaViewModel
            {
                Id = cargaAutomatica.Id,
                FechaProgramada = cargaAutomatica.FechaProgramada,
                FechaProgramadaFormateada = FechaProgramadaString,
                RutaArchivo = AppDomain.CurrentDomain.BaseDirectory + "ArchivosCargaAutomatica",
                UsuarioId = cargaAutomatica.UsuarioId,
                User = cargaAutomatica.User,
                TipoArchivo = cargaAutomatica.TipoArchivo,
                Email = cargaAutomatica.Email,
                WasLoaded = cargaAutomatica.WasLoaded,
                TipoArchivoCarga = cargaAutomatica.TipoArchivoCarga
            };

            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaAutomatica.TipoArchivo);
            return View(cargaArutomaticaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult Edit(CargaAutomaticaViewModel cargaAutomaticaViewModel)
        {
            if (ModelState.IsValid)
            {
                DateTime fechaProgramada = DateTime.ParseExact(cargaAutomaticaViewModel.FechaProgramadaFormateada, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                CargaAutomatica cargaAutomatica = new CargaAutomatica
                {
                    Id = cargaAutomaticaViewModel.Id,
                    FechaProgramada = fechaProgramada,                    
                    RutaArchivo = AppDomain.CurrentDomain.BaseDirectory + "ArchivosCargaAutomatica",
                    UsuarioId = cargaAutomaticaViewModel.UsuarioId,                    
                    TipoArchivo = cargaAutomaticaViewModel.TipoArchivo,
                    Email = cargaAutomaticaViewModel.Email,
                    WasLoaded = cargaAutomaticaViewModel.WasLoaded,
                    TipoArchivoCarga = cargaAutomaticaViewModel.TipoArchivoCarga
                };

                db.Entry(cargaAutomatica).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TipoArchivo = new SelectList(db.TipoArchivoCarga, "Id", "Nombre", cargaAutomaticaViewModel.TipoArchivo);

            cargaAutomaticaViewModel.RutaArchivo = AppDomain.CurrentDomain.BaseDirectory + "ArchivosCargaAutomatica";
           
            string currentUser = User.Identity.Name;         
            if (!string.IsNullOrEmpty(currentUser))
            {
                User user = db.User.FirstOrDefault(a => a.Username == currentUser);
                if (user != null)
                {
                    cargaAutomaticaViewModel.User = user;
                }
            }

            return View(cargaAutomaticaViewModel);
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
