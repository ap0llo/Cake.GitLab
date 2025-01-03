//
// Derived from
//  - https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog.Test/Integrations/GitLab/GitLabProjectInfoTest.cs
//  - https://github.com/ap0llo/changelog/blob/97d0d3111510dae9f4222704a50d88938be8debe/src/ChangeLog.Test/Integrations/GitLab/GitLabUrlParserTest.cs
//
// Licensed under the MIT License
//

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;

namespace Cake.GitLab.Test;

/// <summary>
/// Tests for <see cref="ServerIdentity"/>
/// </summary>
public class ServerIdentityTest : EqualityTest<ServerIdentity, ServerIdentityTest>, IEqualityTestDataProvider<ServerIdentity>
{
    //TODO: Consider protocol and port

    public IEnumerable<(ServerIdentity left, ServerIdentity right)> GetEqualTestCases()
    {
        yield return (
            new ServerIdentity("example.com"),
            new ServerIdentity("example.com")
        );

        yield return (
            new ServerIdentity("https", "example.com"),
            new ServerIdentity("https", "example.com")
        );

        yield return (
            new ServerIdentity("http", "example.com"),
            new ServerIdentity("http", "example.com")
        );

        yield return (
            new ServerIdentity("https", "example.com", 4443),
            new ServerIdentity("https", "example.com", 4443)
        );

        yield return (
            new ServerIdentity("example.com"),
            new ServerIdentity("https", "example.com")
        );

        yield return (
            new ServerIdentity("example.com"),
            new ServerIdentity("https", "example.com", 443)
        );

        // Comparisons must be case-insensitive
        yield return (
            new ServerIdentity("example.com"),
            new ServerIdentity("EXAMPLE.COM")
        );

        // Comparisons must be case-insensitive
        yield return (
            new ServerIdentity("HTTP", "example.com"),
            new ServerIdentity("http", "example.com")
        );
    }

    public IEnumerable<(ServerIdentity left, ServerIdentity right)> GetUnequalTestCases()
    {
        yield return (
            new ServerIdentity("example.com"),
            new ServerIdentity("example.net")
        );

        yield return (
            new ServerIdentity("https", "example.com"),
            new ServerIdentity("http", "example.com")
        );

        yield return (
            new ServerIdentity("https", "example.com", 443),
            new ServerIdentity("https", "example.com", 8080)
        );
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\t")]
    public void Host_must_not_be_null_or_whitespace(string? host)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new ServerIdentity(host!));

