namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.WebApi.Models;
using VoltStream.Application.Features.Sales.Commands;


public class SalesController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateSaleCommand command)
        => Ok(new Response() { Data = await Mediator.Send(command) });
}
