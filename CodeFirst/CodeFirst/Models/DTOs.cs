﻿using System.ComponentModel.DataAnnotations;
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
    public String DateFrom { get; set; } = string.Empty;
        
    [Required]
    public String DateTo { get; set; } = string.Empty;

}



public class OneTimePaymentDTO
{
    public int IdClient { get; set; }
    public int IdSoftware { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public String DateFrom { get; set; } = string.Empty;
    public String DateTo { get; set; } = string.Empty;
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

    public String DateFrom { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    

    [Required]
    public int RenevalTimeInMonths { get; set; }
    
}
public class SubscriptionResponseDTO
{
    public int IdContract { get; set; }
    public SubscriptionDTO Subscription { get; set; }
    
}

public class CorpoClientDTO
{
    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public string CorpoName { get; set; } = string.Empty;

    [Required]
    public Decimal KRS { get; set; }
}

public class PersonalClientDTO
{
    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public Decimal PESEL { get; set; }
}

public class CorpoEditDTO
{
    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public string CorpoName { get; set; } = string.Empty;
}

public class PersonalEditDTO
{
    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;
}