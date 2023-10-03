using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using VideoLibrary;
using System.Text.RegularExpressions;

string token = "6381980918:AAEHmy2aqymErt48pvloEqSFMQXaTRxv1pE";
var botClient = new TelegramBotClient(token);

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    pollingErrorHandler: HandlePollingErrorAsync,
    updateHandler: HandleUpdateAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{message.Chat.FirstName}' message in chat {chatId}.");

    Console.WriteLine(message.Chat.Username);



    if (messageText != null)
    {
        if (messageText == "/start")
            await botClient.SendTextMessageAsync(chatId, text: $"Hello {message.Chat.LastName} {message.Chat.FirstName}");
        if (await IsUrlValid(messageText) && messageText.Contains("/p/"))
        {

            try
            {
                var replasedMessage = messageText.Replace("www.", "dd");
                Message message1 = await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromUri($"{replasedMessage}"),
                        caption: "<b>Ara bird</b>. <i> Sourse</i>:<a href=\"https://t.me/letsCreateFirst_bot\">FirstBot</a>",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken
                    );
                Console.WriteLine("Yeeeessss");
            }
            catch { Console.WriteLine("Nooooooo1"); }
        }
        if (await IsUrlValid(messageText) && messageText.Contains("/reel/"))
        {
            try
            {
                var replasedMessage = messageText.Replace("www.", "dd");
                Console.WriteLine($"{replasedMessage}");

                Message msg = await botClient.SendVideoAsync(
                    chatId: chatId,
                    video: InputFile.FromUri($"{replasedMessage}"),
                    supportsStreaming: true,
                    caption: "<b>Ara bird</b>. <i> Sourse</i>:<a href=\"https://t.me/letsCreateFirst_bot\">FirstBot</a>",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
                Console.WriteLine("Yeeeessss");
            }
            catch { Console.WriteLine("Nooooo2"); }
        }
    }



}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine("Error=======>\n" + ErrorMessage + "\n<=====End");
    return Task.CompletedTask;
}
async Task<bool> IsUrlValid(string url)
{

    string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
    Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    return reg.IsMatch(url);
}