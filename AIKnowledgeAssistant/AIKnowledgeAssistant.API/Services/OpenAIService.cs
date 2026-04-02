using AIKnowledgeAssistant.API.Interfaces;
using OpenAI;
using OpenAI.Chat;

namespace AIKnowledgeAssistant.API.Services
{
    public class OpenAIService :IOpenAIService
    {
        private readonly string _apiKey;
        public OpenAIService()
        {
        _apiKey: Environment.GetEnvironmentVariable("OPEN_API_KEY");
        }

        public async Task<string> GenerateAnswerAsync(string matchingContext, string question)
        {
            var prompt = BuildPrompt(matchingContext, question);
            var client = new OpenAIClient(_apiKey);
            var chatClient = client.GetChatClient("gpt-4o-mini");
            var completion = await chatClient.CompleteChatAsync(

                new ChatMessage[]
                {
                    ChatMessage.CreateSystemMessage(prompt)
                });
            Console.WriteLine(completion.Value.Content[0].Text);
            return completion.Value.Content[0].Text;
        }

        
        private string BuildPrompt(string matchingContext, string question)
        {
            return $@"
        You are an AI assistant.

        Answer the question using ONLY the context below.

        Context:
        {matchingContext}

        Question:
        {question}

        If the answer is not in the context, say:
        'I could not find the answer in the provided documents.'
        ";

        }

    }
}
