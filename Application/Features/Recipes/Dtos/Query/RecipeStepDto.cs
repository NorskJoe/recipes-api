namespace Application.Features.Recipes.Dtos.Query
{
    public record RecipeInstructionDto
    {
        public int RecipeInstructionId { get; init; }
        public int Step { get; init; }
        public required string Text { get; init; }
    }
}
