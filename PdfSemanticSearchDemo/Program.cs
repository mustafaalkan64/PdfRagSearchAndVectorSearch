using Qdrant.Client;
using PdfSemanticSearchDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add HTTP client for Ollama
builder.Services.AddHttpClient<IOllamaService, OllamaService>();
builder.Services.AddHttpClient<IRagService, RagService>();

// Add Qdrant client
builder.Services.AddSingleton<QdrantClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var envValue = Environment.GetEnvironmentVariable("QDRANT_API_URL");

    var qdrantUrl = !string.IsNullOrEmpty(envValue) ? envValue : "localhost";
    return new QdrantClient(qdrantUrl, 6334);
});

// Add services
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IOllamaService, OllamaService>();
builder.Services.AddScoped<IVectorService, QdrantVectorService>();
builder.Services.AddScoped<IRagService, RagService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("AllowReactApp");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Initialize vector database
using (var scope = app.Services.CreateScope())
{
    var vectorService = scope.ServiceProvider.GetRequiredService<IVectorService>();
    await vectorService.InitializeAsync();
}

app.Run();
