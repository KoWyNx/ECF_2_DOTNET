using System;
using System.Collections.Generic;

namespace EcfDotnet.Models;

public partial class Evenement
{
    public Guid Primarikey { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public string? Localisation { get; set; }

    public DateTime? DateCreation { get; set; }

    public virtual ICollection<RParticipantEvenement> RParticipantEvenements { get; set; } = new List<RParticipantEvenement>();
}
