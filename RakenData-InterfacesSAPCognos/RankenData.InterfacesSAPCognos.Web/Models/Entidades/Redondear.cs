using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models.Entidades
{
    public class Redondear
    {
        /// <summary>
        /// id de la formula de redondeo
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descripcion de la formula de redondeo
        /// </summary>
        public string Nombre { get; set; }
    }
}