using Ranken.ISC.Contracts.Repositories;
using RankenData.InterfacesSAPCognos.Domain;
using RankenData.InterfacesSAPCognos.Model;
using RankenData.InterfacesSAPCognos.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace RankenData.InterfacesSAPCognos.WebAPI.Controllers
{
    public class CuentaCognos2Controller : ApiController
    {
        IRepository<CuentaCognos> cuentaCognos;

        public CuentaCognos2Controller(IRepository<CuentaCognos> cuentaCognos)
        {
            this.cuentaCognos = cuentaCognos;
        }

        public IQueryable<CuentaCognos> GetCuentaCognos()
        {           
            var valor1 = cuentaCognos.GetAll();
            var valor2 = valor1.ToList();
            return valor1;
        }

        //[ResponseType(typeof(CuentaCognos))]
        //public IHttpActionResult GetCuentaCognos(int id)
        //{
        //    IRepository<CuentaCognos> db = new CuentaCognosRepository(new InterfasSAPCognosEntities());
        //    var valor2 = db.GetAll();
        //    var valor3 = db.GetAll().ToList();

        //CuentaCognos cuentaCognos = db.CuentaCognos.Find(id);
        //if (cuentaCognos == null)
        //{
        //    return NotFound();
        //}

        //    return Ok(cuentaCognos);
        //}
    }
}
