using System.Text.Json.Serialization;

namespace Max.Bot.Types;

/// <summary>
/// Represents a subscription to updates.
/// </summary>
public class Subscription
{
    /// <summary>
    /// Gets or sets the URL where updates will be sent.
    /// </summary>
    /// <value>The webhook URL.</value>
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}

