using System;
using System.Collections.Generic;

namespace CMSWebAppLab1.Models;

public partial class Person
{
    public int Id { get; set; }

    public string PersonName { get; set; } = null!;

    public string TookPartAs { get; set; } = null!;

    public virtual ICollection<PersonsToMovie> PersonsToMovies { get; set; } = new List<PersonsToMovie>();
}
