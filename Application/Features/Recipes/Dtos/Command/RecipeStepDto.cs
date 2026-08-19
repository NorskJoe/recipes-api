namespace Application.Features.Recipes.Dtos.Command
{
    public class RecipeStepDto
    {
        public int StepNumber { get; set; }
        public required string Text { get; set; }
    }
}
