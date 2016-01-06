﻿using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankenData.InterfacesSAPCognos.Consola.FileMethods.ReadFiles
{
    [FixedLengthRecord()]
    [IgnoreFirst]
    [IgnoreLast]
    public class MEXSALCTA
    {
        [FieldFixedLength(1)]
        [FieldTrim(TrimMode.Right)]
        public string Escenario;

        [FieldFixedLength(1)]
        [FieldTrim(TrimMode.Right)]
        public string Version;

        [FieldFixedLength(4)]
        public short Anio;

        [FieldFixedLength(2)]
        public byte Mes;

        [FieldFixedLength(2)]
        [FieldTrim(TrimMode.Right)]
        public string UnidadNegocio;

        [FieldFixedLength(15)]
        [FieldTrim(TrimMode.Right)]
        public string Cuenta;

        [FieldFixedLength(5)]
        [FieldTrim(TrimMode.Right)]
        public string Moneda;

        [FieldFixedLength(2)]
        [FieldTrim(TrimMode.Right)]
        public string GAAP;

        [FieldFixedLength(10)]
        [FieldTrim(TrimMode.Right)]
        public string Interfase;

        [FieldFixedLength(1)]
        public string NominalAjustado;

        [FieldFixedLength(14)]
        [FieldTrim(TrimMode.Right)]
        public string Compania;
        
        [FieldFixedLength(19)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal MovimientoDebitoPeriodo;
        
        [FieldFixedLength(19)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal MovimientoCreditoPeriodo;
        
        [FieldFixedLength(19)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal MovimientoDébitoAcumulado;
        
        [FieldFixedLength(19)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal MovimientoCréditoAcumulado;
        
        [FieldFixedLength(19)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal SaldoAcumuladoPeriodo;
        
        [FieldFixedLength(12)]
        [FieldConverter(ConverterKind.Date, "yyyyMMddHHmm")]
        public DateTime HoraActualizacion;

        [FieldFixedLength(10)]
        [FieldTrim(TrimMode.Right)]
        public string UsuarioSctualizacion;
    }
}
