using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AIKnowledgeAssistant.API.Services
{
    public class VectorDatabaseService
    {
        private readonly QdrantClient _client;

        public VectorDatabaseService()
        {
         
            _client = new QdrantClient("localhost",6334);

        }
        public async Task CreateCollection()
        {
            await _client.CreateCollectionAsync("my_collection", new VectorParams
            {
                Size = 1536,
                Distance = Distance.Cosine
            });
        }
        public async Task StoreEmbedding(List<float>embedding, string text)
        {
            await _client.UpsertAsync("my_collection", new[]
            {
        new PointStruct
        {
            Id = Guid.NewGuid(),
            Vectors = new Vectors
            {
                 Vector = embedding.ToArray()
            },
            Payload = 
            {
                { "text", text }
            }
        }
    });
        }

        public async Task Search(List<float> queryEmbedding)
        {
            var result = await _client.SearchAsync(
                collectionName: "documents",
                vector: queryEmbedding.ToArray(),
                limit: 3
            );

            foreach (var item in result)
            {
                Console.WriteLine(item.Payload["text"]);
            }
        }
    }
}
