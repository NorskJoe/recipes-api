using Domain.Common;

namespace Domain.Entities
{
    public class Tag : BaseEntity
    {
        public required string Name { get; set; }
    }
}
