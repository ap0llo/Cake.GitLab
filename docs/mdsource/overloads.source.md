# Overloads

For all Cake aliases provided by Cake.GitLab, multiple overloads are available.
These overloads differ in the way that the GitLab server, project and access token are specified.

## Individual Parameters

The most basic overloads allow specifying all values as individual parameters:

snippet: Overloads-Individual-Parameters

However, passing all required values individually can become repetitive, especially when calling multiple GitLab APIs.
To improve on that, common parameters may be combined into a single object which can be passed instead:

- ["Identity" objects](#identity-objects)
- ["Connection" objects](#connection-objects)

## "Identity" objects

An "identity" object encapsulates all data required to identify a server or a project.
There are two identity objects supported:

- `ServerIdentity` identifies a GitLab Server
- `ProjectIdentity` identifies a specific project on a GitLab server

snippet: Overloads-Identity-Objects

## "Connection" objects

A "connection" object combines the data to identify a project or server with an access token required to access the GitLab API.

There are two connection objects supported:
  
- `ServerConnection` identifies a GitLab server and provides an access token
- `ProjectConnection` identifies a GitLab project and provides an access token

snippet: Overloads-Connection-Objects

## Mix and Match

Since the project identity/connection objects derive from their "server" counterparts, a ProjectConnection or ProjectIdentity object can also be used to get data from a different project.

For example, use a ProjectConnection object to hold data about the default project, but also get data from a different project on the same server using the same access token.

snippet: Overloads-MixAndMatch