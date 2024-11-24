namespace santa.api.GetBestStoriesUseCase;

public static partial class GetBestStories
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
}