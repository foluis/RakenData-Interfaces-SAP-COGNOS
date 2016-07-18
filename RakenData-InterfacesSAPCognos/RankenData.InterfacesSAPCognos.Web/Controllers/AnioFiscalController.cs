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

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "1")]
    public class AnioFiscalController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();    
     
        public ActionResult Index()
        {           
            return View(db.AnioFiscal.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnioFiscal aniofiscal = db.AnioFiscal.Find(id);
            if (aniofiscal == null)
            {
                return HttpNotFound();
            }
            return View(aniofiscal);
        }
        
        public ActionResult Create()
        {
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Anio,AnioInicio,MesInicio,AnioFin,MesFin")] AnioFiscal aniofiscal)
        {
            if (ModelState.IsValid)
            {
                List<AnioFiscal> anniosFiscales = db.AnioFiscal.Where(x => x.Anio == aniofiscal.Anio).ToList();
                if (anniosFiscales.Count > 0)
                {
                    ModelState.AddModelError("Error", "El año fiscal ya existe");
                    return View();
                }
                else
                {
                    db.AnioFiscal.Add(aniofiscal);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(aniofiscal);
        }
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnioFiscal aniofiscal = db.AnioFiscal.Find(id);
            if (aniofiscal == null)
            {
                return HttpNotFound();
            }
            return View(aniofiscal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Anio,AnioInicio,MesInicio,AnioFin,MesFin")] AnioFiscal aniofiscal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aniofiscal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aniofiscal);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnioFiscal aniofiscal = db.AnioFiscal.Find(id);
            if (aniofiscal == null)
            {
                return HttpNotFound();
            }
            return View(aniofiscal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {            
            AnioFiscal aniofiscal = db.AnioFiscal.Find(id);
            List<SaldoInicial> saldosIniciales = db.SaldoInicial.Where(x => x.AnioFiscalId == id).ToList();

            if (saldosIniciales.Count > 0)
            {               
                ModelState.AddModelError("Error", "Existen saldos iniciales con este año fiscal. Por esta razón no se puede borrar.");
                return View(aniofiscal);
            }
            else
            {
                db.AnioFiscal.Remove(aniofiscal);
            }
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