        // ASSERT
        Assert.IsType<ArgumentException>(ex);
    }


    [Fact]
    public void Url_returns_expected_value()
    {
        // ARRANGE
        var sut = new ServerIdentity("gitlab.example.com");

        // ACT
        var actual = sut.Url;

        // ASSERT
        Assert.Equal("https://gitlab.example.com/", actual);
    }

    [Fact]
    public void Host_returns_expected_value()
    {
        // ARRANGE
        var sut = new ServerIdentity("gitlab.example.com");

        // ACT
        var actual = sut.Host;

        // ASSERT
        Assert.Equal("gitlab.example.com", actual);
    }

    [Fact]
    public void Port_returns_expected_value()
    {
        // ARRANGE
        var sut = new ServerIdentity("gitlab.example.com");

        // ACT
        var actual = sut.Port;

        // ASSERT
        Assert.Equal(443, actual);
    }

    [Theory]
    [InlineData("http", 80)]
    [InlineData("https", 443)]
    [InlineData("some-protocol", -1)]
    public void Port_returns_expected_value_2(string protocol, int expected)
    {
        // ARRANGE
        var sut = new ServerIdentity(protocol, "gitlab.example.com");

        // ACT
        var actual = sut.Port;

        // ASSERT
        Assert.Equal(actual, expected);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(0)]
    public void Port_must_not_be_initialized_to_negative_value_or_zero(int port)
    {
        // ARRANGE

        // ACT
        var ex = Record.Exception(() => new ServerIdentity("https", "gitlab.example.com", port));

        // ASSERT
        Assert.IsType<ArgumentOutOfRangeException>(ex);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(0)]
    public void Port_must_not_be_set_to_a_negative_value_or_zero(int port)
    {
        // ARRANGE
        var sut = new ServerIdentity("https", "gitlab.example.com");

        // ACT
        var ex = Record.Exception(() => sut with { Port = port });

        // ASSERT
        Assert.IsType<ArgumentOutOfRangeException>(ex);
    }

    [Fact]
    public void Protocol_returns_expected_value()
    {
        // ARRANGE
        var sut = new ServerIdentity("gitlab.example.com");

        // ACT
        var actual = sut.Protocol;

        // ASSERT
        Assert.Equal("https", actual);
    }



    [Theory]
    // No change
    [InlineData("https", "example.com", 443, "https", "https://example.com/")]
    // Change protocol, port remains the same
    [InlineData("https", "example.com", 443, "http", "http://example.com:443/")]
    // Change protocol so port is the default port
    [InlineData("http", "example.com", 443, "https", "https://example.com/")]
    public void Updating_the_protocol_updates_the_url(string initialProtocol, string initialHost, int initialPort, string newProtocol, string expectedUrl)
    {
        // ARRANGE
        var sut = new ServerIdentity(initialProtocol, initialHost, initialPort);

        // ACT
        var updated = sut with { Protocol = newProtocol };

        // ASSERT

        // Url and Protocol must have been updated
        Assert.Equal(expectedUrl, updated.Url);
        Assert.Equal(newProtocol, updated.Protocol);

        // Host and Port must be unchanged
        Assert.Equal(initialHost, updated.Host);
        Assert.Equal(initialPort, updated.Port);
    }

    [Theory]
    [InlineData("https", "example.com", "https", 443)]
    [InlineData("https", "example.com", "http", 80)]
    public void Updating_the_protocol_updates_the_port_when_port_was_not_set_explicitly(string initialProtocol, string initialHost, string newProtocol, int expectedPort)
    {
        // ARRANGE
        var sut = new ServerIdentity(initialProtocol, initialHost);

        // ACT
        var updated = sut with { Protocol = newProtocol };

        // ASSERT
        Assert.Equal(expectedPort, updated.Port);
    }

    [Fact]
    public void Updating_the_host_updates_the_url()
    {
        // ARRANGE
        var sut = new ServerIdentity("gitlab.example.com");

        // ACT
        var updated = sut with { Host = "gitlab.com" };

        // ASSERT
        Assert.Equal("https://gitlab.com/", updated.Url);
    }

    [Fact]
    public void Updating_the_port_updates_the_url()
    {
        // ARRANGE
        var sut = new ServerIdentity("gitlab.example.com");

        // ACT
        var updated = sut with { Port = 8080 };

        // ASSERT
        Assert.Equal("https://gitlab.example.com:8080/", updated.Url);
    }

    [Theory]
    // No Change
    [InlineData("https", "example.com", 443, "https://example.com", "https", "example.com", 443)]
    [InlineData("https", "example.com", 443, "https://example.com:443", "https", "example.com", 443)]
    // Change Host name
    [InlineData("https", "example.com", 443, "https://gitlab.example.com", "https", "gitlab.example.com", 443)]
    // Change port
    [InlineData("https", "example.com", 443, "https://example.com:8080", "https", "example.com", 8080)]
    // Change protocol
    [InlineData("https", "example.com", 443, "http://example.com:443", "http", "example.com", 443)]
    // Change protocol, updating port to default scheme
    [InlineData("https", "example.com", 443, "http://example.com", "http", "example.com", 80)]
    public void Updating_the_url_updates_the_protocol_host_and_port(string initialProtocol, string initialHost, int initialPort, string newUrl, string expectedProtocol, string expectedHost, int expectedPort)
    {
        // ARRANGE
        var sut = new ServerIdentity(initialProtocol, initialHost, initialPort);

        // ACT
        var updated = sut with { Url = newUrl };

        // ASSERT
        Assert.Equal(expectedProtocol, updated.Protocol);
        Assert.Equal(expectedHost, updated.Host);
        Assert.Equal(expectedPort, updated.Port);
    }
}
