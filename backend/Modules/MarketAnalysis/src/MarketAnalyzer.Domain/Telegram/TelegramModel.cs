using System.Web;

namespace MarketAnalyzer.Domain
{
    public class TelegramModel
    {
        public async Task Send(string text, Timeframe timeFrame)
        {
            string apiToken = "1757482409:AAGjnJepWfzpbsMLuj8dgma8rC7vJLZwsKg";
            string chatId_15min = "-1001195406346";
            string chatId_1h = "-1001455506629";
            string chatId_1d = "-1001184870411";

            var tel_url = "";

            switch (timeFrame)
            {
                case Timeframe.Weekly:
                    tel_url = "";
                    break;
                case Timeframe.Daily:
                    tel_url = string.Format("https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text=",apiToken,chatId_1d);
                    break;
                case Timeframe.FourHours:
                    tel_url = "";
                    break;
                case Timeframe.OneHour:
                    tel_url = string.Format("https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text=", apiToken, chatId_1h);
                    break;
                case Timeframe.FifteenMinutes:
                    tel_url = string.Format("https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text=", apiToken, chatId_15min);
                    break;
                case Timeframe.FiveMinutes:
                    tel_url = "";
                    break;
            }

            var client = new HttpClient();
            text = HttpUtility.UrlEncode(text);

            var end_text = tel_url + text ;

            var request = await client.GetAsync(end_text);           
        }

    }
}
