using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Max.Bot.Configuration;
using Max.Bot.Exceptions;
using Max.Bot.Networking;
using Max.Bot.Types;
using Max.Bot.Types.Enums;

namespace Max.Bot.Api;

/// <summary>
/// Implementation of file-related API methods.
/// </summary>
internal class FilesApi : BaseApi, IFilesApi, IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesApi"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    /// <param name="options">The bot options containing token and base URL.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or options is null.</exception>
    public FilesApi(IMaxHttpClient httpClient, MaxBotOptions options)
        : base(httpClient, options)
    {
        // Create a separate HttpClient for file uploads (may need to upload to external URLs)
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10) // Longer timeout for file uploads
        };
    }

    /// <summary>
    /// Disposes the resources used by this instance.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
    }

    /// <inheritdoc />
    public async Task<UploadResponse> UploadFileAsync(UploadType uploadType, CancellationToken cancellationToken = default)
    {
        // Map enum to API string value
        var typeString = uploadType switch
        {
            UploadType.Image => "image",
            UploadType.Video => "video",
            UploadType.Audio => "audio",
            UploadType.File => "file",
            _ => throw new ArgumentException($"Unknown upload type: {uploadType}", nameof(uploadType))
        };

        var queryParams = new Dictionary<string, string?>
        {
            { "type", typeString }
        };

        var request = CreateRequest(HttpMethod.Post, "/uploads", null, queryParams);
        return await ExecuteRequestAsync<UploadResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<object> UploadFileDataAsync(string uploadUrl, Stream fileStream, string? fileName = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(uploadUrl))
        {
            throw new ArgumentException("Upload URL cannot be null or empty.", nameof(uploadUrl));
        }

        ArgumentNullException.ThrowIfNull(fileStream);

        if (!fileStream.CanRead)
        {
            throw new ArgumentException("File stream must be readable.", nameof(fileStream));
        }

        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        
        // Set content type if not already set
        if (string.IsNullOrEmpty(streamContent.Headers.ContentType?.MediaType))
        {
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        }

        content.Add(streamContent, "data", fileName ?? "file");

        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
        {
            Content = content
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new MaxApiException(
                $"File upload failed with status {response.StatusCode}: {errorBody}",
                null,
                response.StatusCode);
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        // Try to deserialize as JSON (for video/audio token or image/file response)
        try
        {
            return JsonSerializer.Deserialize<object>(responseBody) ?? new { };
        }
        catch (JsonException)
        {
            // If not JSON, return as string
            return responseBody;
        }
    }

    /// <inheritdoc />
    public async Task<object> UploadFileResumableAsync(string uploadUrl, Stream fileStream, long chunkSize = 1024 * 1024, string? fileName = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(uploadUrl))
        {
            throw new ArgumentException("Upload URL cannot be null or empty.", nameof(uploadUrl));
        }

        ArgumentNullException.ThrowIfNull(fileStream);

        if (!fileStream.CanRead)
        {
            throw new ArgumentException("File stream must be readable.", nameof(fileStream));
        }

        if (chunkSize <= 0)
        {
            throw new ArgumentException("Chunk size must be greater than zero.", nameof(chunkSize));
        }

        // For resumable upload, we need to upload in chunks
        // This is a simplified implementation - full resumable upload would require
        // tracking upload progress and resuming from last successful chunk
        var buffer = new byte[chunkSize];
        long totalBytesRead = 0;
        var fileLength = fileStream.Length;

        while (totalBytesRead < fileLength)
        {
            var bytesRead = await fileStream.ReadAsync(buffer, 0, (int)Math.Min(chunkSize, fileLength - totalBytesRead), cancellationToken).ConfigureAwait(false);
            
            if (bytesRead == 0)
            {
                break;
            }

            using var chunkStream = new MemoryStream(buffer, 0, bytesRead);
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(chunkStream);
            
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            content.Add(streamContent, "data", fileName ?? "file");

            // Add range headers for resumable upload
            using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
            {
                Content = content
            };

            request.Headers.Add("Content-Range", $"bytes {totalBytesRead}-{totalBytesRead + bytesRead - 1}/{fileLength}");

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new MaxApiException(
                    $"File upload chunk failed with status {response.StatusCode}: {errorBody}",
                    null,
                    response.StatusCode);
            }

            totalBytesRead += bytesRead;
        }

        // Read final response (should contain token for video/audio or result for image/file)
        var finalResponse = await _httpClient.GetAsync(uploadUrl, cancellationToken).ConfigureAwait(false);
        if (finalResponse.IsSuccessStatusCode)
        {
            var responseBody = await finalResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return JsonSerializer.Deserialize<object>(responseBody) ?? new { };
            }
            catch (JsonException)
            {
                return responseBody;
            }
        }

        return new { };
    }
}

