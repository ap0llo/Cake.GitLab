# Identity and Connection objects

toc

## Overview

The connections parameters for a GitLab Server or project may be specified as "Identity" or "Connection" objects.

- An "identity" object encapsulates all data required to identify a server or a project.
- A "connection" object combines the data to identify a project or server with an access token required to authenticate.

## "Identity" objects

### GitLabServerIdentity

A `GitLabServerIdentity` object encapsulates all data to identify a Gitlab server.

It can be created from the GitLab server's host name.

snippet: GitLabServerIdentity

### GitLabProjectIdentity

A `GitLabProjectIdentity` object encapsulates all data to identify a project on a Gitlab server.
This includes

- The server's host name
- The project namespace (i.e. the name of the user or group (including subgroups) that owns the project)
- The project name

A project identity can be constructed from these individual values:

snippet: GitLabProjectIdentity-Simple

Further, the project identity can be extracted from the git remote url of a (local) git repository.
This can be useful to e.g. avoid hard-coding GitLab project information in a Cake build script and instead retrieve the data from the local git repository.

snippet: GitLabProjectIdentity-FromRemoteUrl

Project identity objects are immutable record types, modified copies can be created using C#'s `with` expression. 

snippet: GitLabProjectIdentity-CopyAndModify

## "Connection" objects

### GitLabServerConnection

`GitLabServerConnection` extends `GitLabServerIdentity` with an access token and thus encapsulates all data required for identifying a server and authenticating to the GitLab API.

Initialization of `GitLabServerConnection` is analogous to the initialization of `GitLabServerIdentity` with the addition of a `accessToken` parameter

snippet: GitLabServerConnection-Simple

A `GitLabServerConnection` can also be created from an existing `GitLabServerdentity`.

snippet: GitLabServerConnection-FromIdentity

### GitLabProjectConnection

`GitLabProjectConnection` extends `GitLabProjectIdentity` with an access token and thus encapsulates all data required for identifying a project and authenticating to the GitLab API.

Initialization of `GitLabProjectConnection` is analogous to the initialization of `GitLabProjectIdentity` with the addition of a `accessToken` parameter

snippet: GitLabProjectConnection-Simple

A `GitLabProjectConnection` can also be created from an existing `GitLabProjectIdentity`, allowing e.g. the usage for `FromGitRemoteUrl()` as shown above:

snippet: GitLabProjectConnection-FromIdentity