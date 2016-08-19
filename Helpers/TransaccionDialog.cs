using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotApplicationSolidario.Helpers
{
    [LuisModel("32a84403-df3b-4a88-bae3-db1e92da28f1", "fe4a45e7422d404487636f2a76cab2cf")]
    [Serializable]
    class TransaccionDialog : LuisDialog<Transaccion>
    {
        private readonly BuildFormDelegate<Transaccion> TransaccionForm;

        internal TransaccionDialog(BuildFormDelegate<Transaccion> transaccionForm)
        {
            this.TransaccionForm = transaccionForm;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Lo sentimos, no podemos entender la transacción");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Movimientos")]
        public async Task ProcesarMovimientos(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Lo sentimos, no podemos entender la transacción Movimientos");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Saldo")]
        public async Task ProcesaSaldo(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Lo sentimos, no podemos entender la transacción Saldos");
            context.Wait(MessageReceived);
        }

        [LuisIntent("TransaccionMovil")]
        public async Task ProcessTransaccionForm(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            if (!entities.Any((entity) => entity.Type == "Transaccion"))
            {
                foreach (var entity in result.Entities)
                {
                    string transaccion = null;
                    switch (entity.Type)
                    {
                        case "Saldo": transaccion = "Saldo"; break;
                        case "Movimientos": transaccion = "Movimientos"; break;
                        case "TransferenciaDirecta": transaccion = "Transferencia Directa"; break;
                        case "TransferenciaInterbancaria": transaccion = "Transferencia Interbancaria"; break;
                        default:
                            break;
                    }
                    if (transaccion != null)
                    {
                        entities.Add(new EntityRecommendation(type: "Transaccion") { Entity = transaccion });
                        break;
                    }
                }
            }
            var transaccionForm = new FormDialog<Transaccion>(new Transaccion(), this.TransaccionForm, FormOptions.PromptInStart, entities);
            context.Call<Transaccion>(transaccionForm, TransaccionComplete);
        }

        private async Task TransaccionComplete(IDialogContext context, IAwaitable<Transaccion> result)
        {
            Transaccion transaccion = null;
            try
            {
                transaccion = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Has cancelado la transaccón!");
                return;
            }

            if (transaccion != null)
            {
                await context.PostAsync("tu transacción: " + transaccion.ToString());
            }
            else
            {
                await context.PostAsync("No se ha obtenido ninguna respuesta!");
            }

            context.Wait(MessageReceived);
        }
    }
}