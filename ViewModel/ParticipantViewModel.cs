using EcfDotnet.Models;

namespace EcfDotnet.ViewModel;

public class ParticipantViewModel
{
    public Guid Primarikey { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Email { get; set; }

    public int? Age { get; set; }

    public int? Telephone { get; set; }

    public DateTime? DateCreation { get; set; }
    
    public Guid FkEvenement { get; set; }
    
    public ParticipantViewModel()
    {
    }

    public ParticipantViewModel(Participant participant)
    {
        ParseFromPocs(participant);
    }

    public void ParseFromPocs(Participant participant)
    {
        Primarikey = participant.Primarikey;
        Nom = participant.Nom;
        Prenom = participant.Prenom;
        Email = participant.Email;
        Age = participant.Age;
        Telephone = participant.Telephone;
        DateCreation = participant.DateCreation;
        FkEvenement = participant.RParticipantEvenements.FirstOrDefault()?.FkEvenement ?? Guid.Empty;        
    }
}