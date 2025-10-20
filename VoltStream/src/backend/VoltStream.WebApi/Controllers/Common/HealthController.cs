namespace VoltStream.WebApi.Controllers.Common;

using Microsoft.AspNetCore.Mvc;

public class HealthController : BaseController
{
    [HttpGet]
    public IActionResult CheckHealth()
        => Ok("Server is healthy!");
}