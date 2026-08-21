using Application.Common.Wrappers;
using Application.Features.Recipes.Interfaces;
using MediatR;

namespace Application.Features.Recipes.Commands.CreateRecipe
{
    public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Response<string>>
    {
        private readonly IRecipeRepository _recipeDb;

        public CreateRecipeCommandHandler(IRecipeRepository recipes)
        {
            _recipeDb = recipes;
        }

        public async Task<Response<string>> Handle(
            CreateRecipeCommand request,
            CancellationToken cancellationToken
        )
        {
            var response = new Response<string>(request.Slug);
            if (string.IsNullOrEmpty(request.Slug))
            {
                // TODO: check if slug already exists before continuing
            }

            var slug = await _recipeDb.CreateRecipeAsync(request, cancellationToken);
            response.Value = slug;
            // TODO: Better error handling from the implementation that is captured here
            if (slug is null)
                response.AddError($"Recipe creation with slug ${slug} faled");

            return response;
            ;
        }
    }
}
