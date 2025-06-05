using Azure.Data.Tables;
using GeminiIntegration.Interface;
using GeminiIntegration.Models;
using GeminiIntegration.ModelServices;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Register HttpClient for GeminiServices
builder.Services.AddHttpClient<GeminiServices>();

// Register GeminiService
builder.Services.AddTransient<IGeminiService, GeminiServices>();

// Register ChatHistory Table Storage service
builder.Services.AddTransient<Func<ITableStorage<ChatHistory>>>(provider =>
{
    return () =>
    {
        var serviceClient = new TableServiceClient("UseDevelopmentStorage=true");
        return new TableStorageService<ChatHistory>(serviceClient, "ChatHistory", new LoggerFactory());
    };
});

// Register ChatEmbedding Table Storage service
builder.Services.AddTransient<Func<ITableStorage<ChatEmbedding>>>(provider =>
{
    return () =>
    {
        var serviceClient = new TableServiceClient("UseDevelopmentStorage=true");
        return new TableStorageService<ChatEmbedding>(serviceClient, "ChatEmbedding", new LoggerFactory());
    };
});

// Register Azure SDK clients (Blob, Queue, Table)
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection:blobServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnection:queueServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnection:tableServiceUri"]!).WithName("StorageConnection");
});

// Register MVC controllers
builder.Services.AddControllers();

// Swagger support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();