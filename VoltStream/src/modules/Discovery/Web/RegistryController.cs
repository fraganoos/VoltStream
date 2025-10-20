namespace Discovery.Web;

using Discovery.Models;
using Discovery.Store;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/discovery")]
public class RegistryController(IDiscoveryStore store) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] ServiceRegistration registration)
    {
        if (string.IsNullOrWhiteSpace(registration.ServiceId))
            return BadRequest("ServiceId is required.");

        if (string.IsNullOrWhiteSpace(registration.IpAddress))
            return BadRequest("IpAddress is required.");

        if (registration.Port <= 0 || registration.Port > 65535)
            return BadRequest("Valid Port is required.");

        store.Register(registration);
        return Ok(new { message = "✅ Registered successfully." });
    }

    [HttpGet("nodes/{serviceId}")]
    public IActionResult GetNodes(string serviceId)
    {
        var nodes = store.GetNodes(serviceId)
            .Select(x => new
            {
                x.ServiceId,
                x.IpAddress,
                x.Port,
                ttlSeconds = (int)(60 - (DateTime.UtcNow - x.RegisteredAt).TotalSeconds),
                x.RegisteredAt
            })
            .Where(x => x.ttlSeconds > 0)
            .ToArray();

        return Ok(nodes);
    }
}
