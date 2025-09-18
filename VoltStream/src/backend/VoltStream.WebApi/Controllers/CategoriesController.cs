namespace VoltStream.WebApi.Controllers;

using VoltStream.Application.Features.Categories.Commands;
using VoltStream.Application.Features.Categories.DTOs;
using VoltStream.Application.Features.Categories.Queries;
using VoltStream.WebApi.Controllers.Common;

public class CategoriesController
    : CrudController<CategoryDto,
        GetAllCategoriesQuery,
        GetCategoryByIdQuery,
        CreateCategoryCommand,
        UpdateCategoryCommand,
        DeleteCategoryCommand>;