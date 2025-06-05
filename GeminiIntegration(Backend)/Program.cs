using Azure.Data.Tables;
using GeminiIntegration.Interface;
using GeminiIntegration.Models;
using GeminiIntegration.ModelServices;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddTransient<IGeminiService, GeminiService>();

builder.Services.AddTransient<Func<ITableStorage<ChatHistory>>>(provider =>
{
    return () =>
    {
        var serviceClient = new TableServiceClient("UseDevelopmentStorage=true");
        return new TableStorageService<ChatHistory>(serviceClient, "ChatHistory", new Microsoft.Extensions.Logging.LoggerFactory());
    };
});

// Add Azure clients
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection:blobServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnection:queueServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnection:tableServiceUri"]!).WithName("StorageConnection");
});

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
