using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models.Partials
{ 
    public class DatosCabeceraPartial
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        [StringLength(4, ErrorMessage = "Tamaño máximo de 4 caracteres")]
        public string Valor { get; set; }
    }
}