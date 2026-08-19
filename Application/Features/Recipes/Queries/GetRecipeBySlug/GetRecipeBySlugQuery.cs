using Application.Common.Wrappers;
using Application.Features.Recipes.Dtos.Query;
using MediatR;

namespace Application.Features.Recipes.Queries.GetRecipeBySlug
{
    public class GetRecipeBySlugQuery : IRequest<Response<RecipeDto>>
    {
        public required string Slug { get; set; }
    }
}
