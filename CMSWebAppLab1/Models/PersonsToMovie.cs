using System;
using System.Collections.Generic;

namespace CMSWebAppLab1.Models;

public partial class PersonsToMovie
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int PersonId { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
