﻿using System;

public class Customer
{
    public int ID { get; set; } 
    public int CompanyID { get; set; } 
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }

    public Company Company { get; set; }
    public ICollection<Invoice> Invoices { get; set; }
    public ICollection<ActTrans> ActTrans { get; set; }
    public ICollection<Balance> Balances { get; set; }
}
