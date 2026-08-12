using Domain.Common;
using Domain.Entities.Enums;

namespace Domain.Entities
{
    public class RecipeIngredient : BaseEntity
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public MeasurementType Measurement { get; set; }
        public decimal Quantity { get; set; }
    }
}
