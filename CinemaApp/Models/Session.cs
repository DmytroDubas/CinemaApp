using System;
using System.Collections.Generic;

namespace CinemaApp.Models;

public partial class Session
{
    public int Id { get; set; }

    public int? Filmid { get; set; }

    public int? Hallid { get; set; }

    public DateTime Datetime { get; set; }

    public decimal? Price { get; set; }

    public virtual Film? Film { get; set; }

    public virtual Hall? Hall { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
