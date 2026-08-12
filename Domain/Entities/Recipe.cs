using Domain.Common;

namespace Domain.Entities
{
    public class Recipe : AuditableBaseEntity
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int? Servings { get; set; }
        public int? PrepTimeInMinutes { get; set; }
        public int? CookTimeInMinutes { get; set; }
        public required string Slug { get; set; }
    }
}
