using System;
using System.Collections.Generic;

namespace CMSWebAppLab1.Models;

public partial class Session
{
    public int Id { get; set; }

    public int HallId { get; set; }

    public int MovieId { get; set; }

    public DateTime StartTime { get; set; }

    public decimal Price { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

public class SessionSearchViewModel
{
    public string Title { get; set; }
    public string ActorName { get; set; }
    public string DirectorName { get; set; }
    public List<SessionResultViewModel> Sessions { get; set; }
}

public class SessionResultViewModel
{
    public string Title { get; set; }
    public string ActorName { get; set; }
    public string DirectorName { get; set; }
    public DateTime StartTime { get; set; }
    public decimal Price { get; set; }
    public string CinemaName { get; set; }
    public string HallName { get; set; }
}
