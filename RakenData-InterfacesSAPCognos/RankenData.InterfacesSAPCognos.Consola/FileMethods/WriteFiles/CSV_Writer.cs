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
        public void StartWritingArchivoBalance(string anio_2dig, string mes, string path, string sociedad)
        {
            List<ArchivoResultado> archivoResultado = new List<ArchivoResultado>();
            PrepareSpetialHeader(ref archivoResultado, anio_2dig, mes, sociedad);
            //var engine = new FileHelperAsyncEngine<ArchivoResultado>();

            ////engine.HeaderText = "COLUMN1|COLUMN2|COLUMN3|...";

            //path = path == "" ? @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\" : path;
            //fileNname = fileNname == "" ? "Archivo resultado Test.csv" : fileNname;

            //var finalPath = path + fileNname;

            //using (engine.BeginWriteFile(finalPath))
            //{
            //    foreach (ArchivoResultado cust in archivoResultadoList)
            //    {
            //        engine.WriteNext(cust);
            //    }
            //}
        }

        public  void PrepareSpetialHeader(ref List<ArchivoResultado> archivoResultado, string anio_2dig, string mes,string sociedad)
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
                Retrieve = anio_2dig,
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


            archivoResultado.Add(fila_Period_ArchivoResultado);
            archivoResultado.Add(fila_Interval_ArchivoResultado);
            archivoResultado.Add(fila_Actuality_ArchivoResultado);
            archivoResultado.Add(fila_Currency_ArchivoResultado);
            archivoResultado.Add(fila_Plataforma_ArchivoResultado);
            archivoResultado.Add(fila_Metodo_ArchivoResultado);
            archivoResultado.Add(fila_Tipo_ArchivoResultado);
            archivoResultado.Add(fila_Sociedad_ArchivoResultado);
            archivoResultado.Add(fila_Space_ArchivoResultado);
            archivoResultado.Add(fila_Header_ArchivoResultado);
        }
    }
}
