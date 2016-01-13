using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.Web.Models.Entidades
{
    public class Result
    {

        public bool WasSuccessful { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}