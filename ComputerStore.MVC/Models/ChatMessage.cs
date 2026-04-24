using System;
using System.Collections.Generic;

namespace ComputerStore.MVC.Models;

public partial class ChatMessage
{
    public int MessageId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public int? SenderId { get; set; }

    public virtual AppUser? Sender { get; set; }
}
