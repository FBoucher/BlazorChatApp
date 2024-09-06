var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BlazorChatApp>("blazorchatapp");

builder.Build().Run();
