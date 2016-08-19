using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using BotApplicationSolidario.Helpers;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using System.Diagnostics;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Collections.Generic;


namespace BotApplicationSolidario
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public bool _XXx = false;
        private static IForm<Transaccion> BuildForm()
        {
            var builder = new FormBuilder<Transaccion>();
            ActiveDelegate<Transaccion> isMovimientos = (trasaccion) => trasaccion.TipoTransaccion == TipoTransaccion.Movimientos;
            ActiveDelegate<Transaccion> isRecarga = (trasaccion) => trasaccion.TipoTransaccion == TipoTransaccion.Recarga;
            ActiveDelegate<Transaccion> isSaldo = (trasaccion) => trasaccion.TipoTransaccion == TipoTransaccion.Saldo;
            ActiveDelegate<Transaccion> isTransferenciaDirecta = (trasaccion) => trasaccion.TipoTransaccion == TipoTransaccion.TransferenciaDirecta;
            ActiveDelegate<Transaccion> isTransferenciaInterbancaria = (trasaccion) => trasaccion.TipoTransaccion == TipoTransaccion.TransferenciaInterbancaria;
         
            #region Formulario
            builder
                     .Field(nameof(Transaccion.TipoTransaccion))
                     .Field(new FieldReflector<Transaccion>("Movimientos.NumeroCuenta")
                            .SetType(null)
                            .SetActive((state) => state.TipoTransaccion == TipoTransaccion.Movimientos)
                            .SetDefine(async (state, field) =>
                            {
                                if (state.TipoTransaccion == TipoTransaccion.Movimientos)
                                {
                                    foreach (var item in ConsultarCuentas(state, field))
                                    {
                                        field.AddDescription(item, item).AddTerms(item, item);
                                    }
                                }
                                return true;
                            }))
                     .Field("Movimientos.CantidadMovimientos", isMovimientos)

                     .Field(new FieldReflector<Transaccion>("Saldo.NumeroCuenta")
                            .SetType(null)                           
                            .SetActive((state) => state.TipoTransaccion == TipoTransaccion.Saldo)
                            .SetDefine(async (state, field) =>
                            {
                                var ss= field.Values;
                                if (state.TipoTransaccion == TipoTransaccion.Saldo)
                                {
                                    foreach (var item in ConsultarCuentas(state, field))
                                    {
                                        field.AddDescription(item, item).AddTerms(item, item);
                                    }
                                }
                                return true;
                            }))

                     .Field("Recarga.Valor", isRecarga)
                     .Field("Recarga.NumeroRecarga", isRecarga)

                     .Field(new FieldReflector<Transaccion>("TransD.NumeroCuenta")
                            .SetType(null)
                            .SetActive((state) => state.TipoTransaccion == TipoTransaccion.TransferenciaDirecta)
                            .SetDefine(async (state, field) =>
                            {
                                if (state.TipoTransaccion == TipoTransaccion.TransferenciaDirecta)
                                {
                                    foreach (var item in ConsultarCuentas(state, field))
                                    {
                                        field.AddDescription(item, item).AddTerms(item, item);
                                    }
                                }
                                return true;
                            }))
                     .Field("TransD.CuentraDestino", isTransferenciaDirecta)
                     .Field("TransD.Valor", isTransferenciaDirecta)
                     .Field("TransD.CorreoDestino", isTransferenciaDirecta)

                     .Field(new FieldReflector<Transaccion>("TransIner.NumeroCuenta")
                        .SetType(null)
                        .SetActive((state) => state.TipoTransaccion == TipoTransaccion.TransferenciaInterbancaria)
                        .SetDefine(async (state, field) =>
                        {
                            if (state.TipoTransaccion == TipoTransaccion.TransferenciaInterbancaria)
                            {
                                foreach (var item in ConsultarCuentas(state, field))
                                {
                                    field.AddDescription(item, item).AddTerms(item, item);
                                }
                            }
                            return true;
                        }))
                     .Field("TransIner.BancoDestino", isTransferenciaInterbancaria)
                     .Field("TransIner.CuentaDestino", isTransferenciaInterbancaria)
                     .Field("TransIner.Identificacion", isTransferenciaInterbancaria)
                     .Field("TransIner.Valor", isTransferenciaInterbancaria)
                     .Field("TransIner.CorreoDestino", isTransferenciaInterbancaria)

                     .AddRemainingFields()
                     .Confirm("¿Deseas consultar el saldo de la cuenta N° {Saldo.NumeroCuenta} ?", isSaldo)
                     .Confirm("¿Deseas realizar la recarga de ${Recarga.Valor} al número {Recarga.NumeroRecarga} ?", isRecarga)
                     .Confirm("¿Deseas consultar los últimos {Movimientos.CantidadMovimientos} movimientos de la cuenta {Movimientos.NumeroCuenta}  ?", isMovimientos)
                     .Confirm("¿Deseas realizar la transferencia desde la cuenta {TransD.NumeroCuenta} a la cuenta {TransD.CuentraDestino} el valor de ${TransD.Valor}? Notificar a:{TransD.CorreoDestino} ", isTransferenciaDirecta)
                     .Confirm("¿Deseas realizar la transferencia interbancaria desde la cuenta {TransIner.NumeroCuenta} a la cuenta {TransIner.CuentaDestino} del banco {TransIner.BancoDestino} con identificación {TransIner.Identificacion} el valor de ${TransIner.Valor}?. Notificar a: {TransIner.CorreoDestino}", isTransferenciaInterbancaria);
            #endregion
            return builder.Build();
        }

        private static List<string> ConsultarCuentas(Transaccion arg,Field<Transaccion> arg2)
        {
            List<string> _lista = new List<string>();
            _lista.Add("1111111");
            _lista.Add("2222222");
            _lista.Add("3333333");
            return  _lista;
        }

        internal static IDialog<Transaccion> MakeRoot()
        {
            return Chain.From(() => new TransaccionDialog(BuildForm));
        }

        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {

            if (activity != null)
            {
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        await Conversation.SendAsync(activity, MakeRoot);
                        break;

                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}