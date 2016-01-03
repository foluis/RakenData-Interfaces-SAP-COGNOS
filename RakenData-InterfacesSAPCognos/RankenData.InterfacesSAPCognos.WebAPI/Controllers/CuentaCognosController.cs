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
    public class CuentaCognosController : ApiController
    {
        private IRepository<CuentaCognos> cuentaCognos;

        public CuentaCognosController(IRepository<CuentaCognos> cuentaCognos)
        {
            this.cuentaCognos = cuentaCognos;
        }

        public IQueryable<CuentaCognos> GetCuentasCognosFUCK()
        {
            var valor1 = cuentaCognos.GetAll();
            var valor2 = valor1.ToList();
            return valor1;
        }

        //public CuentaCognos GetCuentaCognos(int id)        
        //public HttpResponseMessage GetCuentaCognos(int id)
        //{
        //    var cuentaCognos = this.cuentaCognos.GetById(id);
        //    //return cuentaCognos
        //    if (cuentaCognos != null)
        //    {
        //        return Request.CreateResponse<CuentaCognos>(HttpStatusCode.OK, cuentaCognos);
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Item not found");
        //    }
        //}

        //public HttpResponseMessage PostCuentaCognos(CuentaCognos cuentaCognos)
        //{
        //    this.cuentaCognos.Insert(cuentaCognos);
        //    var msg = Request.CreateResponse<CuentaCognos>(HttpStatusCode.Created, cuentaCognos);
        //    msg.Headers.Location = new Uri(Request.RequestUri + cuentaCognos.Id.ToString());
        //    return msg;
        //}

        
    }


}
