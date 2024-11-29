using Microsoft.Extensions.Caching.Hybrid;
using santa.api.HackerRankApiService;

namespace santa.api.GetBestStoriesUseCase;

public static partial class GetBestStories
{
    private static async Task<IResult> Handler(int numberOfStories, HttpContext context, IHackerRankApi hackerRankApi,
        HybridCache cache)
    {
        if (numberOfStories <= 0)
        {
            return Results.BadRequest("Number of stories must be greater than 0");
        }
        
        var bestStoriesIds = await GetBestStoriesIds(hackerRankApi, cache);

        var bestStories = await GetStoryDetailsTasks(numberOfStories, hackerRankApi, cache, bestStoriesIds);

        return Results.Ok(bestStories);
        
        // streaming implementation, need to add returning a stream instead of single OK result
        // var bestStoriesStreamed = new List<BestStory>();
        // await foreach (var story in GetStoryDetailsTasksWhenAny(numberOfStories, hackerRankApi, cache, bestStoriesIds))
        // {
        //      // somewhere here we should stream the response
        //     bestStoriesStreamed.Add(story);
        // }
    }

    private static async IAsyncEnumerable<BestStory> GetStoryDetailsTasksWhenAny(int numberOfStories, IHackerRankApi hackerRankApi, HybridCache cache,
        long[] bestStoriesIds)
    {
        var getStoryDetailsTasks = new List<Task<BestStory>>();

        for (var i = 0; i < numberOfStories; i++)
            getStoryDetailsTasks.Add(GetStoryDetails(bestStoriesIds[i], hackerRankApi, cache));

        while (getStoryDetailsTasks.Any())
        {
            var resultTask = await Task.WhenAny(getStoryDetailsTasks);
            getStoryDetailsTasks.Remove(resultTask);
         
            yield return resultTask.Result;
        }
    }
    
    private static async Task<List<BestStory>> GetStoryDetailsTasks(int numberOfStories, IHackerRankApi hackerRankApi, HybridCache cache,
        long[] bestStoriesIds)
    {
        var getStoryDetailsTasks = new List<Task<BestStory>>();

        for (var i = 0; i < numberOfStories; i++)
            getStoryDetailsTasks.Add(GetStoryDetails(bestStoriesIds[i], hackerRankApi, cache));

        
        await Task.WhenAll(getStoryDetailsTasks);
        return getStoryDetailsTasks.Select(x => x.Result).ToList();
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
        return await cache.GetOrCreateAsync(
            $"hr-story-{storyId}",
            (hackerRankApi, bestStoriesId: storyId),
            async (state, entry) =>
            {
                var storyDetails = await state.hackerRankApi.GetStoryDetails(state.bestStoriesId);
                return new BestStory(
                    storyDetails.Title,
                    storyDetails.Url,
                    storyDetails.By,
                    ConvertFromUnixTime(storyDetails.Time),
                    storyDetails.Score,
                    storyDetails.kids.Length);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(30),
            }, 
            ["hacker-rank", "hacker-rank-stories"]);
    }

    private static DateTime ConvertFromUnixTime(long unixTime)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTime).ToLocalTime();
        return dateTime;
    }
}