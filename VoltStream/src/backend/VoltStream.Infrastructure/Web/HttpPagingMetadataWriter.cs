namespace VoltStream.Infrastructure.Web;

using Microsoft.AspNetCore.Http;
using System.Text.Json;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;

public class HttpPagingMetadataWriter(IHttpContextAccessor accessor) : IPagingMetadataWriter
{
    public void Write(PagedListMetadata metadata)
    {
        var headers = accessor.HttpContext?.Response?.Headers;
        if (headers is null) return;

        headers["X-Paging"] = JsonSerializer.Serialize(metadata);
    }
}