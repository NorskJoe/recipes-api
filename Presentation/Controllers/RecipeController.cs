using Application.Features.Recipes.Commands.CreateRecipe;
using Application.Features.Recipes.Queries.GetRecipeBySlug;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecipeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Get(string slug)
    {
        return Ok(await _mediator.Send(new GetRecipeBySlugQuery { Slug = slug }));
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateRecipeCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}
