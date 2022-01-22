using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

internal class TelegramBotHelper
{
    const string TEXT_1 = "1";
    const string TEXT_2 = "2";
    const string TEXT_3 = "3";
    const string TEXT_4 = "4";
    const string TEXT_5 = "All";

    string token;
    TelegramBotClient client;

    public TelegramBotHelper(string token)
    {
        this.token = token;
    }

    internal void GetUpdates()
    {
        client = new TelegramBotClient(token);
        var me = client.GetMeAsync().Result;
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
                            processUpdate(update);
                            offset = update.Id + 1;
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

    async void processUpdate(Update update)
    {
        switch (update.Type)
        {
            case Telegram.Bot.Types.Enums.UpdateType.Message:
                var text = update.Message.Text;
                string imagePath = null;
                switch (text)
                {
                    case TEXT_1:
                        imagePath = Path.Combine(Environment.CurrentDirectory, "cat1.jpg");
                        using (var stream = System.IO.File.OpenRead(imagePath))
                        {
                            var r = client.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), caption: "<i> First Cat </i>", replyMarkup: GetInlineButtons(1), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html).Result;
                        }
                        break;
                    case TEXT_2:
                        //imagePath = Path.Combine(Environment.CurrentDirectory, "cat2.jpeg");
                        //using (var stream = File.OpenRead(imagePath))
                        //{
                        //    var r = client.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), caption: "2", replyMarkup: GetInlineButtons(2)).Result;
                        //}
                        var res = client.SendStickerAsync(update.Message.Chat.Id, sticker: "https://cdn.tlgrm.app/stickers/f5c/8b4/f5c8b42c-29be-4e9c-ad42-a7982218957e/thumb-animated-128.mp4", replyMarkup: GetInlineButtons(2)).Result;
                        break;
                    case TEXT_3:
                        imagePath = Path.Combine(Environment.CurrentDirectory, "cat3.jpg");
                        using (var stream = System.IO.File.OpenRead(imagePath))
                        {
                            var r = client.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), caption: "<i> Third cat </i>", replyMarkup: GetInlineButtons(3), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html).Result;
                        }
                        break;
                    case TEXT_4:
                        imagePath = Path.Combine(Environment.CurrentDirectory, "cat4.jpg");
                        using (var stream = System.IO.File.OpenRead(imagePath))
                        {
                            var r = client.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), caption: "<i> Fourth cat </i>", replyMarkup: GetInlineButtons(4), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html).Result;
                        }
                        break;
                    case TEXT_5:
                        await client.SendMediaGroupAsync(
                        chatId: update.Message.Chat.Id,
                        media: new InputMediaPhoto[]
                        {
                            new InputMediaPhoto(new InputMedia(System.IO.File.OpenRead(Path.Combine(Environment.CurrentDirectory, "cat1.jpg")),"name1")),
                            new InputMediaPhoto(new InputMedia(System.IO.File.OpenRead(Path.Combine(Environment.CurrentDirectory, "cat3.jpg")),"name2")),
                            new InputMediaPhoto(new InputMedia(System.IO.File.OpenRead(Path.Combine(Environment.CurrentDirectory, "cat4.jpg")),"name3")),
                        }
                        );
                        break;
                    default:
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Receive text: " + text, replyMarkup: GetButtons());
                        break;
                }
                break;
            case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                switch (update.CallbackQuery.Data)
                {
                    case "1":
                        var msg1 = client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Заказ принят {update.CallbackQuery.Data}", replyMarkup: GetButtons()).Result;
                        break;
                    case "2":
                        var msg2 = client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Заказ принят {update.CallbackQuery.Data}", replyMarkup: GetButtons()).Result;
                        break;
                    case "3":
                        var msg3 = client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Заказ принят {update.CallbackQuery.Data}", replyMarkup: GetButtons()).Result;
                        break;
                    case "4":
                        var msg4 = client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Заказ принят {update.CallbackQuery.Data}", replyMarkup: GetButtons()).Result;
                        break;
                }
                break;
            default:
                Console.WriteLine(update.Type + " not implemented ");
                break;
        }
    }

    IReplyMarkup? GetInlineButtons(int id)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Заказать", id.ToString()));
    }

    IReplyMarkup? GetButtons()
    {
        return new ReplyKeyboardMarkup(

           keyboard: new KeyboardButton[][]
            {
                new KeyboardButton[] { new KeyboardButton( TEXT_1), new KeyboardButton ( TEXT_2 ) },
                new KeyboardButton[] { new KeyboardButton( TEXT_3 ), new KeyboardButton ( TEXT_4 ), new KeyboardButton(TEXT_5) }
            })
        { ResizeKeyboard = true };
    }
}
