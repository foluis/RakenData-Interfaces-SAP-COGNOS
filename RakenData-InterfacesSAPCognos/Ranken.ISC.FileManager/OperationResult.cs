using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranken.ISC.FileManager
{
    public class OperationResult
    {
        /// <summary>
        /// Resultado 0 = Exitoso, 1 = Fallido
        /// </summary>
        public int IdError { get; set; }

        /// <summary>
        /// Mensaje sujerido para el cliente
        /// </summary>
        public string UserError { get; set; }

        /// <summary>
        /// Excepcion
        /// </summary>
        public Exception Exception { get; set; }
    }
}
