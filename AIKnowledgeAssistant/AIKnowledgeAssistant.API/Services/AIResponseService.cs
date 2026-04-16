using AIKnowledgeAssistant.API.Interfaces;
using AIKnowledgeAssistant.API.Models;

namespace AIKnowledgeAssistant.API.Services
{
    public class AIResponseService : IAIResponseService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorDatabaseService _vectorDatabaseService;
        private readonly IOpenAIService _openAIService;
        private readonly ILogger<AIResponseService> _logger;
       
        public AIResponseService(IEmbeddingService embeddingService, IVectorDatabaseService vectorDatabaseService, IOpenAIService openAIService, ILogger<AIResponseService> logger)
        {
            _embeddingService = embeddingService;
            _vectorDatabaseService = vectorDatabaseService;
            _openAIService = openAIService;
            _logger = logger;
        }
        public async Task<AIResponse> AskQuestionAsync(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                _logger.LogWarning("Empty question received.");
                throw new ArgumentException("Question cannot be empty.");
            }
            _logger.LogInformation("Received question: {Question}", question);

            try
            {
                //Step 1: Generate embedding for query
                _logger.LogDebug("Generating embedding for question");
                var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(question);
                _logger.LogInformation("Query embedding generated. Vector length: {VectorLength}", queryEmbedding.Length);
                //Step 2: Perform Vector Search
                _logger.LogDebug("Searching Vector database for question");
                var results = await _vectorDatabaseService.Search(queryEmbedding);
                _logger.LogInformation("Vector search returned {ResultsCount} context chunks", results.Count);
                //Step 3: Build Context
                var matchingContext = BuildContext(results.Select(r => r.Text).ToList());
                _logger.LogDebug("Context built for LLM Prompt");
                //Step 4 : Build Sources
                // Build sources
                var sources = results.Select(r =>
                    $"{r.Name} (Chunk {r.ChunkIndex} (Score{r.Score:F2}))"
                ).ToList();

                
                _logger.LogDebug("Sources built for LLM Prompt");
                //Step 4: Generate AI response
                _logger.LogInformation("Sending prompt to LLM");
                var answer = await _openAIService.GenerateAnswerAsync(matchingContext, question);
                AIResponse response = new AIResponse
                {
                    Answer = answer,
                    Sources = sources
                };
                _logger.LogInformation("AI response generated successfully");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while generating AI response for question");
                throw;
            }
        }

       

        private string BuildContext(List<string> chunks)
        {
            if(chunks == null || chunks.Count == 0)
            {
                _logger.LogWarning("No matching context found for query.");
                return string.Empty;
            }
            _logger.LogDebug("Building context from {chunkCount} Chunks",chunks.Count);
            return string.Join("\n", chunks);
        }

    }
}
