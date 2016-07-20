using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models.Partials
{
    public partial class AnexoPartial
    {
        public AnexoPartial()
        {
            this.CuentaCognos = new HashSet<CuentaCognos>();
        }

        public int id { get; set; }
        public string Clave { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        public bool IsActive { get; set; }
        public bool Modificable { get; set; }

        public virtual ICollection<CuentaCognos> CuentaCognos { get; set; }
    }
}