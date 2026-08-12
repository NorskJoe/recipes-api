using Domain.Common;

namespace Domain.Entities
{
    public class Ingredient : BaseEntity
    {
        public required string Name { get; set; }
    }
}
