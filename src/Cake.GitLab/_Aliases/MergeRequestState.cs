using System;

namespace Cake.GitLab;

/// <summary>
/// Defines the possible states of a Merge Request
/// </summary>
public enum MergeRequestState
{
    Opened,

    Closed,

    Locked,

    Merged
}


public static class MergeRequestStateExtensions
{
    public static NGitLab.Models.MergeRequestState ToNGitLabMergeRequestState(this MergeRequestState state)
    {
        return state switch
        {
            MergeRequestState.Opened => NGitLab.Models.MergeRequestState.opened,
            MergeRequestState.Closed => NGitLab.Models.MergeRequestState.closed,
            MergeRequestState.Locked => NGitLab.Models.MergeRequestState.locked,
            MergeRequestState.Merged => NGitLab.Models.MergeRequestState.merged,
            _ => throw new InvalidOperationException($"Unimplemented case in switch statement: {nameof(MergeRequestState)}.{state}")
        };
    }
}
