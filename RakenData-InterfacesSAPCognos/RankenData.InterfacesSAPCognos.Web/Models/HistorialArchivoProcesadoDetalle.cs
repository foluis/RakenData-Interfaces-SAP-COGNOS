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
    
    public partial class HistorialArchivoProcesadoDetalle
    {
        public int Id { get; set; }
        public int ArchivoProcesadoDetalleId { get; set; }
        public string Account { get; set; }
        public decimal Amount { get; set; }
        public decimal TransactionAmount { get; set; }
        public int UsuarioId { get; set; }
        public System.DateTime FechaModificacion { get; set; }
        public int TipoModificacionId { get; set; }
    
        public virtual ArchivoProcesadoDetalle ArchivoProcesadoDetalle { get; set; }
        public virtual TipoModificacion TipoModificacion { get; set; }
        public virtual User User { get; set; }
    }
}
