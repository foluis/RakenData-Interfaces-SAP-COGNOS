using RankenData.InterfacesSAPCognos.Model;
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
            InterfasSAPCognosEntities db = new InterfasSAPCognosEntities();
            var TipoCuentaSAP = db.TipoCuentaSAP.ToList();
        }
    }
}
