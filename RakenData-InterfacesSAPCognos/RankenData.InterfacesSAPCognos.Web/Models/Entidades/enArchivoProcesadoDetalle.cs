using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models
{
    public partial class ArchivoProcesadoDetalle
    {
        /// <summary>
        /// indica si el registo de archivo procesado detalle se puede modificar
        /// </summary>
        public bool EsModificable { get; set; }
    }
}