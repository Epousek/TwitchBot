using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace TwitchBot.src.Connections
{
  public static class GitHub
  {
    private const long repoID = 407855144;
    private static ApiOptions options = new ApiOptions
    {
      PageCount = 1,
      PageSize = 1
    };
    private static GitHubClient client;

    public static void Init()
    {
      client = new GitHubClient(new ProductHeaderValue("twitch-bot"));
    }

    public static async Task<GitHubCommit> GetLastCommitAsync()
    {
      var allCommits = await client.Repository.Commit.GetAll(repoID, options);
      return allCommits[0];
    }

    public static async Task<Release> GetLastReleaseAsync()
    {
      return await client.Repository.Release.GetLatest(repoID);
    }
  }
}
