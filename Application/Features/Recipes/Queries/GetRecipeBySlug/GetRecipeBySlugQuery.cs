using Application.Common.Wrappers;
using Application.Features.Recipes.Dtos;
using MediatR;

namespace Application.Features.Recipes.Queries.GetRecipeBySlug
{
    public class GetRecipeBySlugQuery : IRequest<Response<RecipeDto>>
    {
        public required string Slug { get; set; }
    }
}
