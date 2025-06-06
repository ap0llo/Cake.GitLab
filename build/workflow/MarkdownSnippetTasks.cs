﻿using System;
using System.Linq;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Security;
using Cake.Common.Tools.Command;
using Cake.Core;
using Cake.Frosting;
using Grynwald.SharedBuild.Tasks;

namespace Cake.GitLab.Build;

[TaskName("ApplyMarkdownSnippets")]
[TaskDescription("Generates all Markdown documentation files from the .source.md files utilizing the mdsnippets tool")]
[IsDependeeOf(typeof(GenerateTask))]
public class ApplyMarkdownSnippetsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Information("Generating Markdown files from .source.md files");
        context.Command(new CommandSettings()
        {
            ToolName = "MarkdownSnippets",
            ToolExecutableNames = ["mdsnippets", "mdsnippets.exe"],
            WorkingDirectory = context.RootDirectory
        });
    }
}


[TaskName("ValidateMarkdownSnippets")]
[TaskDescription("Validates that all generated Markdown files are up-to-date with respect to the .source.md files")]
[IsDependeeOf(typeof(ValidateTask))]
public class ValdiateMarkdownSnippetsTask : ApplyMarkdownSnippetsTask
{
    public override void Run(BuildContext context)
    {
        // Get all Markdown files and save a content hash before running the mdsnippets tool
        var markdownFilesBefore = context.GetFiles($"{context.RootDirectory}/**/*.md");
        var hashesBefore = markdownFilesBefore.ToDictionary(x => x, x => context.CalculateFileHash(x).ToHex());

        // run mdsnippets
        base.Run(context);

        // Get all Markdown files and save a content hash after running the mdsnippets tool
        var markdownFilesAfter = context.GetFiles($"{context.RootDirectory}/**/*.md");
        var hashesAfter = markdownFilesAfter.ToDictionary(x => x, x => context.CalculateFileHash(x).ToHex());


        // Compare before/after
        context.Information("Validating no Markdown files were changed");
        var foundError = false;
        foreach (var file in markdownFilesAfter)
        {

            if (hashesBefore.TryGetValue(file, out var hashBefore))
            {
                var hashAfter = hashesAfter[file];
                if (!StringComparer.OrdinalIgnoreCase.Equals(hashBefore, hashAfter))
                {
                    context.Error($"Markdown files are not up-to-date: File '{context.RootDirectory.GetRelativePath(file)}' was updated by mdsnippets");
                    foundError = true;
                }
            }
            else
            {
                context.Error($"Markdown files are not up-to-date: File '{context.RootDirectory.GetRelativePath(file)}' was generated by mdsnippets but did not exist before");
                foundError = true;
            }
        }

        if (foundError)
        {
            throw new CakeException("Validation of Markdown snippets failed. Check the log above for details");
        }
    }
}
