using Application.Common.Wrappers;
using Application.Features.Recipes.Dtos.Query;
using Application.Features.Recipes.Interfaces;
using MediatR;

namespace Application.Features.Recipes.Queries.GetRecipeBySlug
{
    public class GetRecipeBySlugQueryHandler
        : IRequestHandler<GetRecipeBySlugQuery, Response<RecipeDto>>
    {
        private readonly IRecipeRepository _recipeDb;

        public GetRecipeBySlugQueryHandler(IRecipeRepository recipes)
        {
            _recipeDb = recipes;
        }

        public async Task<Response<RecipeDto>> Handle(
            GetRecipeBySlugQuery request,
            CancellationToken cancellationToken
        )
        {
            var recipe = await _recipeDb.GetBySlugAsync(request.Slug, cancellationToken);

            var response = new Response<RecipeDto>(recipe);
            if (recipe is null)
                response.AddError($"Recipe with slug '{request.Slug}' was not found.");

            return response;
        }
    }
}
