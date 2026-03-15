using GameStore.Domain.Commom;

namespace GameStore.Domain.Entities;

public class CustomerConfiguration : BaseEntity
{
    public string Theme { get; private set; } = "Dark";

    public bool EnableNotifications { get; private set; }
    
    //1..1
    public Guid CustomerId { get; private set; }

    public CustomerConfiguration(Guid customerId)
    {
        CustomerId = customerId;
    }

}