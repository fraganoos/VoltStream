namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Currencies.Commands;
using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Application.Features.Currencies.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class CurrenciesController
    : CrudController<CurrencyDto,
        GetAllCurrenciesQuery,
        GetCurrencyByIdQuery,
        CreateCurrencyCommand,
        UpdateCurrencyCommand,
        DeleteCurrencyCommand>
{
    [HttpPut("all")]
    public async Task<IActionResult> UpdateAll(List<CurrencyCommand> items)
        => Ok(new Response { Data = await Mediator.Send(new UpdateAllCurrenciesCommand(items)) });

    [HttpPost("filter")]
    public async Task<IActionResult> GetFiltered(CurrencyFilterQuery query)
      => Ok(new Response { Data = await Mediator.Send(query) });
}
