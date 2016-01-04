using Ranken.ISC.Contracts.Repositories;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles;
using RankenData.InterfacesSAPCognos.Consola.FileMethods.WriteFiles;
using RankenData.InterfacesSAPCognos.Domain;
using RankenData.InterfacesSAPCognos.Model;
using RankenData.InterfacesSAPCognos.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankenData.InterfacesSAPCognos.Consola
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateFileBalance();

            //LoadFileAsync_MEX_SALINT();
            //LoadFile_MEX_SALINT();
            //LoadFile_MEXSALCTA();
            
            //GetCuentasCognos();
            //InsertCuentaCognos();

            Console.ReadLine();
        }

        private static void CreateFileBalance()
        {
            List<ArchivoResultado> file = new List<ArchivoResultado>();
            Encabezados(ref file);
        }

        private static void Encabezados(ref List<ArchivoResultado> file)
        {
            ArchivoResultado fileRowHeader1 = new ArchivoResultado()
            {
                Company = string.Empty

                
        //public string Period;

       
        //public string Actuality;

     
        //public string Account;

      
        ////Counter-Company
        //public string CounterCompany;

      
        //public string Dim1;

      
        //public string Dim2;

      
        //public string Dim3;

     
        ////IT Opex
        //public string ITOpex;

       
        //public string Amount;

      
        ////Transaction Currency
        //public string TransactionCurrency;

      
        ////Transaction amount
        //public string TransactionAmount;

        //[FieldOrder(13)]
        //public string Form;

        //[FieldOrder(14)]
        ////Account name
        //public string AccountName;

        //[FieldOrder(15)]
        //public string Retrieve;

        //[FieldOrder(16)]
        //public string Variance;
    };
        }

        private static void LoadFileAsync_MEX_SALINT()
        {
            DAT_Reader dAT_Reader = new FileMethods.ReadFiles.DAT_Reader();
            string path = @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALINT_20151211.DAT";
            try
            {
                dAT_Reader.StartReadingAsync_MEX_SALINT(path);
            }
            catch (Exception ex)
            {
                string fieldName = ((FileHelpers.ConvertException)ex).FieldName;
                string lineNumber = ((FileHelpers.ConvertException)ex).LineNumber.ToString();
                Console.WriteLine("lineNumber: " + lineNumber + " - " + "FieldName: " + fieldName + " - " + ex.Message);
            }
        }

        private static void LoadFile_MEX_SALINT()
        {
            DAT_Reader dAT_Reader = new FileMethods.ReadFiles.DAT_Reader();
            string path = @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALINT_20151211.DAT";
            try
            {
                dAT_Reader.StartReading_MEX_SALINT(path);
            }
            catch (Exception ex)
            {
                string fieldName = ((FileHelpers.ConvertException)ex).FieldName;
                string lineNumber = ((FileHelpers.ConvertException)ex).LineNumber.ToString();
                Console.WriteLine("lineNumber: " + lineNumber + " - " + "FieldName: " + fieldName + " - " + ex.Message);
            }

        }

        private static void LoadFile_MEXSALCTA()
        {
            DAT_Reader dAT_Reader = new FileMethods.ReadFiles.DAT_Reader();
            string path = @"D:\SkyDrive\Empleos\08.1 Raken Data Group\Farmacias Benavides\MEX_SALCTA_20151211.DAT";
            dAT_Reader.StartReading_MEXSALCTA(path);
        }

        private static void GetCuentasCognos()
        {
            //1. Agregando un nuevo Modelo 
            //InterfasSAPCognosEntities db = new InterfasSAPCognosEntities();
            //var cuentaCognos = db.CuentaCognos;
            //var cuentaCognos2 = cuentaCognos.ToList();

            ////2. Agregando los repositosios
            //CuentaCognosRepository cuentaCognos = new CuentaCognosRepository(new InterfasSAPCognosEntities());
            //var valor = cuentaCognos.GetAll();

            ////3, Agregando las Interfaces de los repositorios
            IRepository<CuentaCognos> db = new CuentaCognosRepository(new InterfasSAPCognosEntities());
            var valor2 = db.GetAll();
            var valor3 = db.GetAll().ToList();
        }

        private static void InsertCuentaCognos()
        {
            var cuentaCognos = new CuentaCognos()
            {
                Numero = "123",
                Descripcion = "Cuenta 123",
                Anexo = new Anexo()
                {
                    id = 1,
                    Clave = "2",
                    Descripcion = "Otro"                
                }
            };

            using (var context = new InterfasSAPCognosEntities())
            {
                try
                {
                    context.CuentaCognos.Add(cuentaCognos);
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                ve.PropertyName,
                                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                ve.ErrorMessage);
                        }
                    }
                    //throw;
                }
               
            }
        }

       
    }
}
