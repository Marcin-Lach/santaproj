using Refit;

namespace santa.api.GitHubService;

[Headers("accept: application/json")]
public interface IHackerRankApi
{
    [Get("/beststories.json")]
    Task<long[]> GetBestStoriesIds();
}