using AIKnowledgeAssistant.API.Models;

namespace AIKnowledgeAssistant.API.Interfaces
{
    public interface IAIResponseService
    {
        Task <AIResponse> AskQuestionAsync(string question);
    }
}
