// Disable nullable reference methods for the alias method, since
// the Cake.Scripting runner is currently not compatible with aliases that contains nullabilty annotations
// See https://github.com/cake-build/cake/issues/4197
#nullable disable

using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.GitLab;

[CakeAliasCategory("GitLab")]
[CakeNamespaceImport("Cake.GitLab")]
public static class GitLabAliases
{    
    //[CakeMethodAlias]
    //public static void GitLab(this ICakeContext context, ...) => ...;
}
