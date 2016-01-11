using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class CargaIntercompaniasController : Controller
    {
        private EntitiesRakenData db = new EntitiesRakenData();

        //
        // GET: /CargaIntercompanias/
          [Authorize(Roles = "2")]
        public ActionResult Index()
        {
            return View();
        }

         // Cargar Intercompania

          [Authorize(Roles = "2")]
        [HttpPost]
        public string CargarIntercompania()
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
                    MEX_SALINT[] Mexsalint = datReader.StartReading_MEX_SALINT(result);

                    List<ValidateFileToLoad_Result> anioMes_YaExistentes = db.ValidateFileToLoad(Mexsalint[0].Anio, Mexsalint[0].Mes).ToList();

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
                        //Insert tabla archivocarga
                        archivoCarga.Nombre = file.FileName + r.Next(100); //todo: random para pruebas
                        archivoCarga.Identificador = "B/R" + Mexsalint[0].Mes.ToString() + Mexsalint[0].Anio.ToString().Substring(2);
                        archivoCarga.Fecha = DateTime.Now;
                        archivoCarga.TipoArchivoCarga = (int)EnumTipoArchivoCarga.Intercompanias;
                        archivoCarga.Anio_Col3 = (short)Mexsalint[0].Anio;
                        archivoCarga.Mes_Col4 = (byte)Mexsalint[0].Mes;
                        archivoCarga.Usuario = 1; // todo: id del usuario

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
                        for (int i = 0; i < Mexsalint.Length; i++)
                        {
                            lstarchivoCargaDetalle.Add(
                                 new ArchivoCargaDetalle()
                                 {
                                     ArchivoCarga = archivoCarga.Id,
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
                                sbcompaniasNoCargadas.AppendLine(cnc.Description + " "+ cnc.Value + "</br>")
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
        // GET: /EliminarBalance/Default1
        public ActionResult EliminarIntercompanias()
        {
            ViewBag.Identificador = new SelectList(db.ArchivoCarga.Where(aa => aa.TipoArchivoCarga == (int)EnumTipoArchivoCarga.Intercompanias), "Id", "Identificador");
            return View();
        }

        [HttpPost, ActionName("EliminarIntercompanias")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarIntercompaniasConfirmed(string Identificador)
        {
            int idArchivo;
            if (int.TryParse(Identificador, out idArchivo))
            {
                db.EliminarArchivoCarga(idArchivo);
                ModelState.AddModelError("Error", "La Eliminación fue Exitosa");
            }
            else
            {
                ModelState.AddModelError("Error", "El id del archivo es invalido");
            }

            ViewBag.Identificador = new SelectList(db.ArchivoCarga, "Id", "Identificador");
            return View();
        }
    }
}
