try
{
    TelegramBotHelper hlp = new TelegramBotHelper(token: "5115177343:AAH4frzIv0KuCvTKrcOH-GiTB1G5cdwj6iI");
    hlp.GetUpdates();
    
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}