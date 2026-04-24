using System;
using System.Collections.Generic;

namespace ComputerStore.MVC.Models;

public partial class AppUser
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<WarrantyTicket> WarrantyTickets { get; set; } = new List<WarrantyTicket>();
    public string Username { get; set; } = string.Empty;
    public bool IsLocked { get; set; } = false;
}
