using Application.Common.Wrappers;
using Application.Features.Recipes.Interfaces;
using MediatR;

namespace Application.Features.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeBySlugCommandHandler : IRequestHandler<DeleteRecipeBySlugCommand, Response>
{
    private readonly IRecipeRepository _recipeDb;

    public DeleteRecipeBySlugCommandHandler(IRecipeRepository recipeDb) => _recipeDb = recipeDb;

    public async Task<Response> Handle(
        DeleteRecipeBySlugCommand request,
        CancellationToken cancellationToken
    )
    {
        var response = new Response();

        await _recipeDb.DeleteRecipeBySlugAsync(request.Slug, cancellationToken: cancellationToken);

        return response;
    }
}
