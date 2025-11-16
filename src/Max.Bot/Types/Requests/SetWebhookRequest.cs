using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Max.Bot.Types.Requests;

/// <summary>
/// Represents a request to set a webhook.
/// </summary>
public class SetWebhookRequest
{
    /// <summary>
    /// Gets or sets the URL where updates will be sent.
    /// </summary>
    /// <value>The webhook URL (must be HTTPS).</value>
    [Required]
    [Url(ErrorMessage = "URL must be a valid HTTPS URL.")]
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether to drop pending updates when setting the webhook.
    /// </summary>
    /// <value>True to drop pending updates; otherwise, false.</value>
    [JsonPropertyName("drop_pending_updates")]
    public bool? DropPendingUpdates { get; set; }
}

