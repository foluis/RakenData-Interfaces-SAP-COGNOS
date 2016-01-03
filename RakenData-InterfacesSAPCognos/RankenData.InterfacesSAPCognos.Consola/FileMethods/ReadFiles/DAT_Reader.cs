using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles
{
    public class DAT_Reader
    {
        public void StartReading_MEXSALCTA(string path)
        {
            var engine = new FileHelperEngine<MEXSALCTA>();

            path = path == string.Empty ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALCTA_20151211.DAT" : path;
            var result = engine.ReadFile(path);

            foreach (MEXSALCTA file in result)
            {
                int anio = file.Anio;
                string compania = file.Compania;
                decimal MovimientoDebitoPeriodo = file.MovimientoDebitoPeriodo;
                DateTime horaActualizacion = file.HoraActualizacion;
            }
        }

        public void StartReading_MEX_SALINT(string path)
        {
            var engine = new FileHelperEngine<MEX_SALINT>();

            path = path == string.Empty ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALINT_20151211.DAT" : path;
            var result = engine.ReadFile(path);

            foreach (MEX_SALINT file in result)
            {
                int anio = file.Anio;
                string compania = file.Compania;

                decimal MovimientoDebitoPeriodo = file.MovimientoDebitoPeriodo;
                DateTime horaActualizacion = file.HoraActualizacion;
            }
        }

        public void StartReadingAsync_MEX_SALINT(string path)
        {
            FileHelperAsyncEngine engine = new FileHelperAsyncEngine(typeof(MEX_SALINT));
            path = path == string.Empty ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALINT_20151211.DAT" : path;
            var result = engine.BeginReadFile(path);

            int recordCount = 0;

            foreach (MEX_SALINT cust in engine)
            {
                // your code here 
                Console.WriteLine(cust.Compania);

                recordCount++;
                if (recordCount > 100)
                    break; // stop processing 
            }

            engine.Close();
        }
    }
}
