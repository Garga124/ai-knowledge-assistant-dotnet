using AIKnowledgeAssistant.API.Interfaces;
using AIKnowledgeAssistant.API.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AIKnowledgeAssistant.API.Services
{
    public class VectorDatabaseService : IVectorDatabaseService
    {
        private readonly QdrantClient _client;
        private readonly ILogger<VectorDatabaseService> _logger;
        private const string CollectionName = "my_collection";


        public VectorDatabaseService(ILogger<VectorDatabaseService> logger)
        {
            _logger = logger;
            _client = new QdrantClient("localhost", 6334);
            _logger.LogInformation("VectorDatabaseService initialized with Qdrant at localhost:6334");

        }
        public async Task CreateCollection()
        {
            _logger.LogInformation("Checking if collection {CollectionName} exists", CollectionName);

            var exists = await _client.CollectionExistsAsync(CollectionName);

            if (!exists)
            {
                _logger.LogInformation("Collection {CollectionName} does not exist. Creating collection.", CollectionName);

                await _client.CreateCollectionAsync(CollectionName, new VectorParams
                {
                    Size = 1536,
                    Distance = Distance.Cosine
                });
                _logger.LogInformation("Collection {CollectionName} created successfully", CollectionName);
            }
            else
            {
                _logger.LogInformation("Collection {CollectionName} already exists", CollectionName);
            }
            

        }
        public async Task StoreEmbedding(List<float>embedding, string chunk, FileMetaData metaData)
        {
            if (embedding == null || embedding.Count == 0)
            {
                _logger.LogWarning("Attempted to store empty embedding.");
                return;
            }
            try
            {
                _logger.LogDebug("Storing embedding with vector length {VectorLength}", embedding.Count);

                await _client.UpsertAsync(CollectionName, new[]
                {
                    new PointStruct
                    {
                    Id = Guid.NewGuid(),
                    Vectors = new Vectors
                    {
                         Vector = embedding.ToArray()
                    },
                    Payload =
                    {
                        { "text", chunk },
                        {"fileName" ,metaData.Name},
                        { "chunkIndex",metaData.ChunkIndex},
                        {"documentId",metaData.DocumentId },
                        { "uploadedAt",DateTime.UtcNow.ToString()}
                           

                    }
                }
            });
                _logger.LogInformation("Embedding stored successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store embedding in vector database");

                throw;
            }
        }

        public async Task<List<ChunkMetaData>> Search(float[] queryEmbedding)
        {
            if (queryEmbedding == null || queryEmbedding.Length == 0)
            {
                _logger.LogWarning("Search attempted with empty query embedding.");
                return new List<ChunkMetaData>();
            }
            try
            {
                _logger.LogInformation("Performing vector search");
                var result = await _client.SearchAsync(
                       collectionName: CollectionName,
                       vector: queryEmbedding,
                       limit: 3
                   );
                _logger.LogInformation("Vector search returned {ResultCount} results", result.Count);

                
                return result.Select(r => MapToChunkMetaData(r.Payload,r.Score)).ToList();
                //TODO: Filter low-quality matches
                var filteredData = result.Where(r => r.Score >= 0.35).OrderByDescending(r => r.Score).ToList();
                return filteredData.Select(r => MapToChunkMetaData(r.Payload,r.Score)).ToList();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vector search failed");

                throw;
            }
        }

            private ChunkMetaData MapToChunkMetaData(Google.Protobuf.Collections.MapField<string, Qdrant.Client.Grpc.Value> payload, double score)
        {
            return new ChunkMetaData
            {
                Text = GetString(payload, "text"),
                Name = GetString(payload, "fileName"),
                DocumentId = GetString(payload, "documentId"),
                ChunkIndex = GetInt(payload, "chunkIndex"),
                Score = score
                
                
            };
        }
        private string GetString(Google.Protobuf.Collections.MapField<string, Qdrant.Client.Grpc.Value> payload,string key)
        {
            return payload.ContainsKey(key)
                ? payload[key].StringValue
                : string.Empty;
        }
        private int GetInt(Google.Protobuf.Collections.MapField<string, Qdrant.Client.Grpc.Value> payload,string key)
        {
            return payload.ContainsKey(key)
                ? (int)payload[key].IntegerValue
                : 0;
        }
      
    }
    }

