using EcfDotnet.Models;
using EcfDotnet.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcfDotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatistiquesController : ControllerBase
    {
        private readonly MongoDBSvc _mongoDBService;

        public StatistiquesController(MongoDBSvc mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStatistiques()
        {
            var stats = await _mongoDBService.GetAllStatistiquesAsync();
            return Ok(stats);
        }

        [HttpGet("{evenementId:guid}")]
        public async Task<IActionResult> GetStatistiquesByEvenement(Guid evenementId)
        {
            var stat = await _mongoDBService.GetStatistiquesByEvenementIdAsync(evenementId);
            if (stat == null) return NotFound();

            return Ok(stat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStatistique([FromBody] Statistiques statistique)
        {
            await _mongoDBService.CreateStatistiqueAsync(statistique);
            return CreatedAtAction(nameof(GetStatistiquesByEvenement), new { evenementId = statistique.EvenementId }, statistique);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatistique(string id, [FromBody] Statistiques statistique)
        {
            var existingStat = await _mongoDBService.GetStatistiquesByEvenementIdAsync(statistique.EvenementId);
            if (existingStat == null) return NotFound();

            statistique.Id = id;
            await _mongoDBService.UpdateStatistiqueAsync(id, statistique);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatistique(string id)
        {
            var stat = await _mongoDBService.GetStatistiquesByEvenementIdAsync(Guid.NewGuid());
            if (stat == null) return NotFound();

            await _mongoDBService.DeleteStatistiqueAsync(id);
            return NoContent();
        }
    }
}