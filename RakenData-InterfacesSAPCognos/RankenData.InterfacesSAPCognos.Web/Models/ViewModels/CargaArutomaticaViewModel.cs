using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models.ViewModels
{
    public class CargaAutomaticaViewModel
    {
        public int Id { get; set; }
        public DateTime FechaProgramada { get; set; }

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "Ingrese el valor de la fecha")]
        public string FechaProgramadaFormateada { get; set; }
        public string RutaArchivo { get; set; }
        public int UsuarioId { get; set; }
        public User User { get; set; }

        [Display(Name = "Tipo Archivo")]
        [Required(ErrorMessage = "Ingrese el tipo de archivo")]
        public int TipoArchivo { get; set; }

        [Required(ErrorMessage = "Ingrese el valor del email")]
        [EmailAddress(ErrorMessage = "Valor del email incorrecto")]
        public string Email { get; set; }
        public bool WasLoaded { get; set; }
        public TipoArchivoCarga TipoArchivoCarga { get; set; }
    }
}