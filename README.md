# Todo Core

This solution contains Three projects:
  - TodoCore: which is an Identity Server 4 project connected with Asp.net Identity and SQL server to act as an Authentication server.
  
  - TodoApi: which is a .Net Core project that acts as a backend API for a simple Todo application and uses the authentication token generated from Identity Server 4 for Authorization.

  - TodoConsole: It is a simple console program to test the flow of both projects by generating a token and using it to access the API.


# Features

  - Using Identity Server 4 for generating authorization tokens.
  - Using Identity framework.
  - Using .Net Core Web API for backend with Authorization.
  - CRUD operations for todo app.
  - Simple console application to test the flow of the two projects.

### Installation

First: download the .NET Core project titled "TodoCore" from the following 
link: https://github.com/TamirHAhmed/TodoCore

Apply the migrations and change the Connection string to point at your SQL server.

Run the Identity server project and register a new user.

Second: Using command system, navigate to the folders of "TodoCore" and "TodoAPI", and in each one run the command:
```
dotnet run <app name>
```
after that run the Console app after setting it to the default project, and change the username and pass to match the user you have created earlier and everything should work as expected.

### Notes

This is meant only for demonstration purposes and as a simple boilerplate for beginners to use, do not use this code for production.

Please check damienbod's github for more complete examples on this subject:
https://github.com/damienbod/AspNet5IdentityServerAngularImplicitFlow



### Projects connected to this one

Todo Core:
Asp.net core web api + identity server projects that serves as backend and authorization server respectively.

link: https://github.com/TamirHAhmed/TodoCore

Todo Angular:
Front end for the todo app using angular v4
link: https://github.com/TamirHAhmed/TodoAngular

Todo React Native (Coming soon):
Cross platform mobile app using react native - redux - redux persists.

#### Contribute

Everyone are welcome to contribute this simple boilerplate to have the basics of security integrated with authentication/authorization tokens.
