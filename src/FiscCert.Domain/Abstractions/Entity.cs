namespace FiscCert.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid TenantId { get; set; }

    protected Entity() { }
    protected Entity(Guid tenantId)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        TenantId = tenantId;
    }
}
