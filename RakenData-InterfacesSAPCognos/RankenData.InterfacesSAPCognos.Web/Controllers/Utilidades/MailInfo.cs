using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades
{
    public class MailInfo
    {
        /// <summary>
        /// Ruta, Nombre y extencion de los archivos que se desean enviar junto al correo
        /// </summary>
        /// <remarks>
        /// C:\Users\lforero\Desktop\samplaFile.txt
        ///</remarks>
        public List<Attachment> Attachment { get; set; }

        /// <summary>
        /// Direcciones de correo a las cuales se les quiere enviar una copia oculta del mensaje
        /// </summary>
        public List<string> Bcc { get; set; }

        /// <summary>
        /// Direcciones de correo a las cuales se les quiere enviar una copia del correo
        /// </summary>
        public List<string> Cc { get; set; }

        /// <summary>
        /// Direccion de corrreo de donde se envia el mensaje
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Identifica si se va a usar el formato HTML para el envio del correo o no
        /// </summary>
        public bool? IsHTMLFormat { get; set; }

        /// <summary>
        /// Contenido del mensaje que se quiere enviar
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Identifica la prioridad del mensaje
        /// </summary>
        public MailPriority? Priority { get; set; }

        /// <summary>
        /// Objeto o tema del mensaje de correo
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Direccion de correo a donde se va a enviar el mensaje
        /// </summary>
        public List<string> To { get; set; }
    }
}