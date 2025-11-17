// СЂСџвЂњРѓ [MaxBotClientOptions] - Р С›Р С—РЎвЂ Р С‘Р С‘ Р Т‘Р В»РЎРЏ HTTP Р С”Р В»Р С‘Р ВµР Р…РЎвЂљР В°
// СЂСџР‹Р‡ Core function: Р С™Р С•Р Р…РЎвЂћР С‘Р С–РЎС“РЎР‚Р В°РЎвЂ Р С‘РЎРЏ HTTP Р С”Р В»Р С‘Р ВµР Р…РЎвЂљР В° (retry policy, РЎвЂљР В°Р в„–Р СР В°РЎС“РЎвЂљРЎвЂ№, Р В»Р С•Р С–Р С‘РЎР‚Р С•Р Р†Р В°Р Р…Р С‘Р Вµ)
// СЂСџвЂќвЂ” Key dependencies: System
// СЂСџвЂ™РЋ Usage: Р СњР В°РЎРѓРЎвЂљРЎР‚Р С•Р в„–Р С”Р В° Р С—Р С•Р Р†Р ВµР Т‘Р ВµР Р…Р С‘РЎРЏ MaxHttpClient РЎвЂЎР ВµРЎР‚Р ВµР В· MaxBotClientOptions

namespace Max.Bot.Configuration;

/// <summary>
/// Options for configuring the Max Bot HTTP client.
/// </summary>
public class MaxBotClientOptions
{
    /// <summary>
    /// Gets or sets the base URL of the Max Bot API.
    /// </summary>
    /// <value>The base URL of the API (e.g., "https://api.max.ru/bot").</value>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timeout for HTTP requests.
    /// </summary>
    /// <value>The timeout for requests. Default is 30 seconds.</value>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the number of retry attempts for failed requests.
    /// </summary>
    /// <value>The number of retry attempts. Default is 3.</value>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base delay for exponential backoff retry mechanism.
    /// </summary>
    /// <value>The base delay for retry attempts. Default is 1 second.</value>
    public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the maximum delay for exponential backoff retry mechanism.
    /// </summary>
    /// <value>The maximum delay for retry attempts. Default is 30 seconds.</value>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets a value indicating whether detailed logging is enabled.
    /// </summary>
    /// <value>True if detailed logging is enabled (logs request/response body); otherwise, false. Default is false.</value>
    public bool EnableDetailedLogging { get; set; } = false;

    /// <summary>
    /// Validates the options.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when BaseUrl is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when Timeout, RetryCount, RetryBaseDelay, or MaxRetryDelay have invalid values.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ArgumentException("BaseUrl cannot be null or empty.", nameof(BaseUrl));
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) || !uri.IsAbsoluteUri || string.IsNullOrEmpty(uri.Scheme))
        {
            throw new ArgumentException("BaseUrl must be a valid absolute URI.", nameof(BaseUrl));
        }

        if (Timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(Timeout), "Timeout must be greater than zero.");
        }

        if (RetryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(RetryCount), "RetryCount cannot be negative.");
        }

        if (RetryBaseDelay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(RetryBaseDelay), "RetryBaseDelay must be greater than zero.");
        }

        if (MaxRetryDelay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxRetryDelay), "MaxRetryDelay must be greater than zero.");
        }

        if (MaxRetryDelay < RetryBaseDelay)
        {
            throw new ArgumentException("MaxRetryDelay must be greater than or equal to RetryBaseDelay.", nameof(MaxRetryDelay));
        }
    }
}

