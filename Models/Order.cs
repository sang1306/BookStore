using System;
using System.Collections.Generic;
using BookStore.Enums;

namespace BookStore.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public OrderStatus? OrderStatus { get; set; } = Enums.OrderStatus.Pending;

    public DateTime? OrderDate { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
