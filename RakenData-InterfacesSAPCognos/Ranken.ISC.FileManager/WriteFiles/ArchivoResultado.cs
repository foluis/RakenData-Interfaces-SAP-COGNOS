using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranken.ISC.FileManager.WriteFiles
{
    [DelimitedRecord(",")]
    public class ArchivoResultado
    {
        [FieldOrder(1)]
        public string Company;

        [FieldOrder(2)]
        public string Period;

        [FieldOrder(3)]
        public string Actuality;

        [FieldOrder(4)]
        public string Account;

        [FieldOrder(5)]
        //Counter-Company
        public string CounterCompany;

        [FieldOrder(6)]
        public string Dim1;

        [FieldOrder(7)]
        public string Dim2;

        [FieldOrder(8)]
        public string Dim3;

        [FieldOrder(9)]
        //IT Opex
        public string ITOpex;

        [FieldOrder(10)]
        public string Amount;

        [FieldOrder(11)]
        //Transaction Currency
        public string TransactionCurrency;

        [FieldOrder(12)]
        //Transaction amount
        public string TransactionAmount;

        [FieldOrder(13)]
        public string Form;

        [FieldOrder(14)]
        //Account name
        public string AccountName;

        [FieldOrder(15)]
        public string Retrieve;

        [FieldOrder(16)]
        public string Variance;
    }
}
