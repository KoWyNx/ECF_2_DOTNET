using System;
using System.Collections.Generic;

namespace EcfDotnet.Models;

public partial class RParticipantEvenement
{
    public Guid FkParticipant { get; set; }

    public Guid FkEvenement { get; set; }

    public DateTime? DateInscription { get; set; }

    public virtual Evenement FkEvenementNavigation { get; set; } = null!;

    public virtual Participant FkParticipantNavigation { get; set; } = null!;
}
