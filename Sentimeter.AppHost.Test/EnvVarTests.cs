using Aspire.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://learn.microsoft.com/en-us/dotnet/aspire/testing/write-your-first-test?pivots=xunit

namespace Sentimeter.AppHost.Test;

public class EnvVarTests
{
    [Fact]
    public async Task WebResourceEnvVarsResolveToApiService()
    {
        // Arrange
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Sentimeter_AppHost>();

        var frontend = builder.CreateResourceBuilder<ProjectResource>("sentimeter-webapp");

        // Act
        var envVars = await frontend.Resource.GetEnvironmentVariableValuesAsync(
            DistributedApplicationOperation.Publish);

        // Assert
        Assert.Contains(envVars, static (kvp) =>
        {
            var (key, value) = kvp;

            return key is "services__sentimeter-webapi__https__0"
                && value is "{sentimeter-webapi.bindings.https.url}";
        });
    }
}
