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
    
    public partial class CompaniaCognos
    {
        public CompaniaCognos()
        {
            this.ArchivoProcesado = new HashSet<ArchivoProcesado>();
            this.CompaniaRFC = new HashSet<CompaniaRFC>();
        }
    
        public int Id { get; set; }
        public int Clave { get; set; }
        public string Descripcion { get; set; }
    
        public virtual ICollection<ArchivoProcesado> ArchivoProcesado { get; set; }
        public virtual ICollection<CompaniaRFC> CompaniaRFC { get; set; }
    }
}
