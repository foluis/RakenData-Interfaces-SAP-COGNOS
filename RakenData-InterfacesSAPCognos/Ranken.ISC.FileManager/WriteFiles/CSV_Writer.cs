using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranken.ISC.FileManager.WriteFiles
{
    public class CSV_Writer
    {
        public void StartWritingArchivoBalance(string sociedad,string anio, string mes,int tipo, string path, List<ArchivoResultado> archivoResultadoBody)
        {
            List<ArchivoResultado> archivoResultadoHeader = new List<ArchivoResultado>();
            PrepareSpetialHeader(ref archivoResultadoHeader, anio, mes, sociedad);

            archivoResultadoHeader.AddRange(archivoResultadoBody);

            var engine = new FileHelperAsyncEngine<ArchivoResultado>();

            ////engine.HeaderText = "COLUMN1|COLUMN2|COLUMN3|...";

            path = path == "" ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\" : path;
            string fileNname = "Archivo resultado Test.csv" ;

            var finalPath = path + fileNname;

            using (engine.BeginWriteFile(finalPath))
            {
                foreach (ArchivoResultado cust in archivoResultadoHeader)
                {
                    engine.WriteNext(cust);
                }
            }
        }

        private  void PrepareSpetialHeader(ref List<ArchivoResultado> archivoResultadoHeader, string anio, string mes,string sociedad)
        {
            //SP consultar tablaCabecera

            ArchivoResultado fila_Period_ArchivoResultado = new ArchivoResultado()
            {
                Company = "",
                Period = "",
                Actuality = "",
                Account = "",
                CounterCompany = "",
                Dim1 = "",
                Dim2 = "",
                Dim3 = "",
                ITOpex = "",
                Amount = "",
                TransactionCurrency = "",
                TransactionAmount = "",
                Form = "",
                AccountName = "Period",
                Retrieve = anio.Substring(2,2),
                Variance = ""
            };

            ArchivoResultado fila_Interval_ArchivoResultado = new ArchivoResultado()
            {                
                AccountName = "Interval",
                Retrieve = "YTD"                
            };

            ArchivoResultado fila_Actuality_ArchivoResultado = new ArchivoResultado()
            {              
                AccountName = "Actuality",
                Retrieve = "AC"
            };

            ArchivoResultado fila_Currency_ArchivoResultado = new ArchivoResultado()
            {               
                AccountName = "Currency",
                Retrieve = "MXN"
            };

            ArchivoResultado fila_Plataforma_ArchivoResultado = new ArchivoResultado()
            {               
                Retrieve = "MA"
            };

            ArchivoResultado fila_Metodo_ArchivoResultado = new ArchivoResultado()
            {               
                Retrieve = "REPO"
            };

            ArchivoResultado fila_Tipo_ArchivoResultado = new ArchivoResultado()
            {
                Retrieve = "BASE"
            };

            ArchivoResultado fila_Sociedad_ArchivoResultado = new ArchivoResultado()
            {
                AccountName = "Cognos Company number",
                Retrieve = sociedad
            };

            ArchivoResultado fila_Space_ArchivoResultado = new ArchivoResultado() { };

            ArchivoResultado fila_Header_ArchivoResultado = new ArchivoResultado()
            {
                Company = "Company",
                Period = "Period",
                Actuality = "Actuality",
                Account = "Account",
                CounterCompany = "Counter-Company",
                Dim1 = "Dim1",
                Dim2 = "Dim2",
                Dim3 = "Dim3",
                ITOpex = "IT Opex",
                Amount = "Amount",
                TransactionCurrency = "Transaction Currency",
                TransactionAmount = "Transaction amount",
                Form = "Form",
                AccountName = "Account name",
                Retrieve = "Retrieve",
                Variance = "Variance"
            };


            archivoResultadoHeader.Add(fila_Period_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Interval_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Actuality_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Currency_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Plataforma_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Metodo_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Tipo_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Sociedad_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Space_ArchivoResultado);
            archivoResultadoHeader.Add(fila_Header_ArchivoResultado);
        }
    }
}
