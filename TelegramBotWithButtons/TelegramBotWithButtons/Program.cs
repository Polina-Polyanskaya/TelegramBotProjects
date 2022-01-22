try
{
    TelegramBotHelper hlp = new TelegramBotHelper(token: "");
    hlp.GetUpdates();

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}