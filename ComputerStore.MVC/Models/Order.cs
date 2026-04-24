using System;
using System.Collections.Generic;

namespace ComputerStore.MVC.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? UserId { get; set; }

    public int? PromotionId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Promotion? Promotion { get; set; }

    public virtual AppUser? User { get; set; }
}
