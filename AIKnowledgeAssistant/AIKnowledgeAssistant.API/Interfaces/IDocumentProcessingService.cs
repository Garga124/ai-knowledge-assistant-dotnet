namespace AIKnowledgeAssistant.API.Interfaces
{
    public interface IDocumentProcessingService
    {
        Task ProcessDocument(IFormFile file, string filePath);
        string ExtractTextFromPdf(string filePath);
    }
}
