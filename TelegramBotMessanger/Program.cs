namespace TelegramBotMessanger;

internal class Program
{
    static async Task Main(string[] args)
    {
        string apiUrl = "https://api.telegram.org"; // або інший URL API
        string botToken = "7222071956:AAEZnXyviuhKLsqxs1PCdiISqVEX04Jy1fc";
        string chatId = "-1001862802902";
        string message = $@"
*bold \*text*
_italic \*text_
__underline__
~strikethrough~
";



        BotService botService = new BotService(apiUrl, botToken);
        //await botService.SendMessageAsync(chatId, message);


        string photoPath = "cat.jpg";
        string caption = "This is a photo caption";

        // Надсилаємо фотографію
        //await botService.SendPhotoAsync(chatId, photoPath, caption);


        await botService.StartReceivingMessagesAsync();
    }
}
