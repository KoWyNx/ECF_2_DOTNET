using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcfDotnet.Models
{
    public class Statistiques
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Guid EvenementId { get; set; }
        public int NombreParticipants { get; set; }
        public DateTime DerniereMiseAJour { get; set; }
    }
}