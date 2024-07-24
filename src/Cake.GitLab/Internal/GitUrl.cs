// Derived from https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog/Integrations/GitUrl.cs
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;

namespace Cake.GitLab.Internal;

internal static class GitUrl
{
    public static bool TryGetUri(string remoteUrl, [NotNullWhen(true)] out Uri? uri)
    {
        if (Uri.TryCreate(remoteUrl, UriKind.Absolute, out uri))
        {
            return true;
        }
        else
        {
            return TryParseScpUrl(remoteUrl, out uri);
        }
    }

    private static bool TryParseScpUrl(string url, [NotNullWhen(true)] out Uri? sshUri)
    {
        // Parse a scp-format git url: e.g. git@github.com:ap0llo/Cake.GitLab.git

        var fragments = url.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (fragments.Length != 2)
        {
            sshUri = default;
            return false;
        }

        var userNameAndHost = fragments[0];
        var path = fragments[1].TrimStart('/');

        return Uri.TryCreate($"ssh://{userNameAndHost}/{path}", UriKind.Absolute, out sshUri);
    }
}
