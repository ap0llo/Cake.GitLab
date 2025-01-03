return new CakeHost().Run(args);

[TaskName("OverloadsExamples")]
[TaskDescription("Example that shows the different available overloads to specify the GitLab Server, project and access token")]
public class OverloadsExamplesTask : AsyncFrostingTask
{
    //
    // Snippets defined here are used in docs/overloads.source.md.
    // See that document for additional explanations of the samples.
    //
    public override async Task RunAsync(ICakeContext context)
    {
        // begin-snippet: Overloads-Individual-Parameters
        // The project may be specified as either a string with the full project path
        await context.GitLabRepositoryGetBranchesAsync("https://example.com", "ACCESSTOKEN", "example-group/example-project");

        // or using the project's numeric id
        await context.GitLabRepositoryGetBranchesAsync("https://example.com", "ACCESSTOKEN", 12345);
        // end-snippet


        // begin-snippet: Overloads-Identity-Objects
        // ServerIdentity can be used instead of the "serverUrl" string
        var serverIdentity = new ServerIdentity("example.com");
        await context.GitLabRepositoryGetBranchesAsync(serverIdentity, "ACCESSTOKEN", "example-group/example-project");

        // ProjectIdentity replaces both the serverUrl and project parameters
        var projectIdentity = new ProjectIdentity(serverIdentity, "example-group", "example-project");
        await context.GitLabRepositoryGetBranchesAsync(projectIdentity, "ACCESSTOKEN");
        // end-snippet


        // begin-snippet: Overloads-Connection-Objects
        // ServerConnection replaces the serverUrl and accessToken parameters
        var serverConnection = new ServerConnection("example.com", "ACCESSTOKEN");
        await context.GitLabRepositoryGetBranchesAsync(serverConnection, "example-group/example-project");

        // ProjectConnection replaces the serverUrl, accessToken and project parameters
        var projectConnection = new ProjectConnection(serverConnection, "example-group", "example-project");
        await context.GitLabRepositoryGetBranchesAsync(projectConnection);
        // end-snippet
    }
}

[TaskName("IdentityAndConnectionExamples")]
[TaskDescription("Example that shows ways to work with identity and connection object")]
public class IdentityAndConnectionExamplesTask : FrostingTask
{
    public override void Run(ICakeContext context)
    {
        ServerIdentityExamples();
        ProjectIdentityExamples();
        ServerConnectionExamples();
        ProjectConnectionExamples();
    }

    private void ServerIdentityExamples()
    {
        //begin-snippet: ServerIdentity
        ServerIdentity serverIdentity;

        // Create ServerIdentity from host name. This assumes the server uses HTTPS on the default port
        serverIdentity = new ServerIdentity(host: "example.com");

        // To use http or a different port, pass in a protocol and/or a port in addition to the host name
        serverIdentity = new ServerIdentity(protocol: "http", host: "example.com");
        serverIdentity = new ServerIdentity(protocol: "https", host: "example.com", port: 8086);

        // Alternatively, a server identity can be initialized from the server url
        serverIdentity = ServerIdentity.FromUrl("https://gitlab.example.com:8080");

        //end-snippet
    }

    private void ProjectIdentityExamples()
    {
        {
            // begin-snippet: ProjectIdentity-Simple
            ProjectIdentity projectIdentity;

            // A ProjectIdentity can be constructed from a server identity, namespace and project name
            projectIdentity = new ProjectIdentity(new ServerIdentity("example.com"), "example-group", "example-project");

            // Alternatively, the project path (which is both the namespace and project name) can be passed in as a single parameter
            projectIdentity = new ProjectIdentity(new ServerIdentity("example.com"), "example-group/example-project");

            // end-snippet
        }

        {
            // begin-snippet: ProjectIdentity-FromRemoteUrl
            // The project identity can be initialized from a git remote url (either SSH or HTTP urls)
            ProjectIdentity projectIdentity;
            projectIdentity = ProjectIdentity.FromGitRemoteUrl("git@example.com:example-group/example-project.git");
            projectIdentity = ProjectIdentity.FromGitRemoteUrl("https://example.com/example-group/example-project.git");
            // end-snippet
        }

        {
            // begin-snippet: ProjectIdentity-CopyAndModify
            var projectIdentity = new ProjectIdentity(new ServerIdentity("example.com"), "example-group", "example-project");

            // Changing the "Project" property creates the identity of a project in the same group/subgroup on the same GitLab server
            var siblingProjectIdentity = projectIdentity with { Project = "sibling-project" };

            ProjectIdentity otherProjectIdentity;
            // Changing the "ProjectPath" property will also update the "Project" and "Namespace" properties
            otherProjectIdentity = projectIdentity with { ProjectPath = "another-group/subgroup/another-project" };
            // This is equivalent to
            otherProjectIdentity = projectIdentity with { Namespace = "another-group/subgroup", Project = "another-project" };

            //end-snippet
        }
    }

    private void ServerConnectionExamples()
    {
        {
            // begin-snippet: ServerConnection-Simple
            var serverConnection = new ServerConnection("example.com", "ACCESSTOKEN");
            // end-snippet
        }

        {
            // begin-snippet: ServerConnection-FromIdentity
            var serverIdentity = new ServerIdentity("example.com");

            ServerConnection serverConnection;
            serverConnection = new ServerConnection(serverIdentity, "ACCESSTOKEN");

            // since ProjectIdentity also provides a server identity, a server connection can also be initialized from a git remote url:
            var projectIdentity = ProjectIdentity.FromGitRemoteUrl("git@example.com:example-group/example-project.git");
            serverConnection = new ServerConnection(projectIdentity.Server, "ACCESSTOKEN");

            // end-snippet
        }
    }

    private void ProjectConnectionExamples()
    {
        {
            // begin-snippet: ProjectConnection-Simple
            ProjectConnection projectConnection;

            // A ProjectConnection can be constructed from a server identity, namespace, project name and access token
            projectConnection = new ProjectConnection(new ServerIdentity("example.com"), "example-group", "example-project", "ACCESSTOKEN");

            // Alternatively, the project path (which is both the namespace and project name) can be passed in as a single parameter
            projectConnection = new ProjectConnection(new ServerIdentity("example.com"), "example-group/example-project", "ACCESSSTOKEN");

            // end-snippet
        }
        {

            // begin-snippet: ProjectConnection-FromIdentity
            var projectIdentity = ProjectIdentity.FromGitRemoteUrl("git@example.com:example-group/example-project.git");

            var projectConnection = new ProjectConnection(projectIdentity, "ACCESSTOKEN");
            // end-snippet
        }
    }
}
