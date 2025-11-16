// 📁 [AddChatAdminRequest] - Запрос на назначение администратора чата
// 🎯 Core function: Представляет запрос для назначения администратора чата
// 🔗 Key dependencies: System.Text.Json.Serialization, System.ComponentModel.DataAnnotations
// 💡 Usage: Используется в ChatsApi.AddChatAdminAsync для назначения администратора

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Max.Bot.Types.Requests;

/// <summary>
/// Represents a request to add an admin to a chat.
/// </summary>
public class AddChatAdminRequest
{
    /// <summary>
    /// Gets or sets the user ID to make an admin.
    /// </summary>
    /// <value>
    /// The user ID. Must be greater than zero.
    /// </value>
    [Range(1, long.MaxValue, ErrorMessage = "User ID must be greater than zero.")]
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }
}

