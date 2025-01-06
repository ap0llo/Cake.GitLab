using System.Runtime.Serialization;

namespace Cake.GitLab;

/// <summary>
/// Defines the supported scopes for querying pipeline jobs (see <see cref="GetPipelineJobsOptions"/>)
/// </summary>
public enum JobScope
{
    [EnumMember(Value = "created")]
    Created,

    [EnumMember(Value = "pending")]
    Pending,

    [EnumMember(Value = "running")]
    Running,

    [EnumMember(Value = "failed")]
    Failed,

    [EnumMember(Value = "success")]
    Success,

    [EnumMember(Value = "canceled")]
    Canceled,

    [EnumMember(Value = "skipped")]
    Skipped,

    [EnumMember(Value = "waiting_for_resource")]
    WaitingForResource,

    [EnumMember(Value = "manual")]
    Manual
}

