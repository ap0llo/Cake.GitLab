using Cake.Core;

namespace Cake.GitLab.Testing;

/// <summary>
/// A <see cref="ICakeContext"/> that provides a implementation of <see cref="IGitLabProvider"/> for use with the <c>Cake.GitLab</c> aliases (<see cref="GitLabAliases"/>)
/// </summary>
/// <remarks>
/// When one of the aliases from <see cref="GitLabAliases"/> is used with a Cake context that implements this interface,
/// the provider returned by <see cref="GitLab"/> will be used instead of the default implementation of the aliases.
/// (<see cref="IGitLabProvider"/> defines methods that correspond 1:1 to alias definitions in <see cref="GitLabAliases"/>).
/// <para>
/// USe this to provide mock/test implementations of <see cref="IGitLabProvider"/> for (unit-)testing.
/// </para>
/// </remarks>
public interface IGitLabCakeContext : ICakeContext
{
    /// <summary>
    /// Gets the <see cref="IGitLabProvider"/> to use.
    /// </summary>
    IGitLabProvider GitLab { get; }
}
