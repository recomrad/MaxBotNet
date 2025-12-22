using System.Text.Json.Serialization;

namespace Max.Bot.Types;

/// <summary>
/// Represents the recipient of a message (can be a chat or user).
/// </summary>
public class MessageRecipient
{
    /// <summary>
    /// Gets or sets the chat ID of the recipient.
    /// For direct messages (dialogs), this can be null or 0.
    /// </summary>
    /// <value>The unique identifier of the chat, or null/0 for direct messages.</value>
    [JsonPropertyName("chat_id")]
    public long? ChatId { get; set; }

    /// <summary>
    /// Gets or sets the type of the chat.
    /// </summary>
    /// <value>The type of the chat (e.g., "dialog", "group", "channel").</value>
    [JsonPropertyName("chat_type")]
    public string? ChatType { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the recipient (for dialogs).
    /// </summary>
    /// <value>The unique identifier of the user, or null if not applicable.</value>
    [JsonPropertyName("user_id")]
    public long? UserId { get; set; }
}

