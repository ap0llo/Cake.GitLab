using Cake.Core;
using NGitLab;

namespace Cake.GitLab;

/// <summary>
/// A factory for creating instances of <see cref="IGitLabClient"/>.
/// </summary>
/// <remarks>
/// When implemented by the cake context class (see <see cref="ICakeContext"/>), <see cref="GitLabAliases"/> will use the context's <see cref="GetClient"/> method to create client instances.
/// <para>
/// This enables context implementation's to (optionally) control the creation of the GitLab client being used.
/// It can also be used for testing by returingin a mock instance.
/// </para>
/// </remarks>
public interface IGitLabClientFactory
{
    IGitLabClient GetClient(string serverUrl, string accessToken);
}
