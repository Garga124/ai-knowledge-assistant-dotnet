namespace AIKnowledgeAssistant.API.Interfaces
{
    public interface IDocumentProcessingService
    {
        Task ProcessDocument(IFormFile file);
        string ExtractTextFromPdf(string filePath);
    }
}
