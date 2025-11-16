using System.Text.Json.Serialization;

namespace Max.Bot.Types.Requests;

/// <summary>
/// Represents a request to delete a webhook.
/// </summary>
public class DeleteWebhookRequest
{
    /// <summary>
    /// Gets or sets a value indicating whether to drop pending updates when deleting the webhook.
    /// </summary>
    /// <value>True to drop pending updates; otherwise, false.</value>
    [JsonPropertyName("drop_pending_updates")]
    public bool? DropPendingUpdates { get; set; }
}

