using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RankenData.InterfacesSAPCognos.Web.Models;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    [Authorize(Roles = "1")]
    public class AnioFiscalController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /AnioFiscal/
         //[Authorize(Roles = "1")]
        public ActionResult Index()
        {          
            return View(db.AnioFiscal.ToList());
        }

        // GET: /AnioFiscal/Details/5
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

        // GET: /AnioFiscal/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AnioFiscal/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: /AnioFiscal/Edit/5
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

        // POST: /AnioFiscal/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: /AnioFiscal/Delete/5
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

        // POST: /AnioFiscal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AnioFiscal aniofiscal = db.AnioFiscal.Find(id);
            db.AnioFiscal.Remove(aniofiscal);
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
