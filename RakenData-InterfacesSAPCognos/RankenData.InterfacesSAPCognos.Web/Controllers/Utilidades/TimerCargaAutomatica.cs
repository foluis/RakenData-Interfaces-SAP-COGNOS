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
using System.Timers;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class TimerCargaAutomatica
    {
        private EntitiesRakenData db = new EntitiesRakenData();
        //List<CargaAutomatica> lstCargaAutomatica = null;
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
            Timer timer = new Timer();

            double time = 0;
            bool isNumber = double.TryParse(tiempoCargaAutomatica, out time);
            time = time <= 0 ? 3 : time; //minimo cada 3 hora
            time = time * 3600000;

#if DEBUG
            time = 30000; //Valida cada 30 segundos en debug
#endif

            timer.Interval = time;
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);

            CreateFolder();

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
                Log.WriteLog("No se pudo obtener informacion de la tabla de administracion para el proceso de Carga automatica, valide que tenga conección. Error: " + ex.ToString(), EnumTypeLog.Error, true);
            }
        }

        private void CreateFolder()
        {
            string folderName = AppDomain.CurrentDomain.BaseDirectory + "ArchivosCargaAutomatica";

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
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
            //this.lstCargaAutomatica = new List<CargaAutomatica>();

            var cargaAutomaticaInfo = db.CargaAutomatica.Where(c => c.WasLoaded == false).ToList();

            if (procesaCargaAutomatica == "1" && cargaAutomaticaInfo != null && cargaAutomaticaInfo.Count() > 0)
            {
                //var registrosCargaAutomatica = cargaAutomaticaInfo.ToList();
                bool hayArchivo = false;
                //foreach (CargaAutomatica cargaAutomatica in registrosCargaAutomatica)
                //{
                //    if (cargaAutomatica.WasLoaded == false)
                //    {
                //        this.lstCargaAutomatica.Add(cargaAutomatica);
                //    }
                //}

                //if (lstCargaAutomatica != null && lstCargaAutomatica.Count > 0)
                //{
                foreach (CargaAutomatica cargaAutomatica in cargaAutomaticaInfo)
                {                    
                    CargarArchivo cargarArchivo = new CargarArchivo();
                    string fechaExtencion = cargaAutomatica.FechaProgramada.Year.ToString() + cargaAutomatica.FechaProgramada.ToString("MM") + cargaAutomatica.FechaProgramada.Day.ToString("00") + ".DAT";

                    if (cargaAutomatica.TipoArchivo == (int)EnumTipoArchivoCarga.Balance)
                    {
                        nombreArchivo = ConfigurationManager.AppSettings["nombreArchivoBalance"] + fechaExtencion;
                        ruta = Path.Combine(cargaAutomatica.RutaArchivo, nombreArchivo);
                        if (File.Exists(ruta))
                        {
                            result = File.ReadAllText(ruta);
                            errores = cargarArchivo.CargarArchivoBD(nombreArchivo, result, EnumTipoArchivoCarga.Balance, 1);
                            hayArchivo = true;
                        }
                        else
                        {
                            hayArchivo = false;
                        }
                    }
                    else // Si el archivo es intercompañias
                    {
                        nombreArchivo = ConfigurationManager.AppSettings["nombreArchivoIntercomania"] + fechaExtencion;
                        ruta = Path.Combine(cargaAutomatica.RutaArchivo, nombreArchivo);
                        if (File.Exists(ruta))
                        {
                            result = File.ReadAllText(ruta);
                            errores = cargarArchivo.CargarArchivoBD(nombreArchivo, result, EnumTipoArchivoCarga.Intercompanias, 1);
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
                        mailInfo.Message = string.Format(ConfigurationManager.AppSettings["emailMessage"], nombreArchivo);
                        AdmMail.Enviar(mailInfo);
                    }
                    else if (errores != string.Empty)
                    {
                        Log.WriteLog("El archivo : " + nombreArchivo + " presentó los siguientes errores: " + errores, EnumTypeLog.Event, true);
                        mailInfo.Subject = ConfigurationManager.AppSettings["emailSubject"];
                        mailInfo.To = cargaAutomatica.Email.Replace(",", ";").Split(';').ToList();
                        mailInfo.Message = "El archivo : " + nombreArchivo + " presentó los siguientes errores: " + errores; ;
                        AdmMail.Enviar(mailInfo);
                    }
                }
                //}
            }
        }
    }
}