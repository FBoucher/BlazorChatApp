using Azure.Identity;
using BlazorChatApp;
using BlazorChatApp.Components;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Aspire update
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Semantic Kernel
builder.Services.AddKernel();

// Add DetailedErrors
builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });

// Use this when Azure OpenAI
var aiConfig = builder.Configuration.GetSection("SmartComponents");
//builder.Services.AddAzureOpenAIChatCompletion(
//    deploymentName: aiConfig["DeploymentName"]!,
//    endpoint: aiConfig["Endpoint"]!,
//    new DefaultAzureCredential());

builder.Services.AddAzureOpenAIChatCompletion( 
    deploymentName: aiConfig["DeploymentName"]!,
    endpoint: aiConfig["Endpoint"]!,
    apiKey: aiConfig["ApiKey"]!);

// Use this when LLM is local
//#pragma warning disable SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0052

//builder.Services.AddOpenAIChatCompletion(
//    modelId: "llama3.1",
//    endpoint: new Uri(builder.Configuration["LOCAL_LLM"] ?? "http://localhost:11434/"),
//    apiKey: "apikey");

//builder.Services.AddScoped(sp => KernelPluginFactory.CreateFromType<ThemePlugin>(serviceProvider: sp));


var app = builder.Build();

// Aspire update
app.MapDefaultEndpoints();

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
