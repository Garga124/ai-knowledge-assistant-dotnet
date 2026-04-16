namespace AIKnowledgeAssistant.API.Models
{
    public class ChunkMetaData
    {
        public string Text { get; set; }
        public string Name { get; set; }
        public string DocumentId { get; set; }
        public int ChunkIndex { get; set; }
        public double Score { get; set; }

    }
}
