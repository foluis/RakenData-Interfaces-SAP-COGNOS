using FileHelpers;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranken.ISC.FileManager.ReadFiles
{
    public class DAT_Reader
    {
        public MEXSALCTA[] StartReading_MEXSALCTA(string path)
        {
            var engine = new FileHelperEngine<MEXSALCTA>();

            //path = path == string.Empty ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALCTA_20151211.DAT" : path;

            //var result1 = engine.ReadFile(path);
            MEXSALCTA[] result = engine.ReadFile(path);

            //foreach (MEXSALCTA file in result1)
            //{
            //    int anio = file.Anio;
            //    string compania = file.Compania;
            //    decimal MovimientoDebitoPeriodo = file.MovimientoDebitoPeriodo;
            //    DateTime horaActualizacion = file.HoraActualizacion;
            //}

            return result;
        }

        public MEX_SALINT[] StartReading_MEX_SALINT(string path)
        {
            var engine = new FileHelperEngine<MEX_SALINT>();

            //path = path == string.Empty ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALINT_20151211.DAT" : path;
            var result = engine.ReadFile(path);

            //foreach (MEX_SALINT file in result)
            //{
            //    int anio = file.Anio;
            //    string compania = file.Compania;

            //    decimal MovimientoDebitoPeriodo = file.MovimientoDebitoPeriodo;
            //    DateTime horaActualizacion = file.HoraActualizacion;
            //}

            return result;
        }

        public List<MEX_SALINT> StartReadingAsync_MEX_SALINT(string path)
        {
            FileHelperAsyncEngine engine = new FileHelperAsyncEngine(typeof(MEX_SALINT));
            //path = path == string.Empty ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALINT_20151211.DAT" : path;
            var result = engine.BeginReadFile(path);           

            List<MEX_SALINT> file = new List<MEX_SALINT>();

            foreach (MEX_SALINT fileRow in engine)
            {
                file.Add(fileRow);
               
                //Console.WriteLine(fileRow.Compania);

                //recordCount++;
                //if (recordCount > 100)
                //    break; 
            }

            engine.Close();

            return file;
        }
    }
}
