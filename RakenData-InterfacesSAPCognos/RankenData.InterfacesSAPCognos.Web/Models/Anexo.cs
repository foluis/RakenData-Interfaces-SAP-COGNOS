//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RankenData.InterfacesSAPCognos.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Anexo
    {
        public Anexo()
        {
            this.CuentaCognos = new HashSet<CuentaCognos>();
        }
    
        public int id { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public bool IsActive { get; set; }
        public bool Modificable { get; set; }
    
        public virtual ICollection<CuentaCognos> CuentaCognos { get; set; }
    }
}
