using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class TimerCargaAutomatica
    {
        private EntitiesRakenData db = new EntitiesRakenData();
        List<CargaAutomatica> lstCargaAutomatica = null;
        DAT_Reader datReader = new DAT_Reader();
        MailInfo mailInfo = new MailInfo();
        string ruta;

        /// <summary>
        /// Inicalizar timer
        /// </summary>
        public void Init()
        {
            
            //mailInfo.Subject = "prueba 5";
            //mailInfo.To = new List<string>() { "mmbatu@hotmail.com", "mmbattou@gmail.com" };
                       
            //mailInfo.Message = "prueba de mensaje 555";
            //AdmMail.Enviar(mailInfo);

            //string tiempoCargaAutomatica = ConfigurationManager.AppSettings["tiempoCargaAutomatica"];
            //System.Timers.Timer timer = new System.Timers.Timer();
            //int time = int.Parse(tiempoCargaAutomatica);
            //time = time < 0 ? 1 : time; //minimo cada hora
            //// time = time * 3600000;
            //time = 30000; //TODO: Esta linea es de pruebas
            //timer.Interval = time;
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            //timer.Start();
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
            if (ConfigurationManager.AppSettings["procesaCargaAutomatica"] == "1")
            {
                foreach (CargaAutomatica cargaAutomatica in db.CargaAutomatica.ToList())
                {
                    if (cargaAutomatica.FechaProgramada.Date == DateTime.Now.Date)
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
                            nombreArchivo = ConfigurationManager.AppSettings["nombreArchivoBalance"] + cargaAutomatica.FechaProgramada.Year.ToString() + cargaAutomatica.FechaProgramada.Month.ToString() + cargaAutomatica.FechaProgramada.Day.ToString() + ".DAT";
                            ruta = Path.Combine(cargaAutomatica.RutaArchivo, nombreArchivo);
                            result = System.IO.File.ReadAllText(ruta);
                            errores = CargarArchivo.CargarArchivoBD("nombreArchivo", result, EnumTipoArchivoCarga.Balance);
                        }
                        else // Si el archivo es intercompañias
                        {
                            nombreArchivo = ConfigurationManager.AppSettings["nombreArchivoIntercomania"] + cargaAutomatica.FechaProgramada.Year.ToString() + cargaAutomatica.FechaProgramada.Month.ToString() + cargaAutomatica.FechaProgramada.Day.ToString() + ".DAT";
                            ruta = Path.Combine(cargaAutomatica.RutaArchivo, nombreArchivo);
                            result = System.IO.File.ReadAllText(@"C:\Users\mgonzalez\Documents\ProyectoLuisF\prueba.DAT");
                            errores = CargarArchivo.CargarArchivoBD("nombreArchivo", result, EnumTipoArchivoCarga.Intercompanias);
                        }

                        if (string.IsNullOrEmpty(errores))
                        {
                            //TODO: guardar que ya se subio en la base de datos y tener el correo para enviar el resultado
                            //cargaAutomatica.
                            //db.Entry(cargaAutomatica).State = EntityState.Modified;
                            //db.SaveChanges();

                            Log.WriteLog("El archivo : " + nombreArchivo + " se cargo exitosamente", EnumTypeLog.Event, true);

                            mailInfo.Subject = "prueba 5";
                            //mailInfo.To = cargaAutomatica.correo.Replace(",", ";").Split(';');

                            mailInfo.Message = "prueba de mensaje 555";
                            AdmMail.Enviar(mailInfo);



                        }
                        else
                        {
                            Log.WriteLog("El archivo : " + nombreArchivo + " el archivo presento presento los siguientes errores: " + errores, EnumTypeLog.Event, true);
                        }
                    }
                }
            }
            //TODO: enviar correo electronico
        }
    }
}