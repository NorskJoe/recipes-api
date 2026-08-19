using Application.Features.Recipes.Commands.CreateRecipe;

namespace Application.Features.Recipes.Interfaces
{
    public interface IRecipeWriteRepository
    {
        Task<string> CreateRecipeAsync(
            CreateRecipeCommand recipe,
            CancellationToken cancellationToken
        );
    }
}
