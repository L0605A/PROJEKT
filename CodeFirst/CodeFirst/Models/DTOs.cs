using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirst.Models;

public class DiscountDTO
{
        
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
        
    [Required]
    [MaxLength(100)]
    public string Offer { get; set; } = string.Empty;
        
    [Required]
    public int Amt { get; set; }
        
    [Required]
    public String DateFrom { get; set; }
        
    [Required]
    public String DateTo { get; set; }

}



public class OneTimePaymentDTO
{
    public int IdClient { get; set; }
    public int IdSoftware { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public decimal Price { get; set; }
    public int UpdatePeriod { get; set; }
    
}

public class OneTimePaymentResponseDTO
{
    public int IdContract { get; set; }
    public string Status { get; set; } = string.Empty;
    public OneTimePaymentDTO oneTimePayment { get; set; }
    
}

public class SubscriptionDTO
{
    public int IdClient { get; set; }
    public int IdSoftware { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public DateOnly DateFrom { get; set; }
    public decimal Price { get; set; }
    
    public int UpdatePeriod { get; set; }
    

    [Required]
    public int RenevalTimeInMonths { get; set; }
    
}
public class SubscriptionResponseDTO
{
    public int IdContract { get; set; }
    public SubscriptionDTO Subscription { get; set; }
    
}

