using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.DotNetLocalTools.Module;
using Cake.Frosting;
using Grynwald.SharedBuild;
using Grynwald.SharedBuild.Tasks;

return new CakeHost()
    .UseModule<LocalToolsModule>()
    .InstallToolsFromManifest(".config/dotnet-tools.json")
    .UseSharedBuild<BuildContext>(taskFilter:
        // Running dotnet format on Azure Pipelines when using the .NET 9 SDK and Nerdbank.GitVersioning causes the build to hang
        // See: https://github.com/dotnet/sdk/issues/44951
        // To work around this, exclude the task when running on Azure Pipelines
        task =>
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(Environment.GetEnvironmentVariable("TF_BUILD"), "true") &&
                task == typeof(ValidateCodeFormattingTask))
            {
                return false;
            }

            return true;
        }
    )
    .Run(args);

public class BuildContext(ICakeContext context) : DefaultBuildContext(context)
{
    public override IReadOnlyCollection<IPushTarget> PushTargets { get; } =
    [
        new PushTarget(
            type: PushTargetType.AzureArtifacts,
            feedUrl: "https://pkgs.dev.azure.com/ap0llo/OSS/_packaging/PublicCI/nuget/v3/index.json",
            isActive: context => context.Git.IsMainBranch || context.Git.IsReleaseBranch
        ),
        new PushTarget(
            type: PushTargetType.NuGetOrg,
            feedUrl: "https://api.nuget.org/v3/index.json",
            isActive: context => context.Git.IsReleaseBranch
        )
    ];
}
