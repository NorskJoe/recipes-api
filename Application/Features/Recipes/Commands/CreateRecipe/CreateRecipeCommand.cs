using Application.Common.Wrappers;
using Application.Features.Recipes.Dtos.Command;
using MediatR;

namespace Application.Features.Recipes.Commands.CreateRecipe
{
    // TODO: is slug the correct response?  Maybe better to return new recipe
    public class CreateRecipeCommand : IRequest<Response<string>>
    {
        public required string Title { get; set; }
        public required string Slug { get; set; }
        public string? Description { get; set; }
        public int? Servings { get; set; }
        public int? PrepTimeInMinutes { get; set; }
        public int? CookTimeInMinutes { get; set; }
        public string CreatedBy { get; set; } = "system";
        public List<RecipeIngredientDto> Ingredients { get; set; } = [];
        public List<RecipeStepDto> Instructions { get; set; } = [];
        public List<string> Tags { get; set; } = [];
    }
}
