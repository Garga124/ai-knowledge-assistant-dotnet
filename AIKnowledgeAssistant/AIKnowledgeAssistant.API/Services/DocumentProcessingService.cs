using UglyToad.PdfPig;

namespace AIKnowledgeAssistant.API.Services
{
    public class DocumentProcessingService
    {
        public DocumentProcessingService() { }
        public string ExtractTextFromPdf(string filePath)
        {
            using (var document = PdfDocument.Open(filePath))
            {
                var text = "";
                foreach(var page in document.GetPages())
                {
                    text += page.Text;
                }
                return text;
            }
               

        }
        public List<string> SplitIntoChunks(string text, int chunkSize)
        {
            var words = text.Split(' ');
            var chunks = new List<string>();
            for(int i =0;i<words.Length;i+= chunkSize)
            {
                var chunk = string.Join(" ", words.Skip(i).Take(chunkSize));
                chunks.Add(chunk);
            }
            Console.WriteLine("Extracted " + chunks.Count + " Chunks");
            foreach(var chunk in chunks)
            {
                Console.WriteLine("Chunks : " + chunk);
            }
            return chunks;

        }
    }
}
