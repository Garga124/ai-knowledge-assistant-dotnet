namespace AIKnowledgeAssistant.API.Interfaces
{
    public interface IAIResponseService
    {
        Task<string> AskQuestionAsync(string question);
    }
}
