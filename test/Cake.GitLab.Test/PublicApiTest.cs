using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using PublicApiGenerator;
using VerifyXunit;
using Xunit;

namespace Cake.GitLab.Test;

[Trait("Category", "SkipWhenLiveUnitTesting")]
public class PublicApiTest
{
    [Fact]
    public Task Has_expected_public_api()
    {
        // ARRANGE

        var assembly = typeof(GitLabAliases).Assembly;


        var options = new ApiGeneratorOptions()
        {
            ExcludeAttributes = new[]
            {
                typeof(TargetFrameworkAttribute).FullName!,
                typeof(AssemblyMetadataAttribute).FullName!,
                typeof(InternalsVisibleToAttribute).FullName!
            }
        };

        // ACT
        var publicApi = ApiGenerator.GeneratePublicApi(assembly, options);

        // ASSERT        
        return Verifier.Verify(publicApi, extension: "txt").UseFileName("PublicApi");
    }
}
