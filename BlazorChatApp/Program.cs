using Azure.Identity;
using BlazorChatApp;
using BlazorChatApp.Components;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddKernel();

// var aiConfig = builder.Configuration.GetSection("SmartComponents");
// builder.Services.AddAzureOpenAIChatCompletion(
//     deploymentName: aiConfig["DeploymentName"]!,
//     endpoint: aiConfig["Endpoint"]!,
//     new DefaultAzureCredential());

#pragma warning disable SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0052

var aiConfig = builder.Configuration.GetSection("LocalSmart");
builder.Services.AddOpenAIChatCompletion(
    modelId: "llama3.1",
    endpoint: new Uri("http://localhost:11434/"),
    apiKey: "apikey");

builder.Services.AddScoped(sp => KernelPluginFactory.CreateFromType<ThemePlugin>(serviceProvider: sp));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
