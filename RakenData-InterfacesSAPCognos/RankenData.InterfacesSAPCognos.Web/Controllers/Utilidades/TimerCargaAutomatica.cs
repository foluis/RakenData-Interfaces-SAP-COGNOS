using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class TimerCargaAutomatica
    {
        private EntitiesRakenData db = new EntitiesRakenData();
        List<CargaAutomatica> lstCargaAutomatica = null;
        DAT_Reader datReader = new DAT_Reader();
        MailInfo mailInfo = new MailInfo();
        string ruta;
        string procesaCargaAutomatica = "0";

        /// <summary>
        /// Inicalizar timer
        /// </summary>
        public void Init()
        {
            string tiempoCargaAutomatica = ConfigurationManager.AppSettings["tiempoCargaAutomatica"];
            System.Timers.Timer timer = new System.Timers.Timer();
            int time = int.Parse(tiempoCargaAutomatica);
            time = time < 0 ? 3 : time; //minimo cada 3 hora
            // time = time * 3600000;
            time = 30000; //TODO: Esta linea es de pruebas
            timer.Interval = time;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            try
            {
                procesaCargaAutomatica = db.AdministracionAplicacion.Where(aa => aa.Id == 1).FirstOrDefault().Valor;
                if (procesaCargaAutomatica == "1")
                {
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("No se pudo obtener informacion de la tabla de administracion para el proceso de Carga automatica, valide que tenga conección. Error: "  + ex.ToString(), EnumTypeLog.Error, true);
            }            
        }

        /// <summary>
        /// Cada ciclo valida si hay archivos para cargar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            string nombreArchivo = string.Empty;
            string result = string.Empty;
            string errores = string.Empty;
            this.lstCargaAutomatica = new List<CargaAutomatica>();

            //ConfigurationManager.AppSettings["procesaCargaAutomatica"]

            var cargaAutomaticaInfo = db.CargaAutomatica;

            if (procesaCargaAutomatica == "1" && cargaAutomaticaInfo != null && cargaAutomaticaInfo.Count() > 0)
            {
                var registrosCargaAutomatica = cargaAutomaticaInfo.ToList();
                bool hayArchivo = false;
                foreach (CargaAutomatica cargaAutomatica in registrosCargaAutomatica)
                {
                    if (cargaAutomatica.FechaProgramada.Date == DateTime.Now.Date && cargaAutomatica.WasLoaded == false)
                    {
                        this.lstCargaAutomatica.Add(cargaAutomatica);
                    }
                }

                if (lstCargaAutomatica != null && lstCargaAutomatica.Count > 0)
                {
                    foreach (CargaAutomatica cargaAutomatica in lstCargaAutomatica)
                    {
                        if (cargaAutomatica.TipoArchivo == (int)EnumTipoArchivoCarga.Balance)
                        {
                            nombreArchivo = ConfigurationManager.AppSettings["nombreArchivoBalance"] + cargaAutomatica.FechaProgramada.Year.ToString() + cargaAutomatica.FechaProgramada.ToString("MM") + cargaAutomatica.FechaProgramada.Day.ToString() + ".DAT";
                            ruta = Path.Combine(cargaAutomatica.RutaArchivo, nombreArchivo);
                            if (System.IO.File.Exists(ruta))
                            {
                                result = System.IO.File.ReadAllText(ruta);
                                CargarArchivo cargarArchivo = new CargarArchivo();
                                errores = cargarArchivo.CargarArchivoBD("nombreArchivo", result, EnumTipoArchivoCarga.Balance);
                                hayArchivo = true;
                            }
                            else
                            {
                                hayArchivo = false;
                            }
                        }
                        else // Si el archivo es intercompañias
                        {
                            nombreArchivo = ConfigurationManager.AppSettings["nombreArchivoIntercomania"] + cargaAutomatica.FechaProgramada.Year.ToString() + cargaAutomatica.FechaProgramada.ToString("MM") + cargaAutomatica.FechaProgramada.Day.ToString() + ".DAT";
                            ruta = Path.Combine(cargaAutomatica.RutaArchivo, nombreArchivo);
                            if (System.IO.File.Exists(ruta))
                            {
                                result = System.IO.File.ReadAllText(ruta);
                                CargarArchivo cargarArchivo = new CargarArchivo();
                                errores = cargarArchivo.CargarArchivoBD("nombreArchivo", result, EnumTipoArchivoCarga.Intercompanias);
                                hayArchivo = true;
                            }
                            else
                            {
                                hayArchivo = false;
                            }
                        }

                        if (string.IsNullOrEmpty(errores) && hayArchivo)
                        {
                            cargaAutomatica.WasLoaded = true;
                            db.Entry(cargaAutomatica).State = EntityState.Modified;
                            db.SaveChanges();

                            Log.WriteLog("El archivo : " + nombreArchivo + " se cargó exitosamente", EnumTypeLog.Event, true);

                            mailInfo.Subject = ConfigurationManager.AppSettings["emailSubject"];
                            mailInfo.To = cargaAutomatica.Email.Replace(",", ";").Split(';').ToList();
                            mailInfo.Message = ConfigurationManager.AppSettings["emailSubject"]; ;
                            AdmMail.Enviar(mailInfo);
                        }
                        else if(errores!= string.Empty)
                        {
                            Log.WriteLog("El archivo : " + nombreArchivo + " presentó los siguientes errores: " + errores, EnumTypeLog.Event, true);
                            mailInfo.Subject = ConfigurationManager.AppSettings["emailSubject"];
                            mailInfo.To = cargaAutomatica.Email.Replace(",", ";").Split(';').ToList();
                            mailInfo.Message = "El archivo : " + nombreArchivo + " presentó los siguientes errores: " + errores; ;
                            AdmMail.Enviar(mailInfo);
                        }
                    }
                }
            }
        }
    }
}