namespace santa.api.GetBestStoriesUseCase;

public static class GetBestStories
{
    private record BestStory(string Title, string Uri, string PostedBy, DateTime Time, int Score, int CommentCount);

    public static class Endpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/beststories", Handler)
                .WithName("GetBestStories");
        }
    }

    private static IResult Handler(HttpContext context)
    {
        var bestStories = Enumerable.Range(1, 5).Select(index =>
                new BestStory
                (
                    $"Title{index}",
                    "Uri",
                    "PostedBy",
                    DateTime.Now.AddDays(index),
                    Random.Shared.Next(20, 55),
                    Random.Shared.Next(20, 55)
                ))
            .ToArray();
        return Results.Ok(bestStories);
    }
}