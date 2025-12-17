var builder = DistributedApplication.CreateBuilder(args);

var moviesApi = builder.AddProject<Projects.Movies_API>("movies-api");
var usersApi = builder.AddProject<Projects.Users_API>("users-api");

builder.Build().Run();