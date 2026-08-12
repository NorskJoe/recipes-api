using Domain.Common;

namespace Domain.Entities
{
    public class RecipeTag : BaseEntity
    {
        public int RecipeId { get; set; }
        public int TagId { get; set; }
    }
}
