using EcfDotnet.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EcfDotnet.Services
{
    public class MongoDBSvc
    {
        private readonly IMongoCollection<Statistiques> _statistiquesCollection;

        public MongoDBSvc(IOptions<MongoDBSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _statistiquesCollection = mongoDatabase.GetCollection<Statistiques>(mongoDbSettings.Value.CollectionName);
        }

        public async Task<List<Statistiques>> GetAllStatistiquesAsync()
        {
            return await _statistiquesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Statistiques> GetStatistiquesByEvenementIdAsync(Guid evenementId)
        {
            return await _statistiquesCollection.Find(s => s.EvenementId == evenementId).FirstOrDefaultAsync();
        }

        public async Task CreateStatistiqueAsync(Statistiques statistiques)
        {
            await _statistiquesCollection.InsertOneAsync(statistiques);
        }

        public async Task UpdateStatistiqueAsync(string id, Statistiques statistiques)
        {
            await _statistiquesCollection.ReplaceOneAsync(s => s.Id == id, statistiques);
        }

        public async Task DeleteStatistiqueAsync(string id)
        {
            await _statistiquesCollection.DeleteOneAsync(s => s.Id == id);
        }
    }
}