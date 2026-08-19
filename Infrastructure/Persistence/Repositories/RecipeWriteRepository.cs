using Application.Common.Interfaces;
using Application.Features.Recipes.Commands.CreateRecipe;
using Application.Features.Recipes.Interfaces;
using Dapper;

namespace Infrastructure.Persistence.Repositories
{
    public class RecipeWriteRepository : IRecipeWriteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RecipeWriteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<string> CreateRecipeAsync(
            CreateRecipeCommand recipe,
            CancellationToken cancellationToken
        )
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync(
                cancellationToken
            );
            using var tx = connection.BeginTransaction();

            // 1. Insert recipe and get RecipeId
            const string insertRecipeSql =
                @"
                INSERT INTO Recipe (Title, Slug, Description, Servings, PrepTimeInMinutes, CookTimeInMinutes, CreatedBy, CreatedAt)
                VALUES (
                    @Title,
                    @Slug,
                    @Description,
                    @Servings,
                    @PrepTimeInMinutes,
                    @CookTimeInMinutes,
                    @CreatedBy,
                    @CreatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";

            var recipeId = await connection.ExecuteScalarAsync<int>(
                insertRecipeSql,
                new
                {
                    recipe.Title,
                    recipe.Slug,
                    recipe.Description,
                    recipe.Servings,
                    recipe.PrepTimeInMinutes,
                    recipe.CookTimeInMinutes,
                    recipe.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                },
                tx
            );

            // 2. Insert RecipeInstructions
            foreach (var instruction in recipe.Instructions)
            {
                const string sql =
                    @"
                        INSERT INTO RecipeInstruction (RecipeId, StepNumber, Text)
                        VALUES (
                            @RecipeId,
                            @StepNumber,
                            @Text
                        )
                    ";

                await connection.ExecuteAsync(
                    sql,
                    new
                    {
                        RecipeId = recipeId,
                        instruction.StepNumber,
                        instruction.Text,
                    },
                    tx
                );
            }

            // 3. Insert RecipeIngredients
            foreach (var ingredient in recipe.Ingredients)
            {
                const string ingredientSql =
                    @"
                        INSERT INTO Ingredient (Name)
                        SELECT @Name
                        WHERE NOT EXISTS (SELECT 1 FROM Ingredient WHERE Name = @Name);

                        SELECT Id FROM Ingredient WHERE Name = @Name;
                    ";

                var ingredientId = await connection.ExecuteScalarAsync<int>(
                    ingredientSql,
                    new { ingredient.Name },
                    tx
                );

                const string recipeIngredientSql =
                    @"
                        INSERT INTO RecipeIngredient (RecipeId, IngredientId, Measurement, Quantity) 
                        VALUES (
                            @RecipeId,
                            @IngredientId,
                            @Measurement,
                            @Quantity
                        );
                    ";

                await connection.ExecuteAsync(
                    recipeIngredientSql,
                    new
                    {
                        RecipeId = recipeId,
                        IngredientId = ingredientId,
                        Measurement = ingredient.MeasurementType,
                        ingredient.Quantity,
                    },
                    tx
                );
            }

            // 4. Insert Tags
            foreach (var tag in recipe.Tags)
            {
                const string tagSql =
                    @"
                        INSERT INTO Tag (Name) SELECT @Name
                        WHERE NOT EXISTS (SELECT 1 FROM Tag WHERE Name = @Name);

                        SELECT Id FROM Tag
                        WHERE Name = @Name;
                    ";

                var tagId = await connection.ExecuteScalarAsync<int>(
                    tagSql,
                    new { Name = tag },
                    tx
                );

                const string recipeTagSql =
                    @"
                        INSERT INTO RecipeTag (RecipeId, TagId)
                        VALUES (
                            @RecipeId,
                            @TagId
                        )
                    ";

                await connection.ExecuteAsync(
                    recipeTagSql,
                    new { RecipeId = recipeId, TagId = tagId },
                    tx
                );
            }

            tx.Commit();

            return recipe.Slug;
        }
    }
}
