using AIKnowledgeAssistant.API.Interfaces;
using AIKnowledgeAssistant.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//Register Services
builder.Services.AddSingleton<VectorDatabaseService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IVectorDatabaseService, VectorDatabaseService>();
builder.Services.AddScoped<IAIResponseService, AIResponseService>();
builder.Services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//create collection when app starts
using(var scope = app.Services.CreateScope())
{
    var vectorService = scope.ServiceProvider.GetRequiredService<VectorDatabaseService>();  
    await vectorService.CreateCollection();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
