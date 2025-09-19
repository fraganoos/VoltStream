namespace VoltStream.Infrastructure.Web;

using Microsoft.AspNetCore.Http;
using System.Text.Json;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;

public class HttpPagingMetadataWriter : IPagingMetadataWriter
{
    private readonly IHttpContextAccessor accessor;

    public HttpPagingMetadataWriter(IHttpContextAccessor accessor)
    {
        this.accessor = accessor;
        PagingExtensions.ConfigureWriter(this);
    }

    public void Write(PagedListMetadata metadata)
    {
        var headers = accessor.HttpContext?.Response?.Headers;
        if (headers is null) return;

        headers["X-Paging"] = JsonSerializer.Serialize(metadata);
    }
}


