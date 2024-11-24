using Refit;
using santa.api.GetBestStoriesUseCase;
using santa.api.GitHubService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddRefitClient<IHackerRankApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

GetBestStories.Endpoint.MapEndpoint(app);

app.Run();