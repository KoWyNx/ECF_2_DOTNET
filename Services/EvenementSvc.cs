using EcfDotnet.DTL;
using EcfDotnet.Models;
using EcfDotnet.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcfDotnet.Services
{
    public class EvenementSvc
    {
        private readonly EvenementDTL _evenementDtl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EvenementSvc(IServiceProvider provider, IHttpContextAccessor httpContextAccessor)
        {
            _evenementDtl = provider.GetRequiredService<EvenementDTL>();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<EvenementViewModel>> GetAllEventsAsync()
        {
            var events = await _evenementDtl.GetAllEvenementsAsync();
            return events;
        }

        public async Task<EvenementViewModel> GetEvenementByIdAsync(Guid id)
        {
            var evenement = await _evenementDtl.GetEvenetByIdAsyn(id);

            if (evenement == null)
            {
                throw new KeyNotFoundException("L'événement spécifié n'existe pas.");
            }

            return evenement;
        }


        public async Task<Evenement> AddEventAsync(EvenementViewModel evenementViewModel, List<RParticipantEvenement> participants)
        {
            try
            {
                var evenement = new Evenement
                {
                    Primarikey = Guid.NewGuid(), 
                    Nom = evenementViewModel.Nom,
                    Description = evenementViewModel.Description,
                    DateDebut = evenementViewModel.DateDebut,
                    DateFin = evenementViewModel.DateFin,
                    Localisation = evenementViewModel.Localisation,
                    DateCreation = DateTime.Now
                };

                var addedEvenement = await _evenementDtl.Context.Evenements.AddAsync(evenement);

                if (participants != null && participants.Any())
                {
                    foreach (var participant in participants)
                    {
                        participant.FkEvenement = addedEvenement.Entity.Primarikey;
                        participant.DateInscription = DateTime.Now; 
                        await _evenementDtl.Context.RParticipantEvenements.AddAsync(participant);
                    }
                }

                await _evenementDtl.Context.SaveChangesAsync();
                return addedEvenement.Entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'événement : {ex.Message}");
                throw; 
            }
        }


        public async Task<Evenement> UpdateEventAsync(Guid id, EvenementViewModel evenementViewModel)
        {
            var evenementViewModelExist = await _evenementDtl.GetEvenetByIdAsyn(id);

            if (evenementViewModelExist == null)
            {
                throw new KeyNotFoundException("L'événement spécifié n'existe pas.");
            }

            var existingEvenement = MapToEvenement(evenementViewModel, evenementViewModelExist);

            _evenementDtl.Context.Evenements.Update(existingEvenement);
            await _evenementDtl.Context.SaveChangesAsync();

            // Suppression de la gestion des participants

            return existingEvenement;
        }


        

        public async Task DeleteEventAsync(Guid id)
        {
            var evenementViewModel = await _evenementDtl.GetEvenetByIdAsyn(id);

            if (evenementViewModel == null)
            {
                throw new KeyNotFoundException("L'événement spécifié n'existe pas.");
            }

            var evenement = MapToEvenement(evenementViewModel);

            var entry = _evenementDtl.Context.Entry(evenement);
            if (entry.State == EntityState.Detached)
            {
                _evenementDtl.Context.Evenements.Attach(evenement);  
            }

            var participants = _evenementDtl.Context.RParticipantEvenements
                .Where(rp => rp.FkEvenement == id)
                .ToList();

            _evenementDtl.Context.RParticipantEvenements.RemoveRange(participants);

            _evenementDtl.Context.Evenements.Remove(evenement);

            await _evenementDtl.Context.SaveChangesAsync();
        }


        
        private Evenement MapToEvenement(EvenementViewModel viewModel, EvenementViewModel existingViewModel)
        {
            var evenement = new Evenement
            {
                Primarikey = existingViewModel.Primarikey,
                Nom = viewModel.Nom,
                Description = viewModel.Description,
                DateDebut = viewModel.DateDebut,
                DateFin = viewModel.DateFin,
                Localisation = viewModel.Localisation
            };

            return evenement;
        }
        
        private Evenement MapToEvenement(EvenementViewModel viewModel)
        {
            return new Evenement
            {
                Primarikey = viewModel.Primarikey, 
                Nom = viewModel.Nom,
                Description = viewModel.Description,
                DateDebut = viewModel.DateDebut,
                DateFin = viewModel.DateFin,
                Localisation = viewModel.Localisation
            };
        }

    }
}