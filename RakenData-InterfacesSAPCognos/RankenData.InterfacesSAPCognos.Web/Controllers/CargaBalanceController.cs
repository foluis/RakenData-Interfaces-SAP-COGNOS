using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
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
        public ActionResult CargarBalance()
        {
                        ArchivoCarga archivoCarga = new ArchivoCarga();
            List<ArchivoCargaDetalle> lstarchivoCargaDetalle = new List<ArchivoCargaDetalle>();
            StringBuilder errores = new StringBuilder();
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

                    //Insert tabla archivocarga
                    archivoCarga.Nombre = file.FileName + r.Next(100); //todo: random para pruebas
                    archivoCarga.Identificador = "B/R" + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString().Substring(2);
                    archivoCarga.Fecha = DateTime.Now;
                    archivoCarga.TipoArchivoCarga = 1; //todo: cambiar por un enumerable
                    archivoCarga.Anio_Col3 = Mexsalcta[0].Anio; // deberian ser el mismo tipo
                    archivoCarga.Mes_Col4 = Mexsalcta[0].Mes;// deberian ser el mismo tipo
                    archivoCarga.Usuario = 1; // todo: id del usuario

                    db.ArchivoCarga.Add(archivoCarga);
                    db.SaveChanges();

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
                        string error = e.Message;
                        ModelState.AddModelError("Error:", e.Message) ;
                        throw;
                    }

                    List<ValidateFileLoaded_Result> cuentasCompanias_NoExistentes = db.ValidateFileLoaded(archivoCarga.Id).ToList();
                    
                    ModelState.AddModelError("No se han cargado las siguientes companias:", "companias");// --->ValidateFileLoaded_Result.Id = 2
                    ModelState.AddModelError("No se han cargado las siguientes cuentas:", "cuentas"); //---> ValidateFileLoaded_Result.Id = 1

                    // llamar a un store procedure validar que la info esta bien esta devuelve un obj tabla
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
                    /*
                     * 
                     * 
                     insert tabla archivocarga
                      
                     * [Nombre]  = file name
      ,[Identificador] = MEXSALCTA (balance) o MEX_SALINT  (intercompañias)
      ,[Fecha] = fecha del sistema
      ,[TipoArchivoCarga]= id de la tabla TipoArchivoCarga lo puedo hacer con un enumerable
      ,[Anio_Col3] = public int Anio es del primer registro;
      ,[Mes_Col4] = public int Mes es del primer registro;
      ,[Usuario] = el que este logeado en la app
                     * 
                     * for MEXSALCTA[]
                            insert tabla archivo carga detalle
                         *  ,[ArchivoCarga] = id del que acabe de crear en archivo carga
                         *  if mexsalcta => [CopaniaRelacionada]= null
                     *  exit for
                     *  llamar a un store procedure validar que la info esta bien esta devuelve un obj tabla
                     */
            //        foreach (var record in records)
            //        {
            //            i++;
            //            var dato = record.Split(',');
            //            if (dato.Length < 3)
            //            {
            //                errores.AppendLine("No. Registro" + i + "ERROR: lA ESTRUCTURA DEL ARCHIVO NO ES: NUMERO - DESCRIPCION- ANEXO ID");
            //                //TODO: IMPLEMENTAR EL ERROR
            //                // ERROR lA ESTRUCTURA DEL ARCHIVO NO ES: NUMERO - DESCRIPCION- ANEXO ID
            //            }
            //            if (int.TryParse(dato[2], out anexoid) == false)
            //            {
            //                errores.AppendLine("No. Registro" + i + "ERROR: EL ID DEL ANEXO NO ES NUMERICO");

            //                //TODO: IMPLEMENTAR EL ERROR
            //                // ERROR EL ID DEL ANEXO NO ES NUMERICO
            //            }
            //                    if (ModelState.IsValid)
            //            {
            //                db.CuentaCognos.Add();
            //                db.SaveChanges();
            //            }
            //        }
            //    }
            //}
            //if (errores.Length > 0)
            //{


            //    //TODO: IMPLEMENTAR EL ERROR

            //    // ERROR EL ID DEL ANEXO NO ES NUMERICO
            //}
           // return RedirectToAction("Index");
        }

        //
        // GET: /CargaBalance/
        public ActionResult Index()
        {
            return View();
        }
        //
        // GET: /CargaBalance/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /CargaBalance/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CargaBalance/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CargaBalance/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /CargaBalance/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CargaBalance/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /CargaBalance/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
