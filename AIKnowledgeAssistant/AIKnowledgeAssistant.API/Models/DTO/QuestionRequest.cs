using System.ComponentModel.DataAnnotations;

namespace AIKnowledgeAssistant.API.Models.DTO
{
    public class QuestionRequest
    {
        [Required]
        [MaxLength(1000)]
        public string Question { get; set; }

    }
}
