using santa.api.GitHubService;
using santa.api.GitHubService.Models;

namespace santa.api.GetBestStoriesUseCase;

public static partial class GetBestStories
{
    private static async Task<IResult> Handler(int numberOfStories, HttpContext context, IHackerRankApi hackerRankApi)
    {
        var bestStoriesIds = await hackerRankApi.GetBestStoriesIds();

        var bestStories = new List<BestStory>();

        for (var i = 0; i < numberOfStories; i++)
        {
            bestStories.Add(await GetStoryDetails(bestStoriesIds[i], hackerRankApi));
        }
        
        return Results.Ok(bestStories);
    }

    private static async Task<BestStory> GetStoryDetails(long bestStoriesId, IHackerRankApi hackerRankApi)
    {
        var storyDetails = await hackerRankApi.GetStoryDetails(bestStoriesId);
        
        return new BestStory (
            storyDetails.Title,
            storyDetails.Url,
            storyDetails.By, 
            ConvertFromUnixTime(storyDetails.Time),
            storyDetails.Score,
            storyDetails.kids.Length);
    }

    private static DateTime ConvertFromUnixTime(long unixTime)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds( unixTime ).ToLocalTime();
        return dateTime;
    }
}