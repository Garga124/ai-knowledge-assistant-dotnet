using AIKnowledgeAssistant.API.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using OpenAI;
using OpenAI.Embeddings;
using OpenAI.Models;
using System.ClientModel.Primitives;

namespace AIKnowledgeAssistant.API.Services
{
    public class EmbeddingService :IEmbeddingService
    {
        private readonly EmbeddingClient _client;
        public EmbeddingService()
        {
            _client = new EmbeddingClient(
    model: "text-embedding-3-small",
    
    apiKey: Environment.GetEnvironmentVariable("OPEN_API_KEY")
    );

        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var embeddings = await _client.GenerateEmbeddingAsync(text);
            var vector = embeddings.Value.ToFloats().ToArray();
            Console.WriteLine("Embedding Length: {vector.Length}" + vector.Length);
            return vector;
        }


    }
}
