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
        private readonly ILogger<EmbeddingService> _logger;

        public EmbeddingService(ILogger<EmbeddingService> logger)
        {
            _logger = logger;
            var apiKey = Environment.GetEnvironmentVariable("OPEN_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("OpenAI API key is not configured in environment variables.");
                throw new InvalidOperationException("Missing OpenAI API key");
            }
            _client = new EmbeddingClient(
                model: "text-embedding-3-small",
                apiKey: apiKey
                );
            _logger.LogInformation("EmbeddingService initialized using model {Model}", "text-embedding-3-small");

        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("Attempted to generate embedding for empty text.");
                throw new ArgumentException("Text cannot be empty");
            }
            try
            {
                _logger.LogDebug("Generating embedding for text length {TextLength}", text.Length);

                var embeddings = await _client.GenerateEmbeddingAsync(text);
                var vector = embeddings.Value.ToFloats().ToArray();
                _logger.LogInformation("Embedding generated successfully. Vector length: {VectorLength}", vector.Length);
                return vector;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate embedding.");
                throw;
            }
        }
    }
}
