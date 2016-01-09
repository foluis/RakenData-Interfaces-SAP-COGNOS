using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Controllers
{
    public static class ManejoErrores
    {
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
    }
}