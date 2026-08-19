using Application.Common.Interfaces;
using Application.Features.Recipes.Dtos.Query;
using Application.Features.Recipes.Interfaces;
using Dapper;

namespace Infrastructure.Persistence.Repositories
{
    public class RecipeReadRepository : IRecipeReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RecipeReadRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<RecipeDto?> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken
        )
        {
            const string sql =
                @"
                -- 1. Recipe header
                SELECT
                    r.Id,
                    r.Title,
                    r.Slug,
                    r.Description,
                    r.Servings,
                    r.PrepTimeInMinutes,
                    r.CookTimeInMinutes,
                    r.CreatedBy,
                    r.CreatedAt,
                    r.LastModifiedBy,
                    r.LastModified
                FROM Recipe r
                WHERE r.Slug = @Slug;

                -- 2. Ingredients
                SELECT
                    ri.IngredientId,
                    i.Name,
                    ri.Quantity,
                    ri.Measurement   AS MeasurementType,
                    im.DisplayName   AS MeasurementName,
                    im.Abbreviation  AS MeasurementAbbreviation
                FROM RecipeIngredient ri
                INNER JOIN Recipe r                 ON r.Id  = ri.RecipeId
                INNER JOIN Ingredient i             ON i.Id  = ri.IngredientId
                INNER JOIN IngredientMeasurement im ON im.[Type] = ri.Measurement
                WHERE r.Slug = @Slug;

                -- 3. Steps
                SELECT
                    rs.Id          AS RecipeInstructionId,
                    rs.StepNumber  AS Step,
                    rs.[Text]
                FROM RecipeInstruction rs
                INNER JOIN Recipe r ON r.Id = rs.RecipeId
                WHERE r.Slug = @Slug
                ORDER BY rs.StepNumber;

                -- 4. Tags
                SELECT
                    t.Name
                FROM RecipeTag rt
                INNER JOIN Recipe r ON r.Id = rt.RecipeId
                INNER JOIN Tag t    ON t.Id = rt.TagId
                WHERE r.Slug = @Slug
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

            var recipe = await multi.ReadSingleOrDefaultAsync<RecipeDto>();
            if (recipe is null)
                return null;

            var ingredients = (await multi.ReadAsync<RecipeIngredientDto>()).ToList();
            var steps = (await multi.ReadAsync<RecipeInstructionDto>()).ToList();
            var tags = (await multi.ReadAsync<string>()).ToList();

            return recipe with
            {
                Ingredients = ingredients,
                Steps = steps,
                Tags = tags,
            };
        }
    }
}
