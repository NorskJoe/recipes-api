using Application.Common.Wrappers;
using Application.Features.Recipes.Interfaces;
using MediatR;

namespace Application.Features.Recipes.Commands.CreateRecipe
{
    public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Response<string>>
    {
        private readonly IRecipeWriteRepository _recipes;

        public CreateRecipeCommandHandler(IRecipeWriteRepository recipes)
        {
            _recipes = recipes;
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

            var slug = await _recipes.CreateRecipeAsync(request, cancellationToken);
            response.Value = slug;
            // TODO: Better error handling from the implementation that is captured here
            if (slug is null)
                response.AddError($"Recipe creation with slug ${slug} faled");

            return response;
            ;
        }
    }
}
