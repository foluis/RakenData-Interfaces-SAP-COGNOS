using RankenData.InterfacesSAPCognos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankenData.InterfacesSAPCognos.Model.Repositories
{
    public class CuentaSAPRepository : Repository<CuentaSAP>
    {
        public CuentaSAPRepository(InterfasSAPCognosEntities context) : base(context)
        {
            if (context == null)
                throw new ArgumentNullException();
        }
    }
}
