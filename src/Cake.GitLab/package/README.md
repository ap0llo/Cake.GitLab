# Cake.GitLab

Cake.GitLab is a [Cake](https://cakebuild.net/) Addin that provides aliases for interaction with the [GitLab API](https://docs.gitlab.com/ee/api/rest/).

## Usage

### Install the Addin

Install the addin into your Cake build.

- If you are using Cake scripting, use the `#addin` reprocessor directive:

  ```cs
  #addin nuget:?package=Cake.GitLab&version=VERSION
  ```

- If you are using [Cake.Frosting](https://cakebuild.net/docs/running-builds/runners/cake-frosting), install the addin by adding a package reference to your project:

  ```xml
  <PackageReference Include="Cake.GitLab" Version="VERSION" /> 
  ```

### Available Aliases

For a list of aliases provided by the addin, please refer to the [documentation](https://github.com/ap0llo/Cake.GitLab/blob/main/docs/README.md).

## License

Cake.GitLab is licensed under the MIT License.

For details see https://github.com/ap0llo/Cake.GitLab/blob/main/LICENSE