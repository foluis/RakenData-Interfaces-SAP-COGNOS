using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranken.ISC.FileManager
{
    public class OperationResult
    {
        public int IdError { get; set; }
        public string UserError { get; set; }
        public Exception Exception { get; set; }
    }
}
