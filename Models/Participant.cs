using System;
using System.Collections.Generic;

namespace EcfDotnet.Models;

public partial class Participant
{
    public Guid Primarikey { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Email { get; set; }

    public int? Age { get; set; }

    public int? Telephone { get; set; }

    public DateTime? DateCreation { get; set; }

    public virtual ICollection<RParticipantEvenement> RParticipantEvenements { get; set; } = new List<RParticipantEvenement>();
}
