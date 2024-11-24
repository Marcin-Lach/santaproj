using santa.api.GitHubService;

namespace santa.api.GetBestStoriesUseCase;

public static partial class GetBestStories
{
    private static async Task<IResult> Handler(int numberOfStories, HttpContext context, IHackerRankApi hackerRankApi)
    {
        var bestStoriesIds = await hackerRankApi.GetBestStoriesIds();

        var getStoryDetailsTasks = new List<Task<BestStory>>();
        
        for (var i = 0; i < numberOfStories; i++)
        {
            getStoryDetailsTasks.Add(GetStoryDetails(bestStoriesIds[i], hackerRankApi));
        }
        
        await Task.WhenAll(getStoryDetailsTasks);
        
        return Results.Ok(getStoryDetailsTasks.Select(x => x.Result));
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