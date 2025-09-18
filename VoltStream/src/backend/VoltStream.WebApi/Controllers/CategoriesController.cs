namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Categories.Commands;
using VoltStream.Application.Features.Categories.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class CategoriesController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCategoryCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("{Id:long}")]
    public async Task<IActionResult> Delete(long Id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteCategoryCommand(Id)) });

    [HttpGet("{Id:long}")]
    public async Task<IActionResult> GetById(long Id)
        => Ok(new Response { Data = await Mediator.Send(new GetCategoryByIdQuery(Id)) });

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllCategoriesQuery()) });
}
