using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BotApplicationSolidario.Helpers
{
    [Serializable]
    class ConsultaSaldo
    {
        [Prompt("¿Que cuenta desea consultar? {||}")]
        [Describe("Número Cuenta")]
        public string NumeroCuenta;
    };

    [Serializable]
    class MovimientoCuenta
    {
        [Prompt("¿Que cuenta desea consultar? {||}")]
        [Describe("Número Cuenta")]
        public string NumeroCuenta;

        [Numeric(0,20)]
        [Prompt("¿Que cantidad de movimientos desea consultar? {||}")]
        [Describe("Cantidad Movimientos")]
        [Optional]
        public int? CantidadMovimientos;
    };

    [Serializable]
    class TransferenciasDirectas
    {
        [Prompt("¿Cual es la cuenta de origen? {||}")]
        [Describe("Cuenta origen")]
        public string NumeroCuenta;

        [Prompt("¿Cual es el valor de la transacción? {||}")]
        [Describe("valor $")]
        public string Valor;

        [Prompt("¿Cual es la cuenta beneficiaria? {||}")]
        [Describe("Cuenta beneficiaria")]
        public string CuentraDestino;

        [Optional]
        public string CorreoDestino;
    };

    [Serializable]
    class TransferenciasInterbancarias
    {
        [Prompt("¿Cual es la cuenta de origen? {||}")]
        [Describe("Cuenta origen")]
        public string NumeroCuenta;

        [Prompt("¿Cual es la cuenta de destino? {||}")]
        [Describe("Cuenta destino")]
        public string CuentaDestino;

        [Prompt("¿Cual es el valor de la transacción? {||}")]
        [Describe("Valor $")]
        public string Valor;

        [Prompt("¿Banco destinatario? {||}")]
        [Describe("Banco Destino")]
        public BancoDestinoOptions BancoDestino;

        [Prompt("¿Identificación del beneficiario? {||}")]
        [Describe("Identificación")]
        public string Identificacion;

        [Optional]
        public string CorreoDestino;
    };

    [Serializable]
    class Recarga
    {
        [Prompt("¿Cual es el valor de la transacción? {||}")]
        [Describe("Valor $")]
        public string Valor;

        [Pattern("(\\(\\d{3}\\))?\\s*\\d{3}(-|\\s*)\\d{4}")]
        [Prompt("¿Cual es el número de la recarga? {||}")]
        [Describe("Número")]
        [Optional]
        public string NumeroRecarga;
    };

    public enum TipoTransaccion
    {
        Unknown,
        [Terms(new string[] { "saldo", "Consulta de saldos" })]
        Saldo,
        [Terms(new string[] { "mov", "movimiento de cuentas" })]
        Movimientos,
        [Terms(new string[] { "trf", "Transferencia directa" })]
        TransferenciaDirecta,
        [Terms(new string[] { "trfi", "Transferencia interbancaria" })]
        TransferenciaInterbancaria,
        [Terms(new string[] { "rec", "Recraga tiempo aire" })]
        Recarga
    };

    public enum ValorImporte
    {
        _2,
        _3,
        _6,
        _10
    };

    public enum BancoDestinoOptions
    {
        Unknown,
        [Terms(new string[] { "pch", "Banco del Pichincha","Pichincha" })]
        Pichincha,
        [Terms(new string[] { "pcf", "Panco del Pacifico","Pacifico" })]
        Pacifico,
        [Terms(new string[] { "prd", "Produbanco" })]
        Produbanco,
        [Terms(new string[] { "gye", "Banco Guayaquil","Guayaquil" })]
        Guayaquil,
        [Terms(new string[] { "int", "Banco Internacional","internacional" })]
        Internacional
    }

    [Serializable]
    class Transaccion
    {
        TipoTransaccion _transaccion;
        ConsultaSaldo _Saldo;
        MovimientoCuenta _Movimientos;
        TransferenciasDirectas _TransD;
        TransferenciasInterbancarias _TransIner;
        Recarga _Recarga;

        [Prompt("¿Que transacción deseas realizar? {||}")]
        [Template(TemplateUsage.NotUnderstood, "Que significa \"{0}\"???")]
        [Describe("Tipo de transacción")]
        public TipoTransaccion TipoTransaccion;
        public ConsultaSaldo Saldo;
        public MovimientoCuenta Movimientos;
        public TransferenciasDirectas TransD;
        public TransferenciasInterbancarias TransIner;
        public Recarga Recarga;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("({0}, ", TipoTransaccion);
            switch (TipoTransaccion)
            {
                #region Switch
                case TipoTransaccion.Movimientos:
                    if (Movimientos.CantidadMovimientos>0)
                    {
                        builder.AppendFormat("de la Cuenta {0}", Movimientos.NumeroCuenta);
                    }
                    else
                    {
                        builder.AppendFormat("de la Cuenta {0}, los últimos {1} registros", Movimientos.NumeroCuenta, Movimientos.CantidadMovimientos);
                    }
                    break;
                case TipoTransaccion.Recarga:
                    if (string.IsNullOrEmpty(Recarga.NumeroRecarga))
                    {
                        builder.AppendFormat(" el valor de ${0} a mi número por defecto", Recarga.Valor);
                    }
                    else
                    {
                        builder.AppendFormat(" el valor de ${0} al número:{1}", Recarga.Valor,Recarga.NumeroRecarga);
                    }
                    break;
                case TipoTransaccion.Saldo:
                    builder.AppendFormat(" de la Cienta N°:{0}", Saldo.NumeroCuenta);
                    break;
                case TipoTransaccion.TransferenciaDirecta:
                    if (string.IsNullOrEmpty(TransD.CorreoDestino))
                    {
                        builder.AppendFormat(" desde la cuenta N°{0} a la cuenta N° {1} el valor de $ {2}", TransD.NumeroCuenta, TransD.CuentraDestino, TransD.Valor);
                    }
                    else
                    {
                        builder.AppendFormat(" desde la cuenta N°{0} a la cuenta N° {1} el valor de $ {2}, notificar a: {3}", TransD.NumeroCuenta, TransD.CuentraDestino, TransD.Valor, TransD.CorreoDestino);
                    }
                    break;
                case TipoTransaccion.TransferenciaInterbancaria:
                    if (string.IsNullOrEmpty(TransIner.CorreoDestino))
                    {
                        builder.AppendFormat(" desde la cuenta N°{0} a la cuenta N° {1} del banco {2} el valor de $ {3}", TransIner.NumeroCuenta, TransIner.CuentaDestino,TransIner.BancoDestino, TransIner.Valor);
                    }
                    else
                    {
                        builder.AppendFormat(" desde la cuenta N°{0} a la cuenta N° {1} del banco {2} el valor de $ {3} y notificar a {4}", TransIner.NumeroCuenta, TransIner.CuentaDestino, TransIner.BancoDestino, TransIner.Valor, TransIner.CorreoDestino);
                    }
                    break;
                    #endregion
            }
            builder.AppendFormat(")");
            return builder.ToString();
        }
    }
}