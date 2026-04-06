namespace AIKnowledgeAssistant.API.Interfaces
{
    public interface IVectorDatabaseService
    {
        Task CreateCollection();
        Task StoreEmbedding(List<float> embedding, string text);
        Task<List<string>> Search(float[] queryEmbedding);
    }
}
