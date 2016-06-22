using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers.Utilidades
{
    public static class ManejoErrores
    {
        /// <summary>
        /// Error en validacion de las entidades del modelo
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ErrorValidacion(DbEntityValidationException e) 
        {
            StringBuilder errores = new StringBuilder();
            var errorMessages = e.EntityValidationErrors
                           .SelectMany(x => x.ValidationErrors)
                           .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(e.Message, " Las validaciones del error son: ", fullErrorMessage);
            errores.AppendLine(exceptionMessage);
            return errores.ToString();
        }

        public static string ErrorValidacionDb(DbUpdateException e)
        {
            string inner = string.Empty;

            if( e.InnerException != null )
            {
                if (e.InnerException.InnerException != null)
                {
                    if (e.InnerException.InnerException.InnerException != null)
                    {
                        inner = e.InnerException.InnerException.InnerException.Message;
                    }
                    else
                    {
                        inner = e.InnerException.InnerException.Message;
                    }
                }
                else 
                {
                    inner = e.InnerException.Message;
                }
            } 
                
            return "ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message + " Exepción: " + inner; 
        }

        public static string ErrorExepcion(Exception e)
        {
            string inner = string.Empty;

            if (e.InnerException != null)
            {
                if (e.InnerException.InnerException != null)
                {
                    if (e.InnerException.InnerException.InnerException != null)
                    {
                        inner = e.InnerException.InnerException.InnerException.Message;
                    }
                    else
                    {
                        inner = e.InnerException.InnerException.Message;
                    }
                }
                else
                {
                    inner = e.InnerException.Message;
                }
            } 
            return "ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + e.Message + " Exepción: " + inner; 
        }

        public static string ErrorExepcion(string info,Exception e)
        {
            string inner = string.Empty;

            if (e.InnerException != null)
            {
                if (e.InnerException.InnerException != null)
                {
                    if (e.InnerException.InnerException.InnerException != null)
                    {
                        inner = e.InnerException.InnerException.InnerException.Message;
                    }
                    else
                    {
                        inner = e.InnerException.InnerException.Message;
                    }
                }
                else
                {
                    inner = e.InnerException.Message;
                }
            }
            return "ERROR AL ESCRIBIR EN LA BASE DE DATOS: " + Environment.NewLine + 
                info + Environment.NewLine +
                " Mensaje: " + e.Message + Environment.NewLine +
                " Exepción: " + inner;
        }


    }
}