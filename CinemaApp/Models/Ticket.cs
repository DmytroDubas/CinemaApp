using System;
using System.Collections.Generic;

namespace CinemaApp.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int? Sessionid { get; set; }

    public int? Seatnumber { get; set; }

    public int? Customerid { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Session? Session { get; set; }
}
