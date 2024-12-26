# Testing

To facilitate testing of Cake build scripts and tasks, `Cake.GitLab` supports mocking of the aliases it provides.

This is realized through the `IGitLabProvider` interface.
It defines methods for all `Cake.GitLab` aliases, decoupling the implementation of the Cake aliases from the aliases themselves.

- By default, the aliases will use the implementation in `DefaultGitLabProvider`
- However, a different implementation of the interface can be provided by using a `ICakeContext` that implements `IGitLabCakeContext`.
  - When an alias is used with a context that implements that interface, the `IGitLabProvider` returned by the context's `GitLab` property will be used instead of the default implementation
  - This allows injecting alternative/test implementation of that interface

The following snippet shows an example of a (xunit-based) unit test that follows this approach.
A mock of `IGitLabProvider` is created dynamically using [moq](https://github.com/devlooped/moq)

snippet: Example-Testing
