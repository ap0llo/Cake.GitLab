# GitLab Client Factory

`Cake.GitLab` is based on the [NGitLab library](https://github.com/ubisoft/NGitLab) and performs all interactions with GitLab through NGitLab's `IGitLabClient` interface.

`Cake.GitLab` will create an instance of the default implementation (`GitLabClient`) for every operation.
This should work in almost all cases.

In case you want to intercept the creation of the client (e.g. to use a custom implementation of `IGitLabClient` or for [testing](./testing.md)), add an implementation of the `IGitLabClientFactory` interface to your build context class (this will only work for `Cake.Frosting` projects.)

When the context implements the interface, all aliases will use the context's `GetClient()` method to get the GitLab client.

The following snippets shows a build context implementing `IGitLabClientFactory`:

snippet: Example-GitLabClientFactory

## See Also

- [Testing](./testing.md)