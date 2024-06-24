using System;
using System.Collections.Generic;

namespace CMSWebAppLab1.Models;

public partial class Cinema
{
    public int Id { get; set; }

    public string CinemaName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();
}
