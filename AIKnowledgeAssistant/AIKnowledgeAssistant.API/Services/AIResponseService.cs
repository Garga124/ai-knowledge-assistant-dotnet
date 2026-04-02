namespace AIKnowledgeAssistant.API.Services
{
    public class AIResponseService
    {
        private readonly EmbeddingService _embeddingService;
        private readonly VectorDatabaseService _vectorDatabaseService;
        private readonly OpenAIService _openAIService;
       
        public AIResponseService()
        {
            _embeddingService = new EmbeddingService();
            _vectorDatabaseService = new VectorDatabaseService();   
            _openAIService = new OpenAIService();
        }
        public async Task<string> AskQuestion(string question)
        {
            var queryEmbedding = await _embeddingService.GenerateEmbedding(question);
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
