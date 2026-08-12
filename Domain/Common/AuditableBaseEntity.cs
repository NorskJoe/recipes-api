namespace Domain.Common
{
    public abstract class AuditableBaseEntity : BaseEntity
    {
        public required string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
