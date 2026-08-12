namespace Domain.Entities
{
    public class IngredientMeasurement : BaseEntity
    {
        public MeasurementType Type { get; set; }
        public IEnumerable<RecipeIngredient> RecipeIngredints { get; set; }
    }
}
