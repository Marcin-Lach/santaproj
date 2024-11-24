using santa.api.GitHubService;

namespace santa.api.GetBestStoriesUseCase;

public static partial class GetBestStories
{
    private static async Task<IResult> Handler(int numberOfStories, HttpContext context, IHackerRankApi hackerRankApi)
    {
        var bestStories = Enumerable.Range(1, numberOfStories).Select(index =>
                new BestStory
                (
                    $"Title{index}",
                    "HR stories count",
                    "PostedBy",
                    DateTime.Now.AddDays(index),
                    Random.Shared.Next(20, 55),
                    Random.Shared.Next(20, 55)
                ))
            .ToArray();
        return Results.Ok(bestStories);
    }
}