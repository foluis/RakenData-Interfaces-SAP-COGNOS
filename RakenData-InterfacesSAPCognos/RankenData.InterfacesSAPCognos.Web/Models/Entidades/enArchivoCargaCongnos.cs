using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models.Entidades
{
    public class enArchivoCargaCongnos
    {
        /// <summary>
        /// Id de las sociedad cognos seleccionados
        /// </summary>
        public List<int> LstIdCompaniasCognos { get; set; }
        /// <summary>
        /// Periodo a evaluar
        /// </summary>
        public int Periodo { get; set; }

        /// <summary>
        /// Año a evaluar
        /// </summary>
        public int Anio { get; set; }

        public int TipoArchivo { get; set; }
    }
}