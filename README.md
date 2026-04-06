# ai-knowledge-assistant-dotnet
A system that allows users to upload documents and ask questions. The system uses embeddings and vector search to retrieve relevant information and generate AI-powered answers.

## RAG Pipeline

This project implements a Retrieval-Augmented Generation (RAG) pipeline to improve the accuracy of AI responses.

Instead of relying only on the language model, the assistant retrieves relevant information from a knowledge source before generating a response.

Pipeline steps:

1. User submits a question
2. The system retrieves relevant documents from the knowledge store
3. Retrieved context is injected into the prompt
4. The LLM generates an answer using the retrieved knowledge

Benefits:
- Provides answers based on user-uploaded knowledge
- Reduces hallucinations from the language model
- Enables document-based question answering
- Improves accuracy
- Scales to large knowledge collections using vector search
- Allows domain-specific knowledge integration

## Architecture

The architecture follows a layered design with dependency injection and modular components.

Architecture diagram (to be added):
  
# Tech Stack:
- .NET 8 Web API
- OpenAI API
- Vector Database
- Docker
