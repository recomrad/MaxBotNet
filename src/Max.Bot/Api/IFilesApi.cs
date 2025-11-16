using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Max.Bot.Types;
using Max.Bot.Types.Enums;

namespace Max.Bot.Api;

/// <summary>
/// Interface for file-related API methods.
/// </summary>
public interface IFilesApi
{
    /// <summary>
    /// Uploads a file and returns an upload URL and optional token.
    /// </summary>
    /// <param name="uploadType">The type of file to upload.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the upload response with URL and optional token.</returns>
    /// <exception cref="Max.Bot.Exceptions.MaxApiException">Thrown when the API returns an error response.</exception>
    /// <exception cref="Max.Bot.Exceptions.MaxNetworkException">Thrown when a network error occurs.</exception>
    /// <exception cref="Max.Bot.Exceptions.MaxUnauthorizedException">Thrown when authentication fails.</exception>
    Task<UploadResponse> UploadFileAsync(UploadType uploadType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads file data to the upload URL using multipart/form-data.
    /// </summary>
    /// <param name="uploadUrl">The upload URL obtained from UploadFileAsync.</param>
    /// <param name="fileStream">The stream containing the file data to upload.</param>
    /// <param name="fileName">The name of the file. Optional.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the upload result (token for video/audio or JSON object for image/file).</returns>
    /// <exception cref="ArgumentException">Thrown when uploadUrl is null or empty, or fileStream is not readable.</exception>
    /// <exception cref="ArgumentNullException">Thrown when fileStream is null.</exception>
    /// <exception cref="Max.Bot.Exceptions.MaxApiException">Thrown when the upload fails.</exception>
    /// <exception cref="Max.Bot.Exceptions.MaxNetworkException">Thrown when a network error occurs.</exception>
    Task<object> UploadFileDataAsync(string uploadUrl, Stream fileStream, string? fileName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads file data in chunks using resumable upload method.
    /// </summary>
    /// <param name="uploadUrl">The upload URL obtained from UploadFileAsync.</param>
    /// <param name="fileStream">The stream containing the file data to upload.</param>
    /// <param name="chunkSize">The size of each chunk in bytes. Default is 1 MB.</param>
    /// <param name="fileName">The name of the file. Optional.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the upload result (token for video/audio or JSON object for image/file).</returns>
    /// <exception cref="ArgumentException">Thrown when uploadUrl is null or empty, fileStream is not readable, or chunkSize is less than or equal to zero.</exception>
    /// <exception cref="ArgumentNullException">Thrown when fileStream is null.</exception>
    /// <exception cref="Max.Bot.Exceptions.MaxApiException">Thrown when the upload fails.</exception>
    /// <exception cref="Max.Bot.Exceptions.MaxNetworkException">Thrown when a network error occurs.</exception>
    Task<object> UploadFileResumableAsync(string uploadUrl, Stream fileStream, long chunkSize = 1024 * 1024, string? fileName = null, CancellationToken cancellationToken = default);
}

