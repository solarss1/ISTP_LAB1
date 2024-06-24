using System;
using System.Collections.Generic;

namespace CMSWebAppLab1.Models;

public partial class Hall
{
    public int Id { get; set; }

    public int CinemaId { get; set; }

    public string HallName { get; set; } = null!;

    public int MaxPlaces { get; set; }

    public virtual Cinema Cinema { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
