using Ranken.ISC.FileManager.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using RankenData.InterfacesSAPCognos.Web.Models;
using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public class TimerCargaAutomatica
    {
        private EntitiesRakenData db = new EntitiesRakenData();
        List<CargaAutomatica> lstCargaAutomatica = null;
        DAT_Reader datReader =  new DAT_Reader();

        /// <summary>
        /// Inicalizar timer
        /// </summary>
        public void Init()
        {
            string tiempoCargaAutomatica = ConfigurationManager.AppSettings["tiempoCargaAutomatica"];
            System.Timers.Timer timer = new System.Timers.Timer();
            int time = int.Parse(tiempoCargaAutomatica);
            time = time < 0 ? 1 : time; //minimo cada hora
           // time = time * 3600000;
            time = 30000;
            timer.Interval = time;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        /// <summary>
        /// Cada ciclo valida si hay archivos para cargar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            if (ConfigurationManager.AppSettings["procesaCargaAutomatica"] == "1")
            {
                this.lstCargaAutomatica = db.CargaAutomatica.Where(ca => DbFunctions.TruncateTime(ca.FechaProgramada) == DbFunctions.TruncateTime(DateTime.Now)).ToList();
                if (lstCargaAutomatica != null && lstCargaAutomatica.Count > 0)
                {
                    foreach (CargaAutomatica cargaAutomatica in lstCargaAutomatica)
                    {                        
                        string nombreArchivo = ConfigurationManager.AppSettings["procesaCargaAutomatica"] + cargaAutomatica.FechaProgramada.Year.ToString() + cargaAutomatica.FechaProgramada.Month.ToString() +cargaAutomatica.FechaProgramada.Day.ToString()+ ".DAT" ;
                        string result = System.IO.File.ReadAllText(@"C:\Users\mgonzalez\Documents\ProyectoLuisF\prueba.DAT");
                        string errores = CargarArchivo.CargarArchivoBD("nombreArchivo", result, EnumTipoArchivoCarga.Balance);                
                    }                //TODO: escribir en el log
                }
            }
            //TODO: enviar correo electronico
        }
    }
}