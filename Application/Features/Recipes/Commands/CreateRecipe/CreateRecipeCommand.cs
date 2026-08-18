using Application.Common.Wrappers;
using MediatR;

namespace Application.Features.Recipes.Commands.CreateRecipe
{
    public class CreateRecipeCommand : IRequest<Response<int>>
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; } = "system";
    }
}
