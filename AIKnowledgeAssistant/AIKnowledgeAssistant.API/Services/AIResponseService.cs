using AIKnowledgeAssistant.API.Interfaces;

namespace AIKnowledgeAssistant.API.Services
{
    public class AIResponseService : IAIResponseService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorDatabaseService _vectorDatabaseService;
        private readonly IOpenAIService _openAIService;
       
        public AIResponseService(IEmbeddingService embeddingService, IVectorDatabaseService vectorDatabaseService, IOpenAIService openAIService)
        {
            _embeddingService = embeddingService;
            _vectorDatabaseService = vectorDatabaseService;
            _openAIService = openAIService;
        }
        public async Task<string> AskQuestionAsync(string question)
        {
            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(question);
            var results = await _vectorDatabaseService.Search(queryEmbedding);
            var matchingContext = BuildContext(results);
            var answer = await _openAIService.GenerateAnswerAsync(matchingContext, question);
            return answer;
        }
        private string BuildContext(List<string> chunks)
        {
            return string.Join("/n", chunks);
        }

    }
}
