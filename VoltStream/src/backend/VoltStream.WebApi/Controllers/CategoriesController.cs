namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;

using VoltStream.Application.Features.Categories.Commands;
using VoltStream.WebApi.Models;

public class CategoriesController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCategoryCommand command)
        => Ok(new Response() { Data = await Mediator.Send(command) });
}
