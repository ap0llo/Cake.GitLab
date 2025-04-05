using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using PublicApiGenerator;
using VerifyXunit;
using Xunit;

namespace Cake.GitLab.Testing.Test;

[Trait("Category", "SkipWhenLiveUnitTesting")]
public class PublicApiTest
{
    [Fact]
    public Task Has_expected_public_api()
    {
        // ARRANGE

        var assembly = typeof(FakeGitLabProvider).Assembly;


        var options = new ApiGeneratorOptions()
        {
            ExcludeAttributes =
            [
                typeof(TargetFrameworkAttribute).FullName!,
                typeof(AssemblyMetadataAttribute).FullName!,
                typeof(InternalsVisibleToAttribute).FullName!
            ]
        };

        // ACT
        var publicApi = ApiGenerator.GeneratePublicApi(assembly, options);

        // ASSERT        
        return Verifier.Verify(publicApi, extension: "txt").UseFileName("PublicApi");
    }
}
