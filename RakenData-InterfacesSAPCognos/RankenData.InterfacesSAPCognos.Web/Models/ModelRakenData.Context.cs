﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EntitiesRakenData : DbContext
    {
        public EntitiesRakenData()
            : base("name=EntitiesRakenData")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdministracionAplicacion> AdministracionAplicacion { get; set; }
        public virtual DbSet<Anexo> Anexo { get; set; }
        public virtual DbSet<AnioFiscal> AnioFiscal { get; set; }
        public virtual DbSet<ArchivoCarga> ArchivoCarga { get; set; }
        public virtual DbSet<ArchivoCargaDetalle> ArchivoCargaDetalle { get; set; }
        public virtual DbSet<CargaAutomatica> CargaAutomatica { get; set; }
        public virtual DbSet<CompaniaCognos> CompaniaCognos { get; set; }
        public virtual DbSet<CompaniaRFC> CompaniaRFC { get; set; }
        public virtual DbSet<CuentaCognos> CuentaCognos { get; set; }
        public virtual DbSet<CuentaSAP> CuentaSAP { get; set; }
        public virtual DbSet<DatosCabecera> DatosCabecera { get; set; }
        public virtual DbSet<TipoArchivoCarga> TipoArchivoCarga { get; set; }
        public virtual DbSet<TipoCuentaSAP> TipoCuentaSAP { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
