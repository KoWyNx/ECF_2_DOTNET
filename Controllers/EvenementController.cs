using EcfDotnet.Services;
using EcfDotnet.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcfDotnet.Models;

namespace EcfDotnet.Controllers
{
    public class EvenementController : Controller
    {
        private readonly EvenementSvc _evenementSvc;
        private readonly ParticipantSvc _participantSvc;

        public EvenementController(EvenementSvc evenementSvc, ParticipantSvc participantSvc)
        {
            _evenementSvc = evenementSvc;
            _participantSvc = participantSvc;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _evenementSvc.GetAllEventsAsync();
                return View(events);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Une erreur s'est produite lors de la récupération des événements.";
                return View(new List<EvenementViewModel>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var evenement = await _evenementSvc.GetEvenementByIdAsync(id);
                if (evenement == null)
                {
                    return NotFound();
                }
                return View(evenement);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Une erreur s'est produite lors de la récupération de l'événement.";
                return View();
            }
        }

        public async Task<IActionResult> Create()
        {
            var model = new EvenementViewModel
            {
                Participant = await _participantSvc.GetAllParticipants()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EvenementViewModel evenementViewModel)
        {
            try
            {
              
                    var evenement = new EvenementViewModel()
                    {
                        Nom = evenementViewModel.Nom,
                        Description = evenementViewModel.Description,
                        DateDebut = evenementViewModel.DateDebut,
                        DateFin = evenementViewModel.DateFin,
                        Localisation = evenementViewModel.Localisation,
                        DateCreation = DateTime.Now,
                        FkParticipant = evenementViewModel.FkParticipant,
                        Participant = new List<ParticipantViewModel>()
                        {
                            new ParticipantViewModel
                            {
                                Primarikey = evenementViewModel.FkParticipant,  
                            }
                        }
                        
                    };

                    var participants = new List<RParticipantEvenement>();

                    if (evenementViewModel.FkParticipant != Guid.Empty)
                    {
                        participants.Add(new RParticipantEvenement
                        {
                            FkParticipant = evenementViewModel.FkParticipant,
                            DateInscription = DateTime.Now
                        });
                    }

                    await _evenementSvc.AddEventAsync(evenement, participants);

                    return RedirectToAction(nameof(Index));
                


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Une erreur s'est produite lors de la création de l'événement.";
                return View(evenementViewModel);
            }
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var evenement = await _evenementSvc.GetEvenementByIdAsync(id);
                if (evenement == null)
                {
                    return NotFound();
                }
                return View(evenement);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Une erreur s'est produite lors de la récupération de l'événement pour modification.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EvenementViewModel evenement)
        {
            if (id != evenement.Primarikey)
            {
                return BadRequest("L'ID de l'événement ne correspond pas.");
            }

         
                try
                {
                    await _evenementSvc.UpdateEventAsync(id, evenement);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la mise à jour de l'événement : {ex.Message}");
                    ViewBag.ErrorMessage = "Une erreur s'est produite lors de la mise à jour de l'événement.";
                    return View(evenement);
                }
            
                
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, string _method)
        {
            if (_method != "DELETE")
            {
                return BadRequest(); 
            }

            try
            {
                var evenement = await _evenementSvc.GetEvenementByIdAsync(id);
                if (evenement == null)
                {
                    return NotFound();
                }

                await _evenementSvc.DeleteEventAsync(id); 

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Une erreur s'est produite lors de la suppression de l'événement.";
                return View();
            }
        }
        
        public async Task<IActionResult> AjouterParticipant(Guid id)
        {
            try
            {
                var evenement = await _evenementSvc.GetEvenementByIdAsync(id);
                if (evenement == null)
                {
                    return NotFound();
                }

                var model = new ParticipantViewModel
                {
                    FkEvenement = id
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Une erreur s'est produite lors du chargement de l'événement.";
                return View();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> AjouterParticipant(ParticipantViewModel model)
        {
            try
            {
                var participant = new Participant
                {
                    Primarikey = Guid.NewGuid(),  
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    Email = model.Email,
                    Age = model.Age,
                    Telephone = model.Telephone,
                    DateCreation = DateTime.Now
                };

                var evenement = await _evenementSvc.GetEvenementByIdAsync(model.FkEvenement);
                if (evenement == null)
                {
                    return NotFound();
                }

                var updatedParticipant = await _participantSvc.AddParticipantEvenement(participant, model.FkEvenement);

                return RedirectToAction("Details", new { id = model.FkEvenement });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Participant déjà présent dans l'événement.";
                return View(model);
            }
        }




        

    }
}
