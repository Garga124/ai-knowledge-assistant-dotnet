namespace AIKnowledgeAssistant.API.Interfaces
{
    public interface IOpenAIService
    {
        Task<string> GenerateAnswerAsync(string matchingContext, string question);
    }
}
