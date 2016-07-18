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
    public class DatosCabeceraController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // GET: /DatosCabecera/
         //[Authorize(Roles = "1")]
        public ActionResult Index()
        {            
            return View(db.DatosCabecera.ToList());
        }

        // GET: /DatosCabecera/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatosCabecera datoscabecera = db.DatosCabecera.Find(id);
            if (datoscabecera == null)
            {
                return HttpNotFound();
            }
            return View(datoscabecera);
        }

        // GET: /DatosCabecera/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /DatosCabecera/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Nombre,Valor")] DatosCabecera datoscabecera)
        {
            if (ModelState.IsValid)
            {
                db.DatosCabecera.Add(datoscabecera);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datoscabecera);
        }

        // GET: /DatosCabecera/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatosCabecera datoscabecera = db.DatosCabecera.Find(id);
            if (datoscabecera == null)
            {
                return HttpNotFound();
            }
            return View(datoscabecera);
        }

        // POST: /DatosCabecera/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Nombre,Valor")] DatosCabecera datoscabecera)
        {
            if (ModelState.IsValid)
            {
                datoscabecera.Valor = datoscabecera.Valor.ToUpper();
                db.Entry(datoscabecera).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    string exMessage = ex.ToString();
                    throw;
                }
                
                return RedirectToAction("Index");
            }
            return View(datoscabecera);
        }

        // GET: /DatosCabecera/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatosCabecera datoscabecera = db.DatosCabecera.Find(id);
            if (datoscabecera == null)
            {
                return HttpNotFound();
            }
            return View(datoscabecera);
        }

        // POST: /DatosCabecera/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DatosCabecera datoscabecera = db.DatosCabecera.Find(id);
            db.DatosCabecera.Remove(datoscabecera);
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
