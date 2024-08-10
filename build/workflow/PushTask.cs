using System;
using System.Linq;
using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Common.Tools.DotNet.NuGet.Source;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.FileHelpers;
using Cake.Frosting;
using Grynwald.SharedBuild;
using Grynwald.SharedBuild.Tasks;

namespace Cake.GitLab.Build;

[TaskName(TaskNames.Push)]
[IsDependentOn(typeof(PackTask))]
public class PushTask : FrostingTask<IBuildContext>
{
    public override bool ShouldRun(IBuildContext context)
    {
        return context.IsRunningInCI && context.PushTargets.Any(x => x.IsActive(context));
    }

    public override void Run(IBuildContext context)
    {
        var activePushTargets = context.PushTargets.Where(x => x.IsActive(context));
        foreach (var target in activePushTargets)
        {
            switch (target.Type)
            {
                case PushTargetType.AzureArtifacts:
                    PushToAzureArtifacts(context, target);
                    break;

                case PushTargetType.NuGetOrg:
                    PushToNuGetOrg(context, target);
                    break;

                case PushTargetType.MyGet:
                    PushToMyGet(context, target);
                    break;

                default:
                    throw new NotImplementedException($"Unimplemented push target type '{target.Type}'");
            }
        }
    }


    private void PushToAzureArtifacts(IBuildContext context, IPushTarget pushTarget)
    {
        // See https://www.daveaglick.com/posts/pushing-packages-from-azure-pipelines-to-azure-artifacts-using-cake
        var accessToken = context.EnvironmentVariable("SYSTEM_ACCESSTOKEN");
        if (String.IsNullOrEmpty(accessToken))
        {
            throw new InvalidOperationException("Could not resolve SYSTEM_ACCESSTOKEN.");
        }

        var tempDir = context.Environment.GetSpecialPath(SpecialPath.LocalTemp).Combine(Guid.NewGuid().ToString());
        context.EnsureDirectoryExists(tempDir);

        var nugetConfigPath = tempDir.CombineWithFilePath("NuGet.config");
        context.FileWriteText(
            nugetConfigPath,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <packageSources>
                <clear />
              </packageSources>
            </configuration>        
            """);

        context.DotNetNuGetAddSource(
            "AzureArtifacts",
            new DotNetNuGetSourceSettings()
            {
                Source = pushTarget.FeedUrl,
                UserName = "AzureArtifacts",
                Password = accessToken,
                ConfigFile = nugetConfigPath
            });

        context.Log.Information($"Pushing packages to Azure Artifacts feed '{pushTarget.FeedUrl}'");
        var pushSettings = new DotNetNuGetPushSettings()
        {
            Source = "AzureArtifacts",
            ApiKey = "AzureArtifacts",
            WorkingDirectory = tempDir
        };
        foreach (var package in context.Output.PackageFiles)
        {
            context.Log.Information($"Pushing package '{package}'");
            context.DotNetNuGetPush(package.FullPath, pushSettings);
        }
    }

    private void PushToNuGetOrg(IBuildContext context, IPushTarget pushTarget)
    {
        var apiKey = context.EnvironmentVariable("NUGET_ORG_APIKEY");
        if (String.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Could not determine nuget.org API key. Enviornment variable 'NUGET_ORG_APIKEY' is empty.");
        }

        context.Log.Information($"Pushing packages to nuget.org (feed {pushTarget.FeedUrl})");
        foreach (var package in context.Output.PackageFiles)
        {
            context.Log.Information($"Pushing package '{package}'");
            var pushSettings = new DotNetNuGetPushSettings()
            {
                Source = pushTarget.FeedUrl,
                ApiKey = apiKey
            };

            context.DotNetNuGetPush(package.FullPath, pushSettings);
        }
    }

    private void PushToMyGet(IBuildContext context, IPushTarget pushTarget)
    {
        var apiKey = context.EnvironmentVariable("MYGET_APIKEY");
        if (String.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Could not determine MyGet API key. Enviornment variable 'MYGET_APIKEY' is empty.");
        }

        context.Log.Information($"Pushing packages to MyGet (feed {pushTarget.FeedUrl})");
        foreach (var package in context.Output.PackageFiles)
        {
            context.Log.Information($"Pushing package '{package}'");
            var pushSettings = new DotNetNuGetPushSettings()
            {
                Source = pushTarget.FeedUrl,
                ApiKey = apiKey
            };

            context.DotNetNuGetPush(package.FullPath, pushSettings);
        }
    }
}
