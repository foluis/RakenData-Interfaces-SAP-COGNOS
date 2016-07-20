using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models.Partials
{
    public partial class AnioFiscalPartial
    {
        public AnioFiscalPartial()
        {
            this.SaldoInicial = new HashSet<SaldoInicial>();
        }

        public int Id { get; set; }
        public short Anio { get; set; }
        public short AnioInicio { get; set; }

        [Display(Name = "Mes inicio")]
        [Range(1, 12, ErrorMessage = "El valor para el {0} debe ser entre {1} y {2}.")]
        public byte MesInicio { get; set; }
        public short AnioFin { get; set; }

        [Display(Name = "Mes fin")]
        [Range(1, 12, ErrorMessage = "El valor para el {0} debe ser entre {1} y {2}.")]
        public byte MesFin { get; set; }

        public virtual ICollection<SaldoInicial> SaldoInicial { get; set; }
    }
}