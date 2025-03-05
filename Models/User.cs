﻿using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Role { get; set; }

    // full name
    public string? Preferences { get; set; }

    public DateTime? CreateAt { get; set; }

    public string? Address { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Chat> ChatReceivers { get; set; } = new List<Chat>();

    public virtual ICollection<Chat> ChatSenders { get; set; } = new List<Chat>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
