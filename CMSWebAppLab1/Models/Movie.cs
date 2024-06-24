using System;
using System.Collections.Generic;

namespace CMSWebAppLab1.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public short Duration { get; set; }

    public string Genre { get; set; } = null!;

    public int ReleaseYear { get; set; }

    public virtual ICollection<PersonsToMovie> PersonsToMovies { get; set; } = new List<PersonsToMovie>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
