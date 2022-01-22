using Newtonsoft.Json;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace WeatherForecast
{
    internal class TelegramBotHelper
    {
        string token;
        TelegramBotClient client;
        string nameCity = "";
        float tempOfCity = 0;
        string nameOfCity = "";
        string answerOnWeather;

        public TelegramBotHelper(string token)
        {
            this.token = token;
        }

        internal void GetUpdates()
        {
            client = new TelegramBotClient(token)
            {
                Timeout = TimeSpan.FromSeconds(10)
                //если бот используется уже на каком-нибудь сервере и не нагружать сервера телеграм и не забанили}
            };

            var me = client.GetMeAsync().Result; //выводим данные, чтоб проверить запустился или нет
            Console.WriteLine($"Bot Id: {me.Id} \nBot name: {me.FirstName}");
            if (me != null && !string.IsNullOrEmpty(me.Username))
            {
                int offset = 0;
                while (true)
                {
                    try
                    {
                        var updates = client.GetUpdatesAsync(offset).Result;
                        if (updates != null && updates.Count() > 0)
                        {
                            foreach (var update in updates)
                            {
                                Console.WriteLine(update.Type);
                                if (update.Type == UpdateType.Message)
                                {
                                    nameCity = update.Message.Text;
                                    Weather(nameCity);
                                    Celcius(tempOfCity);
                                    client.SendTextMessageAsync(update.Message.Chat.Id, $"{answerOnWeather} \nТемпература в {nameOfCity}: {Math.Round(tempOfCity)} С");
                                    Console.WriteLine(update.Message.Text);
                                }
                                offset = update.Id + 1;//зацикливается на одном отправленном сообщении - без +1
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        //client.StartReceiving();
        //client.OnMessage += Bot_OnMessage;
        //Console.ReadLine();
        //client.StopReceiving();

        async void Bot_OnMessage(object? sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message.Type == MessageType.Text)
            {
                nameCity = message.Text;
                Weather(nameCity);
                Celcius(tempOfCity);
                await client.SendTextMessageAsync(message.Chat.Id, $"{answerOnWeather} \nТемпература в {nameOfCity}: {Math.Round(tempOfCity)} С");
                Console.WriteLine(message.Text);
            }
        }

        void Weather(string cityName)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid=" + "";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                nameOfCity = weatherResponse.Name;
                tempOfCity = weatherResponse.Main.Temp - 273;
            }
            catch (WebException ex)
            {
                Console.WriteLine("Возникло исключение " + ex.Message);
            }
        }

        void Celcius(float celcius)
        {
            if (celcius <= 10)
                answerOnWeather = "Сегодня холодно - одевайся потеплее!";
            else
                answerOnWeather = "Сегодня тепло - одевайся легко!";
        }

    }
}
