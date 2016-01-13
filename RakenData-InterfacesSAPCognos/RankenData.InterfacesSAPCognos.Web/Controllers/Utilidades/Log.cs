using RankenData.InterfacesSAPCognos.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades
{
    public class Log
    {
        private static string date;
        private static string message;
        private static string time;
        private static string logPath;
        private static string writeLog;

        /// <summary>
        /// Write in the log de errores
        /// </summary>
        /// <param name="MensajeError"></param>
        public static void WriteLog(string menssageError, EnumTypeLog typeLog, bool addDate, Exception ex = null)
        {
            date = DateTime.Today.ToString("yyyy-MM-dd");
            time = "\t\t\t\t\t\tTime: " + DateTime.Now.ToString("HH:mm:ss");
            logPath = ConfigurationManager.AppSettings["logPath"];
            writeLog = ConfigurationManager.AppSettings["writeLog"];
            switch (typeLog)
            {
                case EnumTypeLog.Error:
                    //using (StreamWriter file = File.AppendText(GlobalInformation.LogPath + "/Log Error " + date + ".txt"))
                    using (StreamWriter file = File.AppendText(logPath + "/Log Error " + date + ".txt"))
                    {
                        string excepcion = string.Empty;
                        if (ex != null)
                        {
                            excepcion = ex.ToString();
                            if (ex.InnerException != null)
                            {
                                excepcion = excepcion + ex.InnerException.Message;
                            }
                        }
                                                
                        message = menssageError + "  Error: " + excepcion + time;
                        Console.WriteLine(message);
                        file.WriteLine(message);
                        file.WriteLine(string.Empty);
                        file.Close();
                    };
                    break;

                case EnumTypeLog.Event:

                    if (writeLog == "1")
                    {
                        using (StreamWriter file = File.AppendText(logPath + "/Log Event " + date + ".txt"))
                        {
                            string excepcion = string.Empty;
                            if (ex != null)
                            {
                                excepcion = ex.Message;
                                if (ex.InnerException != null)
                                {
                                    excepcion = excepcion + ex.InnerException.Message;
                                }
                            }

                            if (addDate)
                            {
                                message = menssageError + excepcion + time;
                                Console.WriteLine(message);
                                file.WriteLine(message);
                            }
                            else
                            {
                                Console.WriteLine(menssageError);
                                file.WriteLine(menssageError);
                            }

                            file.Close();
                        };
                    }
                    break;

                default:
                    break;
            }
        }
    }
}