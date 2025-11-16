// 📁 [MaxClient] - Главный клиент библиотеки Max Messenger Bot API
// 🎯 Core function: Точка входа для взаимодействия с Max Messenger Bot API
// 🔗 Key dependencies: Max.Bot.Api, Max.Bot.Configuration, Max.Bot.Networking, Max.Bot.Exceptions
// 💡 Usage: Основной класс для работы с Max Messenger Bot API

using System.Net.Http;
using Microsoft.Extensions.Logging;
using Max.Bot.Api;
using Max.Bot.Configuration;
using Max.Bot.Exceptions;
using Max.Bot.Networking;

namespace Max.Bot;

/// <summary>
/// Main client for interacting with the Max Messenger Bot API.
/// </summary>
public class MaxClient : IMaxBotApi
{
    private readonly IMaxHttpClient _httpClient;
    private readonly MaxBotOptions _options;

    /// <inheritdoc />
    public IBotApi Bot { get; }

    /// <inheritdoc />
    public IMessagesApi Messages { get; }

    /// <inheritdoc />
    public IChatsApi Chats { get; }

    /// <inheritdoc />
    public IUsersApi Users { get; }

    /// <inheritdoc />
    public IFilesApi Files { get; }

    /// <inheritdoc />
    public ISubscriptionsApi Subscriptions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxClient"/> class with a bot token.
    /// </summary>
    /// <param name="token">The bot token for authentication.</param>
    /// <exception cref="ArgumentException">Thrown when token is null or empty, or options are invalid.</exception>
    public MaxClient(string token)
        : this(new MaxBotOptions { Token = token })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxClient"/> class with bot options.
    /// </summary>
    /// <param name="options">The bot options containing token and base URL.</param>
    /// <exception cref="ArgumentNullException">Thrown when options is null.</exception>
    /// <exception cref="ArgumentException">Thrown when options are invalid.</exception>
    public MaxClient(MaxBotOptions options)
        : this(options, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxClient"/> class with bot options and optional dependencies.
    /// </summary>
    /// <param name="options">The bot options containing token and base URL.</param>
    /// <param name="httpClient">Optional HTTP client. If null, a new HttpClient will be created.</param>
    /// <param name="logger">Optional logger for logging events.</param>
    /// <exception cref="ArgumentNullException">Thrown when options is null.</exception>
    /// <exception cref="ArgumentException">Thrown when options are invalid.</exception>
    public MaxClient(MaxBotOptions options, HttpClient? httpClient, ILogger<MaxClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _options.Validate();

        // Create or use provided HttpClient
        var client = httpClient ?? new HttpClient();

        // Configure MaxBotClientOptions with token in BaseUrl
        var clientOptions = new MaxBotClientOptions
        {
            BaseUrl = $"{_options.BaseUrl.TrimEnd('/')}/{_options.Token}",
            Timeout = TimeSpan.FromSeconds(30),
            RetryCount = 3
        };
        clientOptions.Validate();

        // Create MaxHttpClient
        // Note: ILogger<MaxHttpClient> requires ILoggerFactory to create
        // If logging is needed, pass ILoggerFactory to MaxClient constructor or use DI
        // For now, we pass null - MaxHttpClient will work without logging
        _httpClient = new MaxHttpClient(client, clientOptions, logger: null);

        // Initialize API groups
        Bot = new BotApi(_httpClient, _options);
        Messages = new MessagesApi(_httpClient, _options);
        Chats = new ChatsApi(_httpClient, _options);
        Users = new UsersApi(_httpClient, _options);
        Files = new FilesApi(_httpClient, _options);
        Subscriptions = new SubscriptionsApi(_httpClient, _options);
    }
}

