namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Monitoring.Commands;
using VoltStream.Application.Features.Monitoring.DTOs;
using VoltStream.Application.Features.Monitoring.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class AllowedClientsController
    : CrudController<AllowedClientDto,
        GetAllAllowedClientsQuery,
        GetAllowedClientByIdQuery,
        CreateAllowedClientCommand,
        UpdateAllowedClientCommand,
        DeleteAllowedClientCommand>
{
    [HttpPost("filter")]
    public async Task<IActionResult> GetFiltered(AllowedClientFilterQuery query)
        => Ok(new Response { Data = await Mediator.Send(query) });
}