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
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
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
    
        public virtual DbSet<AnioFiscal> AnioFiscal { get; set; }
        public virtual DbSet<AdministracionAplicacion> AdministracionAplicacion { get; set; }
        public virtual DbSet<Anexo> Anexo { get; set; }
        public virtual DbSet<ArchivoCarga> ArchivoCarga { get; set; }
        public virtual DbSet<ArchivoCargaDetalle> ArchivoCargaDetalle { get; set; }
        public virtual DbSet<ArchivoProcesado> ArchivoProcesado { get; set; }
        public virtual DbSet<ArchivoProcesadoDetalle> ArchivoProcesadoDetalle { get; set; }
        public virtual DbSet<CompaniaCognos> CompaniaCognos { get; set; }
        public virtual DbSet<CompaniaRFC> CompaniaRFC { get; set; }
        public virtual DbSet<CuentaCognos> CuentaCognos { get; set; }
        public virtual DbSet<CuentaSAP> CuentaSAP { get; set; }
        public virtual DbSet<DatosCabecera> DatosCabecera { get; set; }
        public virtual DbSet<Grupo> Grupo { get; set; }
        public virtual DbSet<GrupoUsuario> GrupoUsuario { get; set; }
        public virtual DbSet<HistorialArchivoProcesadoDetalle> HistorialArchivoProcesadoDetalle { get; set; }
        public virtual DbSet<TipoArchivoCarga> TipoArchivoCarga { get; set; }
        public virtual DbSet<TipoArchivoCreacion> TipoArchivoCreacion { get; set; }
        public virtual DbSet<TipoCuentaSAP> TipoCuentaSAP { get; set; }
        public virtual DbSet<TipoModificacion> TipoModificacion { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<CargaAutomatica> CargaAutomatica { get; set; }
    
        public virtual ObjectResult<ValidateFileLoaded_Result> ValidateFileLoaded(Nullable<int> fileLoadedId)
        {
            var fileLoadedIdParameter = fileLoadedId.HasValue ?
                new ObjectParameter("fileLoadedId", fileLoadedId) :
                new ObjectParameter("fileLoadedId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ValidateFileLoaded_Result>("ValidateFileLoaded", fileLoadedIdParameter);
        }
    
        public virtual ObjectResult<ValidateFileToLoad_Result> ValidateFileToLoad(Nullable<int> anio, Nullable<int> mes)
        {
            var anioParameter = anio.HasValue ?
                new ObjectParameter("Anio", anio) :
                new ObjectParameter("Anio", typeof(int));
    
            var mesParameter = mes.HasValue ?
                new ObjectParameter("Mes", mes) :
                new ObjectParameter("Mes", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ValidateFileToLoad_Result>("ValidateFileToLoad", anioParameter, mesParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> CreateArchivoBalance(string sociedadesCognos, Nullable<int> periodo, Nullable<int> anio, string tiposArchivoCreacionId, Nullable<int> usuario)
        {
            var sociedadesCognosParameter = sociedadesCognos != null ?
                new ObjectParameter("SociedadesCognos", sociedadesCognos) :
                new ObjectParameter("SociedadesCognos", typeof(string));
    
            var periodoParameter = periodo.HasValue ?
                new ObjectParameter("Periodo", periodo) :
                new ObjectParameter("Periodo", typeof(int));
    
            var anioParameter = anio.HasValue ?
                new ObjectParameter("Anio", anio) :
                new ObjectParameter("Anio", typeof(int));
    
            var tiposArchivoCreacionIdParameter = tiposArchivoCreacionId != null ?
                new ObjectParameter("TiposArchivoCreacionId", tiposArchivoCreacionId) :
                new ObjectParameter("TiposArchivoCreacionId", typeof(string));
    
            var usuarioParameter = usuario.HasValue ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("CreateArchivoBalance", sociedadesCognosParameter, periodoParameter, anioParameter, tiposArchivoCreacionIdParameter, usuarioParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> CreateArchivoResultados(string sociedadesCognos, Nullable<int> periodo, Nullable<int> anio, string tiposArchivoCreacionId, Nullable<int> usuario)
        {
            var sociedadesCognosParameter = sociedadesCognos != null ?
                new ObjectParameter("SociedadesCognos", sociedadesCognos) :
                new ObjectParameter("SociedadesCognos", typeof(string));
    
            var periodoParameter = periodo.HasValue ?
                new ObjectParameter("Periodo", periodo) :
                new ObjectParameter("Periodo", typeof(int));
    
            var anioParameter = anio.HasValue ?
                new ObjectParameter("Anio", anio) :
                new ObjectParameter("Anio", typeof(int));
    
            var tiposArchivoCreacionIdParameter = tiposArchivoCreacionId != null ?
                new ObjectParameter("TiposArchivoCreacionId", tiposArchivoCreacionId) :
                new ObjectParameter("TiposArchivoCreacionId", typeof(string));
    
            var usuarioParameter = usuario.HasValue ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("CreateArchivoResultados", sociedadesCognosParameter, periodoParameter, anioParameter, tiposArchivoCreacionIdParameter, usuarioParameter);
        }
    
        public virtual ObjectResult<CreateArchivoIntercompanias_Result> CreateArchivoIntercompanias(string sociedadesCognos, Nullable<int> periodo, Nullable<int> anio, string tiposArchivoCreacionId, Nullable<int> usuario)
        {
            var sociedadesCognosParameter = sociedadesCognos != null ?
                new ObjectParameter("SociedadesCognos", sociedadesCognos) :
                new ObjectParameter("SociedadesCognos", typeof(string));
    
            var periodoParameter = periodo.HasValue ?
                new ObjectParameter("Periodo", periodo) :
                new ObjectParameter("Periodo", typeof(int));
    
            var anioParameter = anio.HasValue ?
                new ObjectParameter("Anio", anio) :
                new ObjectParameter("Anio", typeof(int));
    
            var tiposArchivoCreacionIdParameter = tiposArchivoCreacionId != null ?
                new ObjectParameter("TiposArchivoCreacionId", tiposArchivoCreacionId) :
                new ObjectParameter("TiposArchivoCreacionId", typeof(string));
    
            var usuarioParameter = usuario.HasValue ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CreateArchivoIntercompanias_Result>("CreateArchivoIntercompanias", sociedadesCognosParameter, periodoParameter, anioParameter, tiposArchivoCreacionIdParameter, usuarioParameter);
        }
    
        public virtual int EliminarArchivoCarga(Nullable<int> archivoCargaId)
        {
            var archivoCargaIdParameter = archivoCargaId.HasValue ?
                new ObjectParameter("ArchivoCargaId", archivoCargaId) :
                new ObjectParameter("ArchivoCargaId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("EliminarArchivoCarga", archivoCargaIdParameter);
        }
    }
}
