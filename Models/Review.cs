﻿using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public int Ratting { get; set; }

    public string? Comment { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
