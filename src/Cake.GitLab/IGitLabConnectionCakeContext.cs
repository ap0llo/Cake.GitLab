﻿using Cake.Core;

namespace Cake.GitLab;

/// <summary>
/// Extended <see cref="ICakeContext"/> that provides a <see cref="GitLabConnection"/>.
/// </summary>
/// <remarks>
/// Implement this interface in your context class to avoid having to pass a <see cref="GitLabConnection"/> to each GitLab alias call.
/// </remarks>
public interface IGitLabConnectionCakeContext : ICakeContext
{
    /// <summary>
    /// Gets the connection parameters for accessing GitLab
    /// </summary>
    public GitLabConnection Connection { get; }
}