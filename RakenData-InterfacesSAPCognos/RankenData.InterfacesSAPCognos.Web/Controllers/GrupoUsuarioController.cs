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

        // GET: /GrupoUsuario/
         //[Authorize(Roles = "4")]
        public ActionResult Index()
        {
            var grupousuario = db.GrupoUsuario.Include(g => g.Grupo).Include(g => g.User);
            return View(grupousuario.ToList());
        }

        // GET: /GrupoUsuario/Details/5
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

        // GET: /GrupoUsuario/Create
        public ActionResult Create()
        {
            ViewBag.IdGrupo = new SelectList(db.Grupo, "Id", "Nombre");
            ViewBag.IdUsuario = new SelectList(db.User, "Id", "Username");
            return View();
        }

        // POST: /GrupoUsuario/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: /GrupoUsuario/Edit/5
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

        // POST: /GrupoUsuario/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: /GrupoUsuario/Delete/5
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

        // POST: /GrupoUsuario/Delete/5
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
