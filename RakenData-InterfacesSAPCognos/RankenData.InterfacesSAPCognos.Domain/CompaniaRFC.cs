//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RankenData.InterfacesSAPCognos.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class CompaniaRFC
    {
        public int Id { get; set; }
        public string RFC { get; set; }
        public string Descripcion { get; set; }
        public int CompaniaCognos { get; set; }
    
        public virtual CompaniaCognos CompaniaCognos1 { get; set; }
    }
}
