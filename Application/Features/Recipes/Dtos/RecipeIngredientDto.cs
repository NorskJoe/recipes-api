using Domain.Entities.Enums;

namespace Application.Features.Recipes.Dtos
{
    public record RecipeIngredientDto
    {
        public int IngredientId { get; init; }
        public required string Name { get; init; }
        public decimal Quantity { get; init; }
        public MeasurementType MeasurementType { get; init; }
        public required string MeasurementName { get; init; }
        public string? MeasurementAbbreviation { get; init; }
    }
}
