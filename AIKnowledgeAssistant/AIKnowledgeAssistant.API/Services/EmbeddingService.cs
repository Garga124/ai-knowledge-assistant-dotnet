using Microsoft.AspNetCore.Components.Forms;
using OpenAI;
using OpenAI.Embeddings;
using OpenAI.Models;
using System.ClientModel.Primitives;

namespace AIKnowledgeAssistant.API.Services
{
    public class EmbeddingService
    {
        private readonly EmbeddingClient _client;
        public EmbeddingService()
        {
            _client = new EmbeddingClient(
                model: "text-embedding-3-small",
                apiKey: ""
                );
        }
        public async Task GenerateEmbedding(string text)
        {
            var embeddings = await _client.GenerateEmbeddingAsync(text);
            var vector = embeddings.Value.ToFloats();
            Console.WriteLine("Embedding Length: {vector.Length}" + vector.Length);
        }
    }
}
