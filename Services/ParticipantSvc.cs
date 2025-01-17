using EcfDotnet.DTL;
using EcfDotnet.Models;
using EcfDotnet.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace EcfDotnet.Services
{

    public class ParticipantSvc{
        private readonly ParticipantDTL _participantDTL;
        private readonly EvenementDTL _evementDtl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParticipantSvc(IServiceProvider provider, IHttpContextAccessor httpContextAccessor)
        {
            _participantDTL = provider.GetRequiredService<ParticipantDTL>();
            _httpContextAccessor = httpContextAccessor;
            _evementDtl = provider.GetRequiredService<EvenementDTL>();

        }

        public async Task<List<ParticipantViewModel>> GetAllParticipants()
        {
            var events = await _participantDTL.GetAllParticipantsAsync();
            return events;
        }

        public async Task<Participant> GetParticpantByIdAsync(Guid id)
        {
            var evenement = await _participantDTL.GetParticipantByIdAsyn(id);

            if (evenement == null)
            {
                throw new KeyNotFoundException("L'événement spécifié n'existe pas.");
            }

            var result = new Participant()
            {
                Primarikey = evenement.Primarikey,
                Nom = evenement.Nom,
                DateCreation = evenement.DateCreation,
                RParticipantEvenements = evenement.RParticipantEvenements?.Select(rp => new RParticipantEvenement
                {
                    FkParticipant = rp.FkParticipant,
                    DateInscription = rp.DateInscription,
                    FkParticipantNavigation = rp.FkParticipantNavigation
                }).ToList()
            };

            return result;
        }

      public async Task<Participant> AddParticipantEvenement(Participant participant, Guid evenementId)
{
    using (var transaction = await _participantDTL.Context.Database.BeginTransactionAsync())
    {
        try
        {
            var existingParticipant = await _participantDTL.Context.Participants
                .FirstOrDefaultAsync(p => p.Email == participant.Email);

            if (existingParticipant == null)
            {
                _participantDTL.Add(participant);
                await _participantDTL.Context.SaveChangesAsync();  
                existingParticipant = participant;  
            }

            var evenement = await _evementDtl.GetByIdAsync(evenementId);
            if (evenement == null)
            {
                throw new Exception("Événement non trouvé.");
            }

            var rParticipantEvenement = new RParticipantEvenement
            {
                FkParticipant = existingParticipant.Primarikey,
                FkEvenement = evenementId,
                DateInscription = DateTime.UtcNow
            };

            await _participantDTL.Context.RParticipantEvenements.AddAsync(rParticipantEvenement);
            await _participantDTL.Context.SaveChangesAsync();  

            await transaction.CommitAsync();

            var updatedParticipant = await _participantDTL.GetParticipantByIdAsyn(existingParticipant.Primarikey);
            return updatedParticipant;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Une erreur s'est produite lors de l'ajout du participant à l'événement.", ex);
        }
    }
}

    }

}
