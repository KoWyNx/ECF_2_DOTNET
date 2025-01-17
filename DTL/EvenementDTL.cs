using EcfDotnet.Context;
using EcfDotnet.Models;
using EcfDotnet.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace EcfDotnet.DTL
{

    public class EvenementDTL : GenericRepository<Evenement>
    {
        public EvenementDTL(IServiceProvider provider, Context.MyDbContext @object) : base(provider)
        {
        }

        public override void Add(Evenement entity)
        {
        
            var rMemberPosts = Context.RParticipantEvenements
                .Where(rp => rp.FkEvenement == entity.Primarikey)
                .Select(rp => rp.FkParticipant);
        
            base.Add(entity);
        }

        public async Task<EvenementViewModel> GetEvenetByIdAsyn(Guid id)
        {
            if (Context == null)
            {
                throw new ObjectDisposedException(nameof(MyDbContext), "Le contexte a déjà été disposé.");
            }

            var evenement = await Context.Evenements 
                .AsNoTracking()
                .Include(p => p.RParticipantEvenements) 
                .ThenInclude(r => r.FkParticipantNavigation) 
                .FirstOrDefaultAsync(p => p.Primarikey == id);

            if (evenement != null)
            {
                if (Context == null)
                {
                    throw new ObjectDisposedException(nameof(MyDbContext), "Le contexte a été disposé après la requête.");
                }

                return new EvenementViewModel(evenement);
            }

            return null;
        }
        


        
        public async Task<List<EvenementViewModel>> GetAllEvenementsAsync()
        {
            return await Context.Evenements
                .Select(e => new EvenementViewModel(e))
                .ToListAsync();
        }



        public void GetService<T>()
        {
            throw new NotImplementedException();
        }
        
    }
}