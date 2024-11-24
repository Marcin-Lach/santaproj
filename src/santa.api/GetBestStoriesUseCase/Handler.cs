using Microsoft.Extensions.Caching.Hybrid;
using santa.api.HackerRankApiService;

namespace santa.api.GetBestStoriesUseCase;

public static partial class GetBestStories
{
    private static async Task<IResult> Handler(int numberOfStories, HttpContext context, IHackerRankApi hackerRankApi,
        HybridCache cache)
    {
        var bestStoriesIds = await GetBestStoriesIds(hackerRankApi, cache);

        var getStoryDetailsTasks = new List<Task<BestStory>>();

        for (var i = 0; i < numberOfStories; i++)
            getStoryDetailsTasks.Add(GetStoryDetails(bestStoriesIds[i], hackerRankApi, cache));

        await Task.WhenAll(getStoryDetailsTasks);

        return Results.Ok(getStoryDetailsTasks.Select(x => x.Result));
    }

    private static async Task<long[]> GetBestStoriesIds(IHackerRankApi hackerRankApi, HybridCache cache)
    {
        return await cache.GetOrCreateAsync(
            "hr-best-stories-ids",
            hackerRankApi,
            async (hackerRank, entry) => await hackerRank.GetBestStoriesIds(),
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(30),
            }, 
            ["hacker-rank"]);
    }

    private static async Task<BestStory> GetStoryDetails(long storyId, IHackerRankApi hackerRankApi,
        HybridCache cache)
    {
        var storyDetails = await cache.GetOrCreateAsync(
            $"hr-story-{storyId}",
            (hackerRankApi, bestStoriesId: storyId),
            async (state, entry) => await state.hackerRankApi.GetStoryDetails(state.bestStoriesId),
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(30),
            }, 
            ["hacker-rank", "hacker-rank-stories"]);

        return new BestStory(
            storyDetails.Title,
            storyDetails.Url,
            storyDetails.By,
            ConvertFromUnixTime(storyDetails.Time),
            storyDetails.Score,
            storyDetails.kids.Length);
    }

    private static DateTime ConvertFromUnixTime(long unixTime)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTime).ToLocalTime();
        return dateTime;
    }
}