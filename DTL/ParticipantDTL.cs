using EcfDotnet.Context;
using EcfDotnet.Models;
using EcfDotnet.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace EcfDotnet.DTL
{
    public class ParticipantDTL: GenericRepository<Participant>
    {
        public ParticipantDTL(IServiceProvider provider, Context.MyDbContext @object) : base(provider)
        {
        }

        public override void Add(Participant entity)
        {
        
            var rMemberPosts = Context.RParticipantEvenements
                .Where(rp => rp.FkEvenement == entity.Primarikey)
                .Select(rp => rp.FkParticipant);
        
            base.Add(entity);
        }

        public async Task<Participant> GetParticipantByIdAsyn(Guid id)
        {
            if (Context == null)
            {
                throw new ObjectDisposedException(nameof(MyDbContext), "Le contexte a déjà été disposé.");
            }

            var participant = await Context.Participants
                .FirstOrDefaultAsync(p => p.Primarikey == id); 

            return participant;
        }


        
        public async Task<List<ParticipantViewModel>> GetAllParticipantsAsync()
        {
            return await Context.Participants
                .Select(e => new ParticipantViewModel(e))
                .ToListAsync();
        }



        public void GetService<T>()
        {
            throw new NotImplementedException();
        }
        
    }
}

