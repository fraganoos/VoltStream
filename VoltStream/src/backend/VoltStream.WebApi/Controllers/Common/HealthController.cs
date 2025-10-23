namespace VoltStream.WebApi.Controllers.Common;

using Microsoft.AspNetCore.Mvc;
using VoltStream.WebApi.Models;

public class HealthController : BaseController
{
    [HttpGet]
    public IActionResult CheckHealth()
        => Ok(new Response { Data = "Server is healthy!" });
}