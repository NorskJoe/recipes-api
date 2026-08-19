namespace Application.Features.Recipes.Dtos.Query
{
    public record RecipeDto
    {
        public int Id { get; init; }
        public required string Title { get; init; }
        public required string Slug { get; init; }
        public string? Description { get; init; }
        public int? Servings { get; init; }

        // cook time info
        public int? PrepTimeInMinutes { get; init; }
        public int? CookTimeInMinutes { get; init; }
        public int? TotalTimeInMinutes =>
            PrepTimeInMinutes is null && CookTimeInMinutes is null
                ? null
                : (PrepTimeInMinutes ?? 0) + (CookTimeInMinutes ?? 0);

        // collections
        public List<RecipeInstructionDto> Steps { get; init; } = [];
        public List<RecipeIngredientDto> Ingredients { get; init; } = [];
        public List<string> Tags { get; init; } = [];

        // audit - required
        public required string CreatedBy { get; init; }
        public required DateTime CreatedAt { get; init; }

        // audit - optional
        public string? LastModifiedBy { get; init; }
        public DateTime? LastModified { get; init; }
    }
}
