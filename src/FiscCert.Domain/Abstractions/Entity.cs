namespace FiscCert.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; private set; }
    public Guid TenantId { get; protected set; }

    protected Entity() { }
    protected Entity(Guid tenantId)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        TenantId = tenantId;
    }
}
