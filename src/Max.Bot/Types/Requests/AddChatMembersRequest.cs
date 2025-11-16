// 📁 [AddChatMembersRequest] - Запрос на добавление участников в чат
// 🎯 Core function: Представляет запрос для добавления участников в чат
// 🔗 Key dependencies: System.Text.Json.Serialization, System.ComponentModel.DataAnnotations
// 💡 Usage: Используется в ChatsApi.AddChatMembersAsync для добавления участников

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Max.Bot.Types.Requests;

/// <summary>
/// Represents a request to add members to a chat.
/// </summary>
public class AddChatMembersRequest
{
    /// <summary>
    /// Gets or sets the array of user IDs to add to the chat.
    /// </summary>
    /// <value>
    /// The array of user IDs. Must not be null or empty.
    /// </value>
    [Required(ErrorMessage = "User IDs array is required.")]
    [MinLength(1, ErrorMessage = "At least one user ID must be provided.")]
    [JsonPropertyName("user_ids")]
    public long[] UserIds { get; set; } = Array.Empty<long>();
}

