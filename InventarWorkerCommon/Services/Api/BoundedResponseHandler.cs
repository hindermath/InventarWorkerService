using System.Net;

namespace InventarWorkerCommon.Services.Api;

/// <summary>
/// DE: Begrenzt Antworten untrusted HTTP-Ziele vor dem Puffern durch Aufrufer.
/// EN: Limits responses from untrusted HTTP targets before callers buffer them.
/// </summary>
internal sealed class BoundedResponseHandler : DelegatingHandler
{
    private readonly long _maximumBytes;

    internal BoundedResponseHandler(long maximumBytes, HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
        _maximumBytes = maximumBytes;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (response.Content.Headers.ContentLength is long contentLength && contentLength > _maximumBytes)
        {
            response.Dispose();
            throw new HttpRequestException("The inventory response exceeds the configured size limit.");
        }

        response.Content = new BoundedHttpContent(response.Content, _maximumBytes);
        return response;
    }

    private sealed class BoundedHttpContent(HttpContent innerContent, long maximumBytes) : HttpContent
    {
        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            CopyBoundedAsync(stream, CancellationToken.None);

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken) =>
            CopyBoundedAsync(stream, cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                innerContent.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task CopyBoundedAsync(Stream destination, CancellationToken cancellationToken)
        {
            await using var source = await innerContent.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var buffer = new byte[81920];
            long total = 0;
            while (true)
            {
                var read = await source.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                if (read == 0)
                {
                    return;
                }

                total += read;
                if (total > maximumBytes)
                {
                    throw new HttpRequestException("The inventory response exceeds the configured size limit.");
                }

                await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
