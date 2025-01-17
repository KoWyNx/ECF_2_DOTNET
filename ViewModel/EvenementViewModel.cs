using EcfDotnet.Models;

namespace EcfDotnet.ViewModel
{

    public class EvenementViewModel
    {
        public Guid Primarikey { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        
        public DateTime? DateDebut { get; set; }
        
        public DateTime? DateFin { get; set; }
        
        public string Localisation { get; set; }
        
        public Guid FkParticipant { get; set; }
        
        public List<ParticipantViewModel> Participant { get; set; }
        
        public DateTime? DateCreation { get; set; }
        



        public EvenementViewModel()
        {
        }
        

        public EvenementViewModel(Evenement evenement)
        {
            ParseFromPocs(evenement);
        }

        public void ParseFromPocs(Evenement evenement)
        {
            Primarikey = evenement.Primarikey;
            Nom = evenement.Nom;
            Description = evenement.Description;
            DateDebut = evenement.DateDebut;
            DateFin = evenement.DateFin;
            Localisation = evenement.Localisation;
            FkParticipant = evenement.RParticipantEvenements
                .FirstOrDefault()?.FkParticipant ?? Guid.Empty;
            
            Participant = evenement.RParticipantEvenements
                .Select(rp => new ParticipantViewModel(rp.FkParticipantNavigation)) 
                .ToList();
            DateCreation = evenement.DateCreation;
        }
        
        
    }
}