using Domain.Entities.Enums;

namespace Application.Features.Recipes.Dtos.Command
{
    public class RecipeIngredientDto
    {
        public required string Name { get; set; }
        public decimal Quantity { get; set; }
        public MeasurementType MeasurementType { get; set; }
    }
}
