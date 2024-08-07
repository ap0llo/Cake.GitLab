# Overloads

For all Cake aliases provided by Cake.GitLab, multiple overloads are available.
These overloads differ in the way that the GitLab server, project and access token are specified.

## Individual Parameters

The most basic overloads allows specifying all values as individual parameters:

snippet: Overloads-Individual-Parameters

However, passing all required values individually can become repetitive, especially when calling multiple GitLab API functions.
To improve on that, common parameters may be combined into a single object which can be passed instead:

- ["Identity" object](#identity-objects)
- ["Connection" object](#connection-objects)

## "Identity" objects

An "identity" object encapsulates all data required to identify a server or a project.
There are two identity objects supported:

- `GitLabServerIdentity` identifies a GitLab Server
- `GitLabProjectIdentity` identifies a specific project on a GitLab server

snippet: Overloads-Identity-Objects

## "Connection" objects

A "connection" object combines the data to identify a project or server with an access token required to authenticate.

There are two connection objects supported:
  
- `GitLabServerConnection` specifies a GitLab server and an access token
- `GitLabProjectConnection` identifies a GitLab project and provides an access token

snippet: Overloads-Connection-Objects

## Mix and Match

Since the project identity/connection objects derive from their "server" counterparts, a GitLabProjectConnection or GitLabProjectIdentity object can also be used to get data from a different project.

For example, use a GitLabProjectConnection object to hold data about the default project, but also get data from a different project on the same server using the same access token.

snippet: Overloads-MixAndMatch