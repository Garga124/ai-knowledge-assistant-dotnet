using AIKnowledgeAssistant.API.Models;

namespace AIKnowledgeAssistant.API.Interfaces
{
    public interface IVectorDatabaseService
    {
        Task CreateCollection();
        Task StoreEmbedding(List<float> embedding, string text, FileMetaData metaData);
        Task<List<ChunkMetaData>> Search(float[] queryEmbedding);
    }
}
