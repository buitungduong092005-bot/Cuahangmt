using System;
using System.Collections.Generic;

namespace ComputerStore.MVC.Models;

public partial class WarrantyTicket
{
    public int TicketId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string IssueDescription { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public int? CustomerId { get; set; }

    public virtual AppUser? Customer { get; set; }
}
