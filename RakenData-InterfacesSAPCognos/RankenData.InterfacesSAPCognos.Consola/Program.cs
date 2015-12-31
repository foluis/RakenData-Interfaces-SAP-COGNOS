using Ranken.ISC.Contracts.Repositories;
using RankenData.InterfacesSAPCognos.Domain;
using RankenData.InterfacesSAPCognos.Model;
using RankenData.InterfacesSAPCognos.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankenData.InterfacesSAPCognos.Consola
{
    class Program
    {
        static void Main(string[] args)
        {
            GetTiposCuentaSAP();
        }

        private static void GetTiposCuentaSAP()
        {
            //Antes de repos 0
            //InterfasSAPCognosEntities db = new InterfasSAPCognosEntities();
            //var TipoCuentaSAP = db.TipoCuentaSAP;
            //var TipoCuentaSAP2 = TipoCuentaSAP.ToList();

            ////Repos paso 1
            CuentaCognosRepository cuentaCognos = new CuentaCognosRepository(new InterfasSAPCognosEntities());
            var valor = cuentaCognos.GetAll();

            ////Repos paso 2
            IRepository<CuentaCognos> repository = new CuentaCognosRepository(new InterfasSAPCognosEntities());
            var valor2 = cuentaCognos.GetAll();
            var valor3 = cuentaCognos.GetAll().ToList();
        }
    }
}
