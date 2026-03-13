using System;
using System.Collections.Generic;

namespace CinemaApp.Models;

public partial class Film
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Genre { get; set; }

    public int? Duration { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
