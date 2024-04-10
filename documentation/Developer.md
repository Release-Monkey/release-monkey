# Release Monkey for your project

Release Monkey is a tool that helps automate the release process of software. It works smoothly with your Continuous Integration/Continuous Deployment (CI/CD) setups, making it simple for non-technical testers to download and test your software builds. Whether your application is a command-line interface (CLI) tool, a web application, or a regular desktop application, you can automate its release using Release Monkey.

With Release Monkey, developers can create projects and invite testers to these projects, known as primary testers. Additionally, they can create a public page where other individuals can sign up to receive notifications about new releases for the project. Once all primary testers approve a release, Release Monkey will automatically trigger the release of your application into production.

There are three types of users recognized in Release Monkey:

1. **Releasers**: These are the individuals responsible for creating and publishing releases.
2. **Primary Testers**: These testers must approve a release before it can be pushed to production. A release only moves to production once all primary testers have given their approval.
3. **Beta Testers**: These testers provide feedback, but their input does not impact the scheduling of production releases. However, their opinions are valued by the developers.

## The rel-monkey (For Releases)

The rel-monkey cli has most of the functionality you will need out of release monkey. Usage is:

```
$ rel-monkey COMMAND ARGUMENTS
```

Where command can be any of the collowing commands:

1. `login` Will open browser so you can log into your Github account.
2. `logout` Will log you out of the cli.
3. `user` Prints the details of the user currently logged in.
4. `help` Prints a help message with links to learning resources.
5. `version` Prints the version of rel-monkey you have.
6. `repos` Lists all the repos the currently signed in user has access to on Github. You may want to run this before creating a project.
7. `create-project PROJECT_NAME GITHUB_REPO PERSONAL_ACCESS_TOKEN IS_PUBLIC` Creates a new project with the given name. The current project will automatically be set to the new project. See `set-project`. Paste GITHUB_REPO the same way it was printed by the `repo` command. You have also paste a PERSONAL_ACCESS_TOKEN (Github) that has the repo scope on Github. IS_PUBLIC can be true or false. If true, your project will get a public url for others to join as beta testers.
8. `set-project PROJECT_ID` Sets the current project to the project with the given id. We use a 'current project as context' approach so you do not have to specify the project id for every command.
9. `project` Prints the current project.
10. `list-projects` Lists all your projects (NOT IMPLEMENTED YET).
11. `add-testers TESTER1_EMAIL TESTER2_EMAIL ...` Adds the given emails as primary testers. The testers will be notified via email that they have been added.
12. `create-release RELEASE_NAME DOWNLOAD_LINK` Creates a new release for testing and notifies testers. Make sure `DOWNLOAD_LINK` points to the executable file or web app to be tested.
13. `list-releases` Lists all releases for the current project.
14. `release-key` Prints a base64 encoding of the current project and user. This encoding is meant to stored as a secret for your GITHUB workflows to enable automated releases from Github.
15. `load-release-key BASE64_ENCDOING` Sets the current user and project from the given base64 encoded release key. Use This to sign into rel-monkey from environments that do not have Web Browsers.

## The Web Client (For Testers)

Primary and Beta testers can use the web client to provide feedback on releases. The link to the web client is emailed to them for every new release made.
