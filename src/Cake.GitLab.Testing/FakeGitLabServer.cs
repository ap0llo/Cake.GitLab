using System;
using System.Collections.Generic;
using Cake.GitLab.Internal;
using NGitLab;
using NGitLab.Mock.Config;

namespace Cake.GitLab.Testing;

/// <summary>
/// Helper class to configure a Mock of a GitLab server for testing.
/// </summary>
public class FakeGitLabServer
{
    private readonly Dictionary<string, FakeGitLabGroup> m_Groups = new Dictionary<string, FakeGitLabGroup>(StringComparer.OrdinalIgnoreCase);
    private int m_NextProjectId = 1;

    /// <summary>
    /// Gets the URL of the fake GitLab server
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Gets the groups that exist in the server
    /// </summary>
    public IEnumerable<FakeGitLabGroup> Groups => m_Groups.Values;

    /// <summary>
    /// Initializes a new instance of <see cref="FakeGitLabServer"/>
    /// </summary>
    /// <param name="hostName">The host name of the GitLab server</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="hostName"/> is null or whitespace or not a valid host name (or IP Address)</exception>
    public FakeGitLabServer(string hostName)
    {
        Guard.NotNullOrWhitespace(hostName);

        var hostNameType = Uri.CheckHostName(hostName);
        if (hostNameType == UriHostNameType.Unknown)
        {
            throw new ArgumentException("Value is not a valid host name", nameof(hostName));

        }

        Url = $"https://{hostName}";
    }


    /// <summary>
    /// Adds a new group to the server
    /// </summary>
    /// <remarks>
    /// Note that this method checks <paramref name="name"/> for null or whitespace and will ensure the group name is unique within this instance of <see cref="FakeGitLabServer"/>.
    /// A real GitLab server will impose additional restrictions on group names.
    /// These rules are not enforced here.
    /// </remarks>
    /// <param name="name">The name of the group</param>
    /// <returns>Returns the newly created group</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public FakeGitLabGroup AddGroup(string name)
    {
        name = Guard.NotNullOrWhitespace(name);

        if (m_Groups.ContainsKey(name))
        {
            throw new ArgumentException($"A group named '{name}' already exists");
        }

        var group = new FakeGitLabGroup(this, name);
        m_Groups.Add(name, group);
        return group;
    }

    /// <summary>
    /// Creates a new <see cref="IGitLabClient"/> for the fake server using <c>NGitLab.Mock</c>
    /// </summary>
    internal IGitLabClient BuildClient()
    {
        var config = new GitLabConfig()
            .WithUser("DefaultUser", isDefault: true);

        foreach (var group in m_Groups.Values)
        {
            group.ApplyTo(config);
        }

        return config.BuildClient();
    }

    internal int NextProjectId() => m_NextProjectId++;
}
