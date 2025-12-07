namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.DiscountOperations.Commands;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class DiscountsController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Apply(ApplyDiscountCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });
}