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
            var exists = await _client.CollectionExistsAsync("my_collection");

            if(!exists)
            {
                await _client.CreateCollectionAsync("my_collection", new VectorParams
                {
                    Size = 1536,
                    Distance = Distance.Cosine
                });
            }
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

        public async Task<List<string>> Search(float[] queryEmbedding)
        {
            var result = await _client.SearchAsync(
                collectionName: "my_collection",
                vector: queryEmbedding,
                limit: 3
            );
            var texts = new List<string>();

            foreach (var item in result)
            {
                if (item.Payload.TryGetValue("text", out var text))
                {
                    Console.WriteLine(item.Payload["text"]);
                    texts.Add(text.ToString());
                }
                
            }
            return texts;
        }
    }
}
