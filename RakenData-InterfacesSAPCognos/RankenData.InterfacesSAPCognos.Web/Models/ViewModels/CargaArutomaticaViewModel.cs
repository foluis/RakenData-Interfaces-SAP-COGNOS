using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models.ViewModels
{
    public class CargaAutomaticaViewModel
    {
        public int Id { get; set; }
        public DateTime FechaProgramada { get; set; }
        public String FechaProgramadaFormateada { get; set; }
        public string RutaArchivo { get; set; }
        public int UsuarioId { get; set; }
        public User User { get; set; }
        public int TipoArchivo { get; set; }
        public string Email { get; set; }
        public bool WasLoaded { get; set; }
        public TipoArchivoCarga TipoArchivoCarga { get; set; }
    }
}