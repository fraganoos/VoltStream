namespace VoltStream.Application.Commons.Interfaces;

using VoltStream.Application.Commons.Models;

public interface IPagingMetadataWriter
{
    void Write(PagedListMetadata metadata);
}
