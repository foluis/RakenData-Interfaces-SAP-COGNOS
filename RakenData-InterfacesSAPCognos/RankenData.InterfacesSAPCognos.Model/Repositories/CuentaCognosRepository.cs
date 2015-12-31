using RankenData.InterfacesSAPCognos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankenData.InterfacesSAPCognos.Model.Repositories
{
    public class CuentaCognosRepository : Repository<CuentaCognos>
    {
        public CuentaCognosRepository(InterfasSAPCognosEntities context) : base(context)
        {
            if (context == null)
                throw new ArgumentNullException();
        }
    }
}
