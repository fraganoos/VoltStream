namespace VoltStream.WPF.Configurations;

using System.Net.Http;
using System.Net.Http.Headers;
using VoltStream.WPF.Commons.Services;

public class AuthHeaderHandler(ISessionService session) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(session.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
