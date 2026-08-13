namespace Domain.Entities
{
    public class IngredientMeasurement
    {
        public MeasurementType Type { get; set; }
        public required string DisplayName { get; set; }
        public required string Abbreviation { get; set; }
    }
}
