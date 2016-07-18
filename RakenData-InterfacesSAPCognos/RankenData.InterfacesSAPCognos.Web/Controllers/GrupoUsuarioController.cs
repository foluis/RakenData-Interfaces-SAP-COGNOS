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
    [Authorize(Roles = "5")]
    public class GrupoUsuarioController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();      
      
        public ActionResult Index()
        {
            var grupousuario = db.GrupoUsuario.Include(g => g.Grupo).Include(g => g.User);
            return View(grupousuario.ToList());
        }
               
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoUsuario grupousuario = db.GrupoUsuario.Find(id);
            if (grupousuario == null)
            {
                return HttpNotFound();
            }
            return View(grupousuario);
        }
     
        public ActionResult Create()
        {
            ViewBag.IdGrupo = new SelectList(db.Grupo, "Id", "Nombre");
            ViewBag.IdUsuario = new SelectList(db.User, "Id", "Username");
            return View();
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,IdUsuario,IdGrupo")] GrupoUsuario grupousuario)
        {
            if (ModelState.IsValid)
            {
                db.GrupoUsuario.Add(grupousuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdGrupo = new SelectList(db.Grupo, "Id", "Nombre", grupousuario.IdGrupo);
            ViewBag.IdUsuario = new SelectList(db.User, "Id", "Username", grupousuario.IdUsuario);
            return View(grupousuario);
        }
                
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoUsuario grupousuario = db.GrupoUsuario.Find(id);
            if (grupousuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdGrupo = new SelectList(db.Grupo, "Id", "Nombre", grupousuario.IdGrupo);
            ViewBag.IdUsuario = new SelectList(db.User, "Id", "Username", grupousuario.IdUsuario);
            return View(grupousuario);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,IdUsuario,IdGrupo")] GrupoUsuario grupousuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grupousuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdGrupo = new SelectList(db.Grupo, "Id", "Nombre", grupousuario.IdGrupo);
            ViewBag.IdUsuario = new SelectList(db.User, "Id", "Username", grupousuario.IdUsuario);
            return View(grupousuario);
        }
       
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoUsuario grupousuario = db.GrupoUsuario.Find(id);
            if (grupousuario == null)
            {
                return HttpNotFound();
            }
            return View(grupousuario);
        }
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GrupoUsuario grupousuario = db.GrupoUsuario.Find(id);
            db.GrupoUsuario.Remove(grupousuario);
            db.SaveChanges();

            User user = db.User.Find(grupousuario.IdUsuario);
            System.Web.Security.Roles.RemoveUserFromRole(user.Username, grupousuario.IdGrupo.ToString());
            
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
