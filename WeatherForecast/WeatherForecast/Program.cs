using WeatherForecast;

const string token = "";

try
{
    TelegramBotHelper hlp = new TelegramBotHelper(token: token);
    hlp.GetUpdates();

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

