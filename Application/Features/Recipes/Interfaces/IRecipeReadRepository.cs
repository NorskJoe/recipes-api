using Application.Features.Recipes.Dtos.Query;

namespace Application.Features.Recipes.Interfaces
{
    public interface IRecipeReadRepository
    {
        Task<RecipeDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
    }
}
