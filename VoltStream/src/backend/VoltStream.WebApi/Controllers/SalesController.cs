using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Sales.Commands;
using VoltStream.WebApi.Models;

namespace VoltStream.WebApi.Controllers;

public class SalesController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateSaleCommand command)
        => Ok(new Response() { Data = await Mediator.Send(command) });
}
