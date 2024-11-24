using Refit;
using santa.api.HackerRankApiService.Models;

namespace santa.api.HackerRankApiService;

[Headers("accept: application/json")]
public interface IHackerRankApi
{
    [Get("/beststories.json")]
    Task<long[]> GetBestStoriesIds();
    
    [Get("/item/{id}.json")]
    Task<StoryDetails> GetStoryDetails(long id);
}