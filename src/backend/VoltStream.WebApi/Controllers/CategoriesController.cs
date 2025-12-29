namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Categories.Commands;
using VoltStream.Application.Features.Categories.DTOs;
using VoltStream.Application.Features.Categories.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class CategoriesController
    : CrudController<CategoryDto,
        GetAllCategoriesQuery,
        GetCategoryByIdQuery,
        CreateCategoryCommand,
        UpdateCategoryCommand,
        DeleteCategoryCommand>
{
    [HttpPost("filter")]
    public async Task<IActionResult> GetFiltered(CategoryFilterQuery query)
        => Ok(new Response { Data = await Mediator.Send(query) });
}