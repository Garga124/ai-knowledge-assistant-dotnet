namespace AIKnowledgeAssistant.API.Models
{
    public class APIResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

    }
}
