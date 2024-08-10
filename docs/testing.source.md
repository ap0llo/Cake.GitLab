# Testing

`Cake.GitLab` is based on the [NGitLab library](https://github.com/ubisoft/NGitLab) which provides support for testing through the [NGitLab.Mock](https://www.nuget.org/packages/NGitLab.Mock) package.

This packages provides test doubles for many of `NGitLab`'s types and can be used to mock a GitLab server in your application or Cake Build.

`Cake.GitLab` provides the `IGitLabClientFactory` interface, which - when implemented by the Cake context - allows controlling the initializaiton of the `IGitLabClient` that is used by the GitLab aliases.

This can be used to inject a mocked GitLab client into the aliases in unit tests.

The following snippet shows an example of an (xunit-based) unit test that follows this approach.

snippet: Example-Testing

## See Also

- [GitLab Client Factory](./client-factory.md)