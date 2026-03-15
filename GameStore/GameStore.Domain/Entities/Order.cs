using GameStore.Domain.Commom;

namespace GameStore.Domain.Entities;


public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; } 
    public decimal TotalValue { get; set; } 

    public Guid CustomerId { get; set; } 
    
    public Customer Customer { get; set; } 
    
}