using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Cake.GitLab.Internal;
using Microsoft.VisualBasic;
using NGitLab.Models;

namespace Cake.GitLab;

/// <summary>
/// Encapsulates the identity of GitLab server
/// </summary>
public record ServerIdentity
{
    private readonly UriBuilder m_UriBuilder;

    /// <summary>
    /// Gets or sets the protocol of the GitLab server
    /// </summary>
    /// <remarks>
    /// Updating the protocol implicitly updates the <see cref="Url"/> property.
    /// <para>
    /// If no port was set explicitly, updating the protocol will also update the <see cref="Port"/> property to the default port for the specified protocol.
    /// </para>
    /// </remarks>
    public string Protocol
    {
        get => m_UriBuilder.Scheme;
        init => m_UriBuilder.Scheme = Guard.NotNullOrWhitespace(value);
    }

    /// <summary>
    /// Gets or sets the host name of the GitLab server.
    /// </summary>
    /// <remarks>
    /// Updating the host name implicitly updates the <see cref="Url"/> property.
    /// </remarks>
    public string Host
    {
        get => m_UriBuilder.Host;
        init => m_UriBuilder.Host = Guard.NotNullOrWhitespace(value);
    }

    /// <summary>
    /// Gets or sets the port of the GitLab server
    /// </summary>
    /// <remarks>
    /// Updating the port implicitly updates the <see cref="Url"/> property
    /// </remarks>
    public int Port
    {
        // If the UriBuilder's port is not valid, it has not been set explicitly.
        // In that case, return the default port for the current protocol
        get => m_UriBuilder.Port > 0 ? m_UriBuilder.Port : m_UriBuilder.Uri.Port;
        init => m_UriBuilder.Port = Guard.Positive(value);
    }

    /// <summary>
    /// Gets or set the GitLab server's URL
    /// </summary>
    /// <remarks>
    /// Setting the Url will implicitly update the properties <see cref="Protocol"/>, <see cref="Host"/> and <see cref="Port"/>
    /// </remarks>
    public string Url
    {
        get
        {
            return m_UriBuilder.Uri.ToString();
        }
        init
        {
            m_UriBuilder = new UriBuilder(value);
        }
    }


    /// <summary>
    /// Initializes a new instance of <see cref="ServerIdentity"/> with the specified host name
    /// </summary>
    /// <remarks>
    /// When only a host name is specified, the protocol defaults to <c>https</c>
    /// </remarks>
    /// <param name="host">The host name of the GitLab server.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="host"/> is null or whitespace</exception>
    public ServerIdentity(string host) : this("https", host)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="ServerIdentity"/> with the specified host name and protocol
    /// </summary>
    /// <param name="protocol">The protocol to use for communicating with the GitLab server.</param>
    /// <param name="host">The host name of the GitLab server.</param>
    /// <remarks>
    /// The post is set implicitly to the default port for the specified protocol.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="protocol"/> is null or whitespace</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="host"/> is null or whitespace</exception>
    public ServerIdentity(string protocol, string host)
    {
        m_UriBuilder = new UriBuilder()
        {
            Host = Guard.NotNullOrWhitespace(host),
            Scheme = Guard.NotNullOrWhitespace(protocol)
        };
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ServerIdentity"/> with the specified protocol, host name and port
    /// </summary>
    /// <param name="protocol">The protocol to use for communicating with the GitLab server.</param>
    /// <param name="host">The host name of the GitLab server.</param>
    /// <param name="port">The port to use for communicating with the GitLab server</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="protocol"/> is null or whitespace</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="host"/> is null or whitespace</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="port"/> is negative or zero.</exception>
    public ServerIdentity(string protocol, string host, int port)
    {
        m_UriBuilder = new UriBuilder()
        {
            Host = Guard.NotNullOrWhitespace(host),
            Scheme = Guard.NotNullOrWhitespace(protocol),
            Port = Guard.Positive(port)
        };
    }

    private ServerIdentity(UriBuilder uriBuilder)
    {
        m_UriBuilder = Guard.NotNull(uriBuilder);
    }



    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(Protocol),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Host),
            Port);

    /// <inheritdoc />
    public virtual bool Equals(ServerIdentity? other)
    {
        return other is not null &&
               StringComparer.OrdinalIgnoreCase.Equals(Protocol, other.Protocol) &&
               StringComparer.OrdinalIgnoreCase.Equals(Host, other.Host) &&
               Port == other.Port;
    }

    /// <summary>
    /// Initializes a new <see cref="ServerIdentity"/> from a server url
    /// </summary>
    /// <param name="url">The url of the GitLab server.</param>
    internal static ServerIdentity FromUrl(string url)
    {
        var uriBuilder = new UriBuilder(url);
        return new ServerIdentity(uriBuilder);
    }
}
