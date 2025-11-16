// 📁 [BaseApi] - Базовый класс для API классов
// 🎯 Core function: Общая логика для всех API классов (построение URL, заголовки, обработка Response<T>)
// 🔗 Key dependencies: Max.Bot.Networking, Max.Bot.Configuration, Max.Bot.Types, Max.Bot.Exceptions
// 💡 Usage: Базовый класс для BotApi, MessagesApi, ChatsApi, UsersApi

using System.Net;
using System.Net.Http;
using Max.Bot.Configuration;
using Max.Bot.Exceptions;
using Max.Bot.Networking;
using Max.Bot.Types;

namespace Max.Bot.Api;

/// <summary>
/// Base class for API classes that provides common functionality.
/// </summary>
internal abstract class BaseApi
{
    /// <summary>
    /// Gets the HTTP client for making API requests.
    /// </summary>
    protected readonly IMaxHttpClient HttpClient;

    /// <summary>
    /// Gets the bot options containing token and base URL.
    /// </summary>
    protected readonly MaxBotOptions Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApi"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    /// <param name="options">The bot options containing token and base URL.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or options is null.</exception>
    protected BaseApi(IMaxHttpClient httpClient, MaxBotOptions options)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Builds the endpoint URL with token included in the path.
    /// </summary>
    /// <param name="path">The endpoint path (e.g., "/me", "/messages").</param>
    /// <returns>The full endpoint path with token (e.g., "/{token}/me").</returns>
    /// <exception cref="ArgumentException">Thrown when path is null or empty.</exception>
    protected string BuildEndpoint(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        // Ensure path starts with '/'
        var normalizedPath = path.StartsWith('/') ? path : $"/{path}";

        // Build endpoint with token: /{token}{path}
        // Example: /{token}/me, /{token}/messages
        return $"/{Options.Token}{normalizedPath}";
    }

    /// <summary>
    /// Creates a MaxApiRequest with Authorization header and proper endpoint.
    /// </summary>
    /// <param name="method">The HTTP method (GET, POST, PUT, DELETE, etc.).</param>
    /// <param name="endpoint">The endpoint path relative to base URL (without token).</param>
    /// <param name="body">The request body object to be serialized to JSON. Can be null.</param>
    /// <param name="queryParams">The query parameters dictionary. Can be null.</param>
    /// <returns>A MaxApiRequest ready to be sent.</returns>
    protected MaxApiRequest CreateRequest(
        HttpMethod method,
        string endpoint,
        object? body = null,
        Dictionary<string, string?>? queryParams = null)
    {
        var request = new MaxApiRequest
        {
            Method = method,
            Endpoint = BuildEndpoint(endpoint),
            Body = body,
            QueryParameters = queryParams
        };

        // Add Authorization header
        request.Headers ??= new Dictionary<string, string?>();
        request.Headers["Authorization"] = $"Bearer {Options.Token}";

        return request;
    }

    /// <summary>
    /// Executes a request and handles Response&lt;T&gt; wrapper.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="request">The API request to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unwrapped response data.</returns>
    /// <exception cref="MaxApiException">Thrown when the API returns an error response (ok: false or result is null).</exception>
    /// <exception cref="MaxNetworkException">Thrown when a network error occurs.</exception>
    /// <exception cref="MaxUnauthorizedException">Thrown when authentication fails.</exception>
    protected async Task<T> ExecuteRequestAsync<T>(
        MaxApiRequest request,
        CancellationToken cancellationToken = default)
    {
        // Special case: if T is Response, deserialize directly without Response<T> wrapper
        if (typeof(T) == typeof(Response) || typeof(T).IsAssignableFrom(typeof(Response)))
        {
            var simpleResponse = await HttpClient.SendAsync<Response>(request, cancellationToken).ConfigureAwait(false);
            if (simpleResponse == null)
            {
                throw new MaxApiException(
                    "API request returned null response.",
                    null,
                    HttpStatusCode.BadRequest);
            }
            return (T)(object)simpleResponse;
        }

        var wrappedResponse = await HttpClient.SendAsync<Response<T>>(request, cancellationToken).ConfigureAwait(false);

        if (!wrappedResponse.Ok || wrappedResponse.Result == null)
        {
            throw new MaxApiException(
                "API request failed. The response indicates an error or contains no data.",
                null,
                HttpStatusCode.BadRequest);
        }

        return wrappedResponse.Result;
    }

    /// <summary>
    /// Validates that a chat ID is greater than zero.
    /// </summary>
    /// <param name="chatId">The chat ID to validate.</param>
    /// <param name="paramName">The name of the parameter (default: "chatId").</param>
    /// <exception cref="ArgumentException">Thrown when chatId is less than or equal to zero.</exception>
    protected static void ValidateChatId(long chatId, string paramName = "chatId")
    {
        if (chatId <= 0)
        {
            throw new ArgumentException("Chat ID must be greater than zero.", paramName);
        }
    }

    /// <summary>
    /// Validates that a user ID is greater than zero.
    /// </summary>
    /// <param name="userId">The user ID to validate.</param>
    /// <param name="paramName">The name of the parameter (default: "userId").</param>
    /// <exception cref="ArgumentException">Thrown when userId is less than or equal to zero.</exception>
    protected static void ValidateUserId(long userId, string paramName = "userId")
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.", paramName);
        }
    }

    /// <summary>
    /// Validates that a string parameter is not null or empty.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <exception cref="ArgumentException">Thrown when value is null or empty.</exception>
    protected static void ValidateNotEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
        }
    }
}

