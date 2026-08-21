using Application.Common.Interfaces;
using Application.Common.Wrappers;
using Application.Features.Recipes.Commands.CreateRecipe;
using Application.Features.Recipes.Dtos.Query;
using Application.Features.Recipes.Interfaces;
using Dapper;

namespace Infrastructure.Persistence.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RecipeRepository(IDbConnectionFactory connectionFactory) =>
            _connectionFactory = connectionFactory;

        public async Task<string> CreateRecipeAsync(
            CreateRecipeCommand command,
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

                    SELECT CAST(SCOPE_IDENTITY() as INT);
                 ";

            var recipeId = await connection.ExecuteScalarAsync<int>(
                insertRecipeSql,
                new
                {
                    command.Title,
                    command.Slug,
                    command.Description,
                    command.Servings,
                    command.PrepTimeInMinutes,
                    command.CookTimeInMinutes,
                    command.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                },
                tx
            );

            // 2. Insert Recipe Instructions
            foreach (var instruction in command.Instructions)
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

            // 3. Insert Recipe Ingredients
            foreach (var ingredient in command.Ingredients)
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
            foreach (var tag in command.Tags)
            {
                const string tagSql =
                    @"
                        INSERT INTO Tag (Name) SELECT @Name
                        WHERE NOT EXISTS (SELECT 1 FROM Tag WHERE Name =  @Name);

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

            return command.Slug;
        }

        public async Task DeleteRecipeBySlugAsync(string slug, CancellationToken cancellationToken)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync(
                cancellationToken
            );
            using var tx = connection.BeginTransaction();

            // 1. Delete recipe
            const string deleteRecipeSql =
                @"
                DELETE FROM Recipe
                WHERE Slug = @Slug;
            ";
            await connection.ExecuteAsync(deleteRecipeSql, new { Slug = slug }, tx);

            // 2. Delete orphaned ingredient names
            const string deleteIngredientsSql =
                @"
                    DELETE i
                    FROM Ingredient i
                    LEFT JOIN RecipeIngredient ri ON i.Id = ri.IngredientId
                    WHERE ri.Id IS NULL;
                ";

            await connection.ExecuteAsync(deleteIngredientsSql, transaction: tx);

            // 3. Delete orphaned tag names
            const string deleteTagsSql =
                @"
                    DELETE t
                    FROM Tag t
                    LEFT JOIN RecipeTag rt ON t.Id = rt.TagId
                    WHERE rt.Id IS NULL;
                ";
            await connection.ExecuteAsync(deleteTagsSql, transaction: tx);

            tx.Commit();
            return;
        }

        public async Task<RecipeDto?> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken
        )
        {
            const string sql =
                @"
                    -- 1. Select Recipe
                    SELECT * FROM Recipe 
                    WHERE Slug = @Slug;

                    -- 2. Ingredients
                    SELECT 
                        ri.IngredientId,
                        i.Name,
                        ri.Quantity,
                        ri.Measurement      AS MeasurementType,
                        im.DisplayName      AS MeasurementName,
                        im.Abbreviation     AS MeasurementAbbreviation
                    FROM RecipeIngredient ri
                    INNER JOIN Recipe r                 ON r.Id = ri.RecipeId
                    INNER JOIN Ingredient i             ON i.Id = ri.IngredientId
                    INNER JOIN IngredientMeasurement im ON im.[Type] = ri.Measurement
                    WHERE r.Slug = @Slug;

                    -- 3. Instructions
                    SELECT 
                        rs.Id AS RecipeInstructionId,
                        rs.StepNumber AS Step,
                        rs.[Text]
                    FROM RecipeInstruction rs
                    INNER JOIN Recipe r ON r.Id = rs.RecipeId
                    WHERE r.Slug = @Slug
                    ORDER BY rs.StepNumber;

                    -- 4. Tags
                    SELECT t.Name
                    FROM RecipeTag rt
                    INNER JOIN Recipe r ON r.Id = rt.RecipeId
                    INNER JOIN Tag t ON t.Id = rt.TagId
                    WHERE r.Slug = @Slug;
                ";

            using var connection = await _connectionFactory.CreateOpenConnectionAsync(
                cancellationToken
            );

            using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(
                    sql,
                    new { Slug = slug },
                    cancellationToken: cancellationToken
                )
            );

            var recipe = multi.ReadSingleOrDefault<RecipeDto>();

            if (recipe is null)
                return recipe;

            var ingredients = (await multi.ReadAsync<RecipeIngredientDto>()).ToList();
            var instructions = (await multi.ReadAsync<RecipeInstructionDto>()).ToList();
            var tags = (await multi.ReadAsync<string>()).ToList();

            return recipe with
            {
                Ingredients = ingredients,
                Steps = instructions,
                Tags = tags,
            };
        }
    }
}
