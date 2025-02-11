using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Chat
{
    public int ChatId { get; set; }

    public int ReceiverId { get; set; }

    public int SenderId { get; set; }

    public string? Message { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
