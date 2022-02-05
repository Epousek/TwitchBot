using System.Threading.Tasks;
using Octokit;

namespace TwitchBot.Connections
{
  public static class GitHub
  {
    private const long  RepoId = 407855144;
    private static readonly ApiOptions options = new ApiOptions
    {
      PageCount = 1,
      PageSize = 1
    };
    private static GitHubClient _client;

    public static void Init()
    {
      _client = new GitHubClient(new ProductHeaderValue("twitch-bot"));
    }

    public static async Task<GitHubCommit> GetLastCommitAsync()
    {
      var allCommits = await _client.Repository.Commit.GetAll(RepoId, options);
      return allCommits[0];
    }

    public static async Task<Release> GetLastReleaseAsync()
    {
      return await _client.Repository.Release.GetLatest(RepoId);
    }
  }
}
