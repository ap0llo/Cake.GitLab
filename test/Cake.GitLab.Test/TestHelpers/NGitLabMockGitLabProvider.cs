using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using NGitLab;
using NGitLab.Mock;

namespace Cake.GitLab.Test;

/// <summary>
/// Implementation of <see cref="IGitLabProvider"/> that uses <c>NGitLab.Mock</c> for mocking access to GitLab
/// </summary>
public class NGitLabMockGitLabProvider(ICakeContext context) : DefaultGitLabProvider(context)
{
    private readonly Dictionary<string, GitLabServer> m_GitLabServers = new(StringComparer.OrdinalIgnoreCase);

    protected override IGitLabClient GetClient(string serverUrl, string accessToken)
    {
        if (m_GitLabServers.TryGetValue(serverUrl, out var server))
        {
            return server.CreateClient(server.Users.First());
        }
        else
        {
            throw new ArgumentException($"No server for connection {serverUrl} configured");
        }
    }

    public void AddServer(GitLabServer server) => m_GitLabServers.Add(server.Url.ToString(), server);
}
