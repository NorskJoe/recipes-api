using Application.Common.Wrappers;
using MediatR;

namespace Application.Features.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeBySlugCommand : IRequest<Response>
{
    public required string Slug { get; set; }
}
