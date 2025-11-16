using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Max.Bot.Configuration;
using Max.Bot.Networking;
using Max.Bot.Types;
using Max.Bot.Types.Requests;

namespace Max.Bot.Api;

/// <summary>
/// Implementation of subscriptions/updates-related API methods.
/// </summary>
internal class SubscriptionsApi : BaseApi, ISubscriptionsApi
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionsApi"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    /// <param name="options">The bot options containing token and base URL.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or options is null.</exception>
    public SubscriptionsApi(IMaxHttpClient httpClient, MaxBotOptions options)
        : base(httpClient, options)
    {
    }

    /// <inheritdoc />
    public async Task<Subscription[]> GetSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        // Response structure: { "subscriptions": [...] }
        // Different from standard Response<T> wrapper
        var response = await HttpClient.SendAsync<SubscriptionsResponse>(
            CreateRequest(HttpMethod.Get, "/subscriptions", null),
            cancellationToken).ConfigureAwait(false);

        if (response == null || response.Subscriptions == null)
        {
            throw new Exceptions.MaxApiException(
                "API request failed. The response indicates an error or contains no data.",
                null,
                System.Net.HttpStatusCode.BadRequest);
        }

        return response.Subscriptions;
    }

    /// <inheritdoc />
    public async Task<Response> SetWebhookAsync(SetWebhookRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var queryParams = new Dictionary<string, string?>
        {
            { "url", request.Url }
        };

        if (request.DropPendingUpdates.HasValue)
        {
            queryParams["drop_pending_updates"] = request.DropPendingUpdates.Value.ToString().ToLowerInvariant();
        }

        var apiRequest = CreateRequest(HttpMethod.Post, "/subscriptions", null, queryParams);
        return await ExecuteRequestAsync<Response>(apiRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Response> DeleteWebhookAsync(DeleteWebhookRequest? request = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>();

        if (request?.DropPendingUpdates.HasValue == true)
        {
            queryParams["drop_pending_updates"] = request.DropPendingUpdates.Value.ToString().ToLowerInvariant();
        }

        var apiRequest = CreateRequest(HttpMethod.Delete, "/subscriptions", null, queryParams.Count > 0 ? queryParams : null);
        return await ExecuteRequestAsync<Response>(apiRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<WebhookInfo> GetWebhookInfoAsync(CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(HttpMethod.Get, "/subscriptions", null);
        return await ExecuteRequestAsync<WebhookInfo>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<GetUpdatesResponse> GetUpdatesAsync(GetUpdatesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var queryParams = new Dictionary<string, string?>();

        if (request.Limit.HasValue)
        {
            queryParams["limit"] = request.Limit.Value.ToString();
        }

        if (request.Timeout.HasValue)
        {
            queryParams["timeout"] = request.Timeout.Value.ToString();
        }

        if (request.Marker.HasValue)
        {
            queryParams["marker"] = request.Marker.Value.ToString();
        }

        if (request.Types != null && request.Types.Count > 0)
        {
            queryParams["types"] = string.Join(",", request.Types);
        }

        var apiRequest = CreateRequest(HttpMethod.Get, "/updates", null, queryParams.Count > 0 ? queryParams : null);
        return await ExecuteRequestAsync<GetUpdatesResponse>(apiRequest, cancellationToken).ConfigureAwait(false);
    }
}

