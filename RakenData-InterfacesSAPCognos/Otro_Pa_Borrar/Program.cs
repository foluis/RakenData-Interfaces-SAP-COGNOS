using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otro_Pa_Borrar
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio");
            Debug.Write("Debug.Write Funciona");

            DateTime date = DateTime.Now;
            Console.WriteLine(date);
            var formatedDate = date.ToString("yyyy/dd/MM");
            Console.WriteLine(formatedDate);

            Console.ReadLine();
        }
    }
}
