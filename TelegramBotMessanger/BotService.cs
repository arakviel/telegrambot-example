using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace TelegramBotMessanger;

public class BotService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _botToken;

    public BotService(string apiUrl, string botToken)
    {
        _httpClient = new HttpClient();
        _apiUrl = apiUrl;
        _botToken = botToken;
    }

    public async Task SendMessageAsync(string chatId, string message)
    {
        var requestUri = $"{_apiUrl}/bot{_botToken}/sendMessage";
        var payload = new
        {
            chat_id = chatId,
            text = message,
            parse_mode = "MarkdownV2",
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUri, content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Message sent successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response content: {responseContent}");
        }
    }

    public async Task SendPhotoAsync(string chatId, string photoPath, string caption = null)
    {
        var requestUri = $"{_apiUrl}/bot{_botToken}/sendPhoto";

        using (var form = new MultipartFormDataContent())
        {
            form.Add(new StringContent(chatId), "chat_id");

            if (caption != null)
            {
                form.Add(new StringContent(caption), "caption");
            }

            var imageContent = new ByteArrayContent(File.ReadAllBytes(photoPath));
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            form.Add(imageContent, "photo", Path.GetFileName(photoPath));

            var response = await _httpClient.PostAsync(requestUri, form);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Photo sent successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to send photo. Status code: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response content: {responseContent}");
            }
        }
    }

    public async Task StartReceivingMessagesAsync()
    {
        int offset = 0;

        while (true)
        {
            var requestUri = $"{_apiUrl}/bot{_botToken}/getUpdates?offset={offset}";
            var response = await _httpClient.GetStringAsync(requestUri);
            var updates = JArray.Parse(JObject.Parse(response)["result"].ToString());

            foreach (var update in updates)
            {
                var updateId = update["update_id"].Value<int>();
                offset = updateId + 1;

                var message = update["message"];
                if (message != null)
                {
                    var chatId = message["chat"]["id"].Value<string>();
                    var text = message["text"].Value<string>();
                    Console.WriteLine($"Received message from {chatId}: {text}");
                }
            }

            await Task.Delay(1000); // Затримка перед наступним запитом
        }
    }
}
