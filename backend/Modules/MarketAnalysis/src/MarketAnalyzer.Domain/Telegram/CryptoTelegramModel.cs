
namespace MarketAnalyzer.Domain
{
    public class CryptoTelegramModel
    {
        public async Task Send(CryptoTelegramParameterModel model)
        {
            var telegram = new TelegramModel();

            var take = 5;

            //var broker = Enum.GetName(typeof(BrokerEnum), (int)model.Broker);
            var market = Enum.GetName(typeof(MarketType), (int)model.Market);

            var header = $"📅 UTC Date : " + model.Date.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine +
                         $"📅 Tehran Date : " + model.Date.AddHours(3).AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine +
                         $"🏦 Exchenge : ( #{market} ) " + Environment.NewLine + Environment.NewLine + Environment.NewLine +
                         @"     📣     📣     📣     📣     📣     📣     📣" + Environment.NewLine + Environment.NewLine + Environment.NewLine;

            var mod = model.Signals.Count % take;
            var cou = model.Signals.Count / take;
            var sendText = "";

            if (model.Signals.Count > take)
            {
                for (int i = 0; i < cou; i++)
                {
                    var body = CreateBody(model.Signals.Skip(i * take).Take(take).ToList());
                    sendText = body;

                    if (i == 0)
                    {
                        sendText = header + body;
                    }

                    await telegram.Send(sendText, model.Timeframe);
                }
            }
            else
            {
                var body = CreateBody(model.Signals);
                sendText = header + body;
                await telegram.Send(sendText, model.Timeframe);
            }

        }

        private string CreateBody(List<SignalModel> model)
        {
            var text = "";
            //var position = "";

            //foreach (var item in model)
            //{
            //    if (item.TriggerPosition == PositionEnum.Long)
            //        position = $"⬆️ Trade Side : 🟢 {item.TriggerPosition} ";
            //    else
            //        position = $"⬇️ Trade Side : 🔴 {item.TriggerPosition} ";

            //    text += position + Environment.NewLine +
            //    $"💰 Coin : #{item.Symbol}" + Environment.NewLine + Environment.NewLine +
            //    $"📌 Entry : {item.SignalPrice.ToString("0.00000")}" + Environment.NewLine +
            //    $"🚫 Stoploss : {item.StopLoss.ToString("0.00000")}" + Environment.NewLine +
            //    $"🚫 Stoploss2 : {item.StopLoss2.ToString("0.00000")}" + Environment.NewLine +
            //    $"🚫 StopLossPercent : {item.StopLossPercent.ToString("0.00")} %" + Environment.NewLine +
            //    $"🚫 StopLossPercent2 : {item.StopLossPercent2.ToString("0.00")} %" + Environment.NewLine +
            //    $"🎯 TP1 : {item.TakeProfit_1.ToString("0.00000")}" + Environment.NewLine +
            //    $"🎖 TP2 : {item.TakeProfit_2.ToString("0.00000")}" + Environment.NewLine +
            //    $"🎉 TP3 : {item.TakeProfit_3.ToString("0.00000")}" + Environment.NewLine + Environment.NewLine + Environment.NewLine+

            //    @"     ✨     ✨     ✨     ✨     ✨" + Environment.NewLine + Environment.NewLine + Environment.NewLine;
            //}

            return text;
        }


    }
}
