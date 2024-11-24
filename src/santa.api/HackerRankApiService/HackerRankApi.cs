using Refit;
using santa.api.GitHubService.Models;

namespace santa.api.GitHubService;

[Headers("accept: application/json")]
public interface IHackerRankApi
{
    [Get("/beststories.json")]
    Task<long[]> GetBestStoriesIds();
    
    [Get("/item/{id}.json")]
    Task<StoryDetails> GetStoryDetails(long id);
}