using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CargaBalanceController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        // Cargue balance / resultados
        [HttpPost]
        public string CargarBalance()
        {
            ArchivoCarga archivoCarga = new ArchivoCarga();
            List<ArchivoCargaDetalle> lstarchivoCargaDetalle = new List<ArchivoCargaDetalle>();
            StringBuilder errores = new StringBuilder();
            StringBuilder sbcompaniasNoCargadas = new StringBuilder();
            StringBuilder sbcuentasNoCargadas = new StringBuilder();
            Random r = new Random();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    BinaryReader b = new BinaryReader(file.InputStream);
                    byte[] binData = b.ReadBytes((int)file.InputStream.Length);
                    string result = System.Text.Encoding.UTF8.GetString(binData);
                    var records = result.Split('\n');

                    DAT_Reader datReader = new DAT_Reader();
                    MEXSALCTA[] Mexsalcta = datReader.StartReading_MEXSALCTA(result);

                    List<ValidateFileToLoad_Result> anioMes_YaExistentes = db.ValidateFileToLoad(Mexsalcta[0].Anio, Mexsalcta[0].Mes).ToList();
                    
                    if (anioMes_YaExistentes.Count == 0)
                    {                       
                        return "No se cargo el archivo";
                    }
                    if (anioMes_YaExistentes[0].IdTipo == -1)
                    {
                       return "No se carga el archivo si el periodo ya existe";
                    }
                     if (anioMes_YaExistentes[0].IdTipo == 0)
                     {
                         ModelState.AddModelError("Si se carga el archivo", "cuentas");
                         
                         //Insert tabla archivocarga
                        archivoCarga.Nombre = file.FileName + r.Next(100); //todo: random para pruebas
                        archivoCarga.Identificador = "B/R" + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString().Substring(2);
                        archivoCarga.Fecha = DateTime.Now;
                        archivoCarga.TipoArchivoCarga = (int)EnumTipoArchivoCarga.Balance;
                        archivoCarga.Anio_Col3 = Mexsalcta[0].Anio; 
                        archivoCarga.Mes_Col4 = Mexsalcta[0].Mes;
                        archivoCarga.Usuario = 1; // todo: id del usuario

                         //Guardar en bd
                        db.ArchivoCarga.Add(archivoCarga);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            return ManejoErrores.ErrorValidacion(e);
                        }
                        catch (DbUpdateException e)
                        {
                            return ManejoErrores.ErrorValidacionDb(e);
                        }
                        catch (Exception e)
                        {
                            return ManejoErrores.ErrorExepcion(e);
                        }


                        // Insert tabla archivo carga detalle
                        for (int i = 0; i < Mexsalcta.Length; i++)
                        {
                            lstarchivoCargaDetalle.Add(
                                 new ArchivoCargaDetalle()
                                 {
                                     ArchivoCarga = archivoCarga.Id,
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

                        db.ArchivoCargaDetalle.AddRange(lstarchivoCargaDetalle);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            return ManejoErrores.ErrorValidacion(e);
                        }
                        catch (DbUpdateException e)
                        {
                            return ManejoErrores.ErrorValidacionDb(e);
                        }
                        catch (Exception e)
                        {
                            return ManejoErrores.ErrorExepcion(e);
                        }


                        List<ValidateFileLoaded_Result> cuentasCompanias_NoExistentes = db.ValidateFileLoaded(archivoCarga.Id).ToList();

                        // Companias no cargadas
                        List<ValidateFileLoaded_Result> companiasNoCargadas = cuentasCompanias_NoExistentes.Where(cc => cc.IdTipo == 2).ToList();
                        
                        if (companiasNoCargadas.Count > 0)
                        {
                            sbcompaniasNoCargadas.AppendLine("No se han cargado las siguientes Compañias: </br>");
                            companiasNoCargadas.ForEach(cnc =>
                                sbcompaniasNoCargadas.AppendLine(cnc.Description + " " + cnc.Value + "</br>")
                                );
                        }

                        // Cuentas no cargadas
                        List<ValidateFileLoaded_Result> cuentasNoCargadas = cuentasCompanias_NoExistentes.Where(cc => cc.IdTipo == 1).ToList();
                        
                        if (cuentasNoCargadas.Count > 0)
                        {
                            sbcuentasNoCargadas.AppendLine("No se han cargado las siguientes Cuentas: </br>");
                            cuentasNoCargadas.ForEach(cnc =>
                                sbcuentasNoCargadas.AppendLine(cnc.Description + " " + cnc.Value + "</br>")
                                );                                                                  
                        }

                        if (sbcompaniasNoCargadas.ToString() != string.Empty || sbcuentasNoCargadas.ToString() != string.Empty)
                         {
                             return sbcompaniasNoCargadas.ToString() + "</br>" + sbcuentasNoCargadas.ToString();
                         }

                        return string.Empty;
                     }
                }
            }
            return string.Empty;     
        }

        //
        // GET: /CargaBalance/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /EliminarBalance/Default1
        public ActionResult EliminarBalance()
        {
            var a = new SelectList(db.ArchivoCarga, "Id", "Id");
            //TODO: revisar si solo debe traer informacion filtrada solo por las base

            ViewBag.Identificador1 = new SelectList(db.Anexo, "id", "Clave");
            return View();
        }

        [HttpPost, ActionName("EliminarBalance")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarBalanceConfirmed(string Identificador)
        {
            ArchivoCarga archivoCarga = db.ArchivoCarga.FirstOrDefault(ac => ac.Identificador == Identificador);
            if (archivoCarga != null)
            {
                int id = archivoCarga.Id;
                 db.EliminarArchivoCarga(id);
               
            }
            else 
            {
                ModelState.AddModelError("Error", "No se encontro informacion para ese identificador");
            }
            
            ViewBag.Identificador = new SelectList(db.ArchivoCarga, "Identificador", "Identificador");
            return View();
        }
    }
}
