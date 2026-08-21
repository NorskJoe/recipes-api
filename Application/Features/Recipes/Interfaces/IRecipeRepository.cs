using Application.Features.Recipes.Commands.CreateRecipe;
using Application.Features.Recipes.Dtos.Query;

namespace Application.Features.Recipes.Interfaces
{
    public interface IRecipeRepository
    {
        Task<RecipeDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
        Task<string> CreateRecipeAsync(
            CreateRecipeCommand command,
            CancellationToken cancellationToken
        );
        Task DeleteRecipeBySlugAsync(string slug, CancellationToken cancellationToken);
    }
}
