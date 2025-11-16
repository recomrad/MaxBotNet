// 📁 [SendChatActionRequest] - Запрос на отправку действия в чат
// 🎯 Core function: Представляет запрос для отправки действия бота в чат
// 🔗 Key dependencies: System.Text.Json.Serialization, Max.Bot.Types.Enums
// 💡 Usage: Используется в ChatsApi.SendChatActionAsync для отправки действий

using System.Text.Json.Serialization;
using Max.Bot.Types.Enums;

namespace Max.Bot.Types.Requests;

/// <summary>
/// Represents a request to send a chat action.
/// </summary>
public class SendChatActionRequest
{
    /// <summary>
    /// Gets or sets the action to send.
    /// </summary>
    /// <value>The chat action to send.</value>
    [JsonPropertyName("action")]
    public ChatAction Action { get; set; }
}

