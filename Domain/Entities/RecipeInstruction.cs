using Domain.Common;

namespace Domain.Entities
{
    public class RecipeInstruction : BaseEntity
    {
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        public required string Text { get; set; }
    }
}
