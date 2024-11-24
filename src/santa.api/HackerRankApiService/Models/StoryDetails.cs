namespace santa.api.HackerRankApiService.Models;

public record StoryDetails(string Title, string Url, string By, long Time, int Score, int[] kids);