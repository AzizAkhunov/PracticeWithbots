using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using VideoLibrary;



string token = "6381980918:AAEHmy2aqymErt48pvloEqSFMQXaTRxv1pE";
var botClient = new TelegramBotClient(token);

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
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

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");


    //YouTube youTube = new YouTube();
    //var youTubeVideo = youTube.GetVideo(messageText).Stream();
    //await botClient.SendVideoAsync(
    //    chatId: chatId,
    //    video: InputFile.FromStream(youT ubeVideo),
    //    cancellationToken: cancellationToken);
    // Echo received message text


    //string replaceMessage = messageText.Replace("www.","dd");

    //Message msg = await botClient.SendVideoAsync(
    //    chatId:chatId,
    //    video:replaceMessage,
    //    supportsStreaming:true,
    //    cancellationToken:cancellationToken
    //    );

    string replacePhoto = messageText.Replace("www.", "dd");

    Message msg = await botClient.SendPhotoAsync(
            chatId:chatId,
            photo: replacePhoto,
            parseMode:ParseMode.Html,
            cancellationToken:cancellationToken
        );
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}