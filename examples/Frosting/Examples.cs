﻿using Cake.GitLab;

return new CakeHost().Run(args);


[TaskName("OverloadsExamples")]
[TaskDescription("Example that shows the different available overloads to specify the GitLab Server, project and access token")]
public class OverloadsExamplesTask : FrostingTask
{
    //
    // Snippets defined here are used in docs/overloads.source.md.
    // See that document for additional explanations of the samples.
    //
    public override void Run(ICakeContext context)
    {
        // begin-snippet: Overloads-Individual-Parameters
        // The project may be specified as either a string with the full project path 
        context.GitLabRepositoryGetBranches("https://example.com", "ACCESSTOKEN", "example-group/example-project");

        // or using the project's numeric id
        context.GitLabRepositoryGetBranches("https://example.com", "ACCESSTOKEN", 12345);
        // end-snippet


        // begin-snippet: Overloads-Identity-Objects
        // GitLabServerIdentity can be used instead of the "serverUrl" string
        var serverIdentity = new GitLabServerIdentity("example.com");
        context.GitLabRepositoryGetBranches(serverIdentity, "ACCESSTOKEN", "example-group/example-project");

        // GitLabProjectIdentity replaces both the serverUrl and project parameters
        var projectIdentity = new GitLabProjectIdentity("example.com", "example-group", "example-project");
        context.GitLabRepositoryGetBranches(projectIdentity, "ACCESSTOKEN");
        // end-snippet


        // begin-snippet: Overloads-Connection-Objects
        // GitLabServerConnection replaces the serverUrl and accessToken parameters
        var serverConnection = new GitLabServerConnection("example.com", "ACCESSTOKEN");
        context.GitLabRepositoryGetBranches(serverConnection, "example-group/example-project");

        // GitLabProjectConnection replaces the serverUrl, accessToken and project parameters
        var projectConnection = new GitLabProjectConnection("example.com", "example-group", "example-project", "ACCESSTOKEN");
        context.GitLabRepositoryGetBranches(projectConnection);
        // end-snippet

        // begin-snippet: Overloads-MixAndMatch
        context.GitLabRepositoryGetBranches(projectIdentity, "ACCESSTOKEN");
        context.GitLabRepositoryGetBranches(projectIdentity, "ACCESSTOKEN", "another-group/another-project");

        context.GitLabRepositoryGetBranches(projectConnection);
        context.GitLabRepositoryGetBranches(projectConnection, "another-group/another-project");
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
        //begin-snippet: GitLabServerIdentity
        var serverIdentity = new GitLabServerIdentity("example.com");
        //end-snippet
    }

    private void ProjectIdentityExamples()
    {
        {
            // begin-snippet: GitLabProjectIdentity-Simple
            GitLabProjectIdentity projectIdentity;

            // A GitLabProjectIdentity can be constructed from the server host name, namespace and project name
            projectIdentity = new GitLabProjectIdentity("example.com", "example-group", "example-project");

            // Alternatively, the project path (which is both the namesapce and project name) can be passed in as a single parameter
            projectIdentity = new GitLabProjectIdentity("example.com", "example-group/example-project");

            // end-snippet
        }

        {
            // begin-snippet: GitLabProjectIdentity-FromRemoteUrl
            // The project identity can also be constructed from a git remote url (either SSH or HTTP urls)
            GitLabProjectIdentity projectIdentity;
            projectIdentity = GitLabProjectIdentity.FromGitRemoteUrl("git@example.com:example-group/example-project.git");
            projectIdentity = GitLabProjectIdentity.FromGitRemoteUrl("https://example.com/example-group/example-project.git");
            // end-snippet
        }

        {
            // begin-snippet: GitLabProjectIdentity-CopyAndModifiy
            var projectIdentity = new GitLabProjectIdentity("example.com", "example-group", "example-project");

            // Changing the "Project" property creates the identity of a project in the same group/subgroup on the same GitLab server
            var siblingProjectIdentity = projectIdentity with { Project = "sibling-project" };


            GitLabProjectIdentity otherProjectIdentity;
            // Changing the "ProjectPath" will also update the "Project" and "Namespace" properties
            otherProjectIdentity = projectIdentity with { ProjectPath = "another-group/subgroup/another-project" };
            // This is equivalent to
            otherProjectIdentity = projectIdentity with { Namespace = "another-group/subgroup", Project = "another-project" };

            //end-snippet
        }
    }

    private void ServerConnectionExamples()
    {
        {
            // begin-snippet: GitLabServerConnection-Simple            
            var serverConnection = new GitLabServerConnection("example.com", "ACCESSTOKEN");
            // end-snippet
        }

        {

            // begin-snippet: GitLabServerConnection-FromIdentity     
            var serverIdentity = new GitLabServerIdentity("example.com");

            GitLabServerConnection serverConnection;
            serverConnection = new GitLabServerConnection(serverIdentity, "ACCESSTOKEN");

            // since GitLabProjectIdentity derives from GitLabServerIdentity, a server connection can also be initialized from a git remote url:
            var projectIdentity = GitLabProjectIdentity.FromGitRemoteUrl("git@example.com:example-group/example-project.git");
            serverConnection = new GitLabServerConnection(projectIdentity, "ACCESSTOKEN");

            // end-snippet
        }
    }
    private void ProjectConnectionExamples()
    {
        {
            // begin-snippet: GitLabProjectConnection-Simple
            GitLabProjectConnection projectConnection;

            // A GitLabProjectConnection can be constructed from the server host name, namespace, project name and access token
            projectConnection = new GitLabProjectConnection("example.com", "example-group", "example-project", "ACCESSTOKEN");

            // Alternatively, the project path (which is both the namesapce and project name) can be passed in as a single parameter
            projectConnection = new GitLabProjectConnection("example.com", "example-group/example-project", "ACCESSSTOKEN");

            // end-snippet
        }

        {

            // begin-snippet: GitLabProjectConnection-FromIdentity            
            var projectIdentity = GitLabProjectIdentity.FromGitRemoteUrl("git@example.com:example-group/example-project.git");

            var projectConnection = new GitLabProjectConnection(projectIdentity, "ACCESSTOKEN");
            // end-snippet
        }
    }
}
