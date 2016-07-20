using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades
{
    public  class CargarArchivo
    {

        private  EntitiesRakenData db = new EntitiesRakenData();

        /// <summary>
        /// Carga los archivos segun su tipo en la base de datos
        /// </summary>
        /// <param name="nombreArchivo">Nombre del archivo a cargar</param>
        /// <param name="datosCargar">Datos del archivo</param>
        /// <param name="tipoArchivo">tipo de archivo Balance  o  Intercompanias</param>
        /// <returns></returns>
        public string CargarArchivoBD(string nombreArchivo, string datosCargar, EnumTipoArchivoCarga tipoArchivo, int UserId)
        {
            ArchivoCarga archivoCarga = new ArchivoCarga();
            List<ArchivoCargaDetalle> lstarchivoCargaDetalle = null;
            StringBuilder errores = new StringBuilder();
            StringBuilder sbcompaniasNoCargadas = new StringBuilder();
            StringBuilder sbcuentasNoCargadas = new StringBuilder();
            List<ValidateFileToLoad_Result> anioMes_YaExistentes = null;
            MEXSALCTA[] Mexsalcta = null;
            MEX_SALINT[] Mexsalint = null;
            short anio;
            byte mes;
            DAT_Reader datReader = new DAT_Reader();

            try
            {
                if (tipoArchivo == EnumTipoArchivoCarga.Balance)
                {
                    Mexsalcta = datReader.StartReading_MEXSALCTA(datosCargar);
                    anioMes_YaExistentes = db.ValidateFileToLoad(Mexsalcta[0].Anio, Mexsalcta[0].Mes, (int)tipoArchivo).ToList();
                    anio = Mexsalcta[0].Anio;
                    mes = Mexsalcta[0].Mes;
                }
                else
                {
                    Mexsalint = datReader.StartReading_MEX_SALINT(datosCargar);
                    anioMes_YaExistentes = db.ValidateFileToLoad(Mexsalint[0].Anio, Mexsalint[0].Mes, (int)tipoArchivo).ToList();
                    anio = (short)Mexsalint[0].Anio;
                    mes = (byte)Mexsalint[0].Mes;
                }

                if (anioMes_YaExistentes.Count == 0)
                {
                    return "No se cargo el archivo";
                }
                if (anioMes_YaExistentes[0].IdTipo == -1)
                {
                    return "El periodo que contiene el archivo ya existe";
                }
                if (anioMes_YaExistentes[0].IdTipo == 0)
                {         
                    //Insert tabla archivocarga
                    archivoCarga.Nombre = nombreArchivo.Substring(nombreArchivo.LastIndexOf(@"\") + 1);

                    if(tipoArchivo == EnumTipoArchivoCarga.Balance)
                        archivoCarga.Identificador = "B/R" + DateTime.Today.ToString("MM") + DateTime.Today.Year.ToString().Substring(2);
                    else
                        archivoCarga.Identificador = "INT" + DateTime.Today.ToString("MM") + DateTime.Today.Year.ToString().Substring(2);

                    archivoCarga.Fecha = DateTime.Now;
                    archivoCarga.TipoArchivoCarga = (int)tipoArchivo;
                    archivoCarga.Anio_Col3 = anio;
                    archivoCarga.Mes_Col4 = mes;
                    archivoCarga.Usuario = UserId; //TODO: id del usuario
                  
                    db.ArchivoCarga.Add(archivoCarga);                   
                   
                    db.SaveChanges();

                    if (tipoArchivo == EnumTipoArchivoCarga.Balance)
                    {
                        lstarchivoCargaDetalle = MapeaDetalleBalance(Mexsalcta, archivoCarga.Id);
                    }
                    else
                    {
                        lstarchivoCargaDetalle = MapeaDetalleIntercompania(Mexsalint, archivoCarga.Id);
                    }                

                    // Insert tabla archivo carga detalle
                    db.ArchivoCargaDetalle.AddRange(lstarchivoCargaDetalle);
                  
                    db.SaveChanges();                 

                    List<ValidateFileLoaded_Result> cuentasCompanias_NoExistentes = db.ValidateFileLoaded(archivoCarga.Id).ToList();

                    // Companias no cargadas
                    List<ValidateFileLoaded_Result> companiasNoCargadas = cuentasCompanias_NoExistentes.Where(cc => cc.IdTipo == 2).ToList();

                    if (companiasNoCargadas.Count > 0)
                    {
                        sbcompaniasNoCargadas.AppendLine("No se han cargado las siguientes Compañias: </br>");
                        companiasNoCargadas.ForEach(cnc =>
                            sbcompaniasNoCargadas.AppendLine(cnc.Descripcion + " " + cnc.Value + "</br>")
                            );
                    }

                    // Cuentas no cargadas
                    List<ValidateFileLoaded_Result> cuentasNoCargadas = cuentasCompanias_NoExistentes.Where(cc => cc.IdTipo == 1).ToList();

                    if (cuentasNoCargadas.Count > 0)
                    {
                        sbcuentasNoCargadas.AppendLine("No se han cargado las siguientes Cuentas: </br>");
                        cuentasNoCargadas.ForEach(cnc =>
                            sbcuentasNoCargadas.AppendLine(cnc.Descripcion + " " + cnc.Value + "</br>")
                            );
                    }

                    if (sbcompaniasNoCargadas.ToString() != string.Empty || sbcuentasNoCargadas.ToString() != string.Empty)
                    {
                        return sbcompaniasNoCargadas.ToString() + "</br>" + sbcuentasNoCargadas.ToString();
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                return ManejoErrores.ErrorValidacion(e);
            }
            catch (DbUpdateException e)
            {
                return ManejoErrores.ErrorValidacionDb(e);
            }
            catch (FileHelpers.FileHelpersException)
            {
                throw;
                //string fieldName = ((FileHelpers.ConvertException)ex).FieldName;
                //string lineNumber = ((FileHelpers.ConvertException)ex).LineNumber.ToString();
            }
            catch (Exception ex)
            {
                return ManejoErrores.ErrorExepcion(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Mapea el objeto mexsalcta en la lista archivo carga detalle
        /// </summary>
        /// <param name="Mexsalcta"></param>
        /// <returns></returns>
        private static List<ArchivoCargaDetalle> MapeaDetalleBalance(MEXSALCTA[] Mexsalcta, int archivoCargaID)
        {
            List<ArchivoCargaDetalle> lstarchivoCargaDetalle = new List<ArchivoCargaDetalle>();
            for (int i = 0; i < Mexsalcta.Length; i++)
            {
                lstarchivoCargaDetalle.Add(
                     new ArchivoCargaDetalle()
                     {
                         ArchivoCarga = archivoCargaID,
                         FilaArchivo = i + 1,
                         Escenario = int.Parse(Mexsalcta[i].Escenario),
                         Versión = byte.Parse(Mexsalcta[i].Version),
                         Anio = Mexsalcta[i].Anio,
                         Mes = Mexsalcta[i].Mes,
                         UnidadDeNegocio = byte.Parse(Mexsalcta[i].UnidadNegocio),
                         Cuenta = Mexsalcta[i].Cuenta,
                         Moneda = Mexsalcta[i].Moneda,
                         GAAP = Mexsalcta[i].GAAP,
                         Interfase = Mexsalcta[i].Interfase,
                         NominalAjustado = byte.Parse(Mexsalcta[i].NominalAjustado),
                         Compania = Mexsalcta[i].Compania,
                         CopaniaRelacionada = null,
                         MovimientoDebitoPeriodo = Mexsalcta[i].MovimientoDebitoPeriodo,
                         MovimientoCreditoPeriodo = Mexsalcta[i].MovimientoCreditoPeriodo,
                         MovimientoDebitoAcumulado = Mexsalcta[i].MovimientoDébitoAcumulado,
                         MovimientoCreditoAcumulado = Mexsalcta[i].MovimientoCréditoAcumulado,
                         SaldoAcumuladoPeriodo = Mexsalcta[i].SaldoAcumuladoPeriodo,
                         HoraActualizacion = Mexsalcta[i].HoraActualizacion.ToString("yyyyMMddHHmm"),
                         UsuarioActualizacion = Mexsalcta[i].UsuarioSctualizacion
                     });
            }

            return lstarchivoCargaDetalle;
        }

        /// <summary>
        /// Mapea el objeto mexsalcta en la lista archivo carga detalle
        /// </summary>
        /// <param name="Mexsalcta"></param>
        /// <param name="archivoCargaID"></param>
        /// <returns></returns>
        private static List<ArchivoCargaDetalle> MapeaDetalleIntercompania(MEX_SALINT[] Mexsalint, int archivoCargaID)
        {
            List<ArchivoCargaDetalle> lstarchivoCargaDetalle = new List<ArchivoCargaDetalle>();
            for (int i = 0; i < Mexsalint.Length; i++)
            {
                lstarchivoCargaDetalle.Add(
                     new ArchivoCargaDetalle()
                     {
                         ArchivoCarga = archivoCargaID,
                         FilaArchivo = i + 1,
                         Escenario = int.Parse(Mexsalint[i].Escenario),
                         Versión = byte.Parse(Mexsalint[i].Version),
                         Anio = (short)Mexsalint[i].Anio,
                         Mes = (byte)Mexsalint[i].Mes,
                         UnidadDeNegocio = byte.Parse(Mexsalint[i].UnidadNegocio),
                         Cuenta = Mexsalint[i].Cuenta,
                         Moneda = Mexsalint[i].Moneda,
                         GAAP = Mexsalint[i].GAAP,
                         Interfase = Mexsalint[i].Interfase,
                         NominalAjustado = byte.Parse(Mexsalint[i].NominalAjustado),
                         Compania = Mexsalint[i].Compania,
                         CopaniaRelacionada = Mexsalint[i].CompaniaRelacionada,
                         MovimientoDebitoPeriodo = Mexsalint[i].MovimientoDebitoPeriodo,
                         MovimientoCreditoPeriodo = Mexsalint[i].MovimientoCreditoPeriodo,
                         MovimientoDebitoAcumulado = Mexsalint[i].MovimientoDebitoAcumulado,
                         MovimientoCreditoAcumulado = Mexsalint[i].MovimientoCreditoAcumulado,
                         SaldoAcumuladoPeriodo = Mexsalint[i].SaldoAcumuladoPeriodo,
                         HoraActualizacion = Mexsalint[i].HoraActualizacion.ToString("yyyyMMddHHmm"),
                         UsuarioActualizacion = Mexsalint[i].UsuarioActualizacion
                     });
            }
            return lstarchivoCargaDetalle;
        }
    }
}


