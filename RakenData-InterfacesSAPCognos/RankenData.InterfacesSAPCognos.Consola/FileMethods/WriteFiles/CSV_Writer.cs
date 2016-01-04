using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankenData.InterfacesSAPCognos.Consola.FileMethods.WriteFiles
{
    public class CSV_Writer
    {
        public void StartWritingArchivoBalance(string path, string fileNname, List<ArchivoResultado> archivoResultadoList)
        {
            var engine = new FileHelperAsyncEngine<ArchivoResultado>();
            
            //engine.HeaderText = "COLUMN1|COLUMN2|COLUMN3|...";

            path = path == string.Empty ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\" : path;
            fileNname = fileNname == string.Empty ? "Archivo resultado Test.csv" : fileNname;

            var finalPath = path + fileNname;

            using (engine.BeginWriteFile(finalPath))
            {
                foreach (ArchivoResultado cust in archivoResultadoList)
                {
                    engine.WriteNext(cust);
                }
            }
        }
    }
}
