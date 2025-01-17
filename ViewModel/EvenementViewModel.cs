using EcfDotnet.Models;

namespace EcfDotnet.ViewModel
{

    public class EvenementViewModel
    {
        public Guid Primarikey { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }



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
        }
    }
}