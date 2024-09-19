# Blazor Chat App

This is a simple AI-powered Blazor chat app that uses [Semantic Kernel](https://aka.ms/semantic-kernel) with a custom [plugin](https://learn.microsoft.com/semantic-kernel/concepts/plugins) to enable the AI assistant to change the theme colors of the app. 

It leverage .NET Aspire to enhance the developer experience when being executed locally. And can send OpenTelemetry to Azure Application Insight or Datadog. 


## Configure the desire AI backend

It's possible to use the App with different AI models: Azure OpenAI, GitHub Model, and Local Ollama Model.

### Use Azure OpenAI

1. [Create and deploy an Azure OpenAI resource](https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource).
1. This app uses the [Azure Identity client library](https://learn.microsoft.com/dotnet/api/overview/azure/identity-readme) to authenticate with Azure OpenAI. Use [Role-based access control](https://learn.microsoft.com/dotnet/api/overview/azure/identity-readme) to grant permissions to the appropriate identity or group to have access to the Azure OpenAI resource.
1. Configure the deployment name and endpoint URL for your Azure OpenAI resource:
    ```json
    {
      "SmartComponents": {
        "DeploymentName": "<Azure OpenAI deployment name>",
        "Endpoint": "<Azure OpenAI endpoint URL>"
      }
    }
    ```
1. In the `Program.cs` file, make sure the appropriate Azure OpenAI section is uncommented:
    ```csharp
    var aiConfig = builder.Configuration.GetSection("SmartComponents");

    builder.Services.AddAzureOpenAIChatCompletion(
        deploymentName: aiConfig["DeploymentName"]!,
        endpoint: aiConfig["Endpoint"]!,
        new DefaultAzureCredential());
    ```

### Use GitHub Model backend

1. [Follow the instruction on GitHub Model](https://github.com/marketplace/models/azure-openai/gpt-4o-mini) to retreive your *token* and *modelId*
1. Configure the deployment name and endpoint URL for your Azure OpenAI resource:
    ```json
    {
      "GHComponents": {
        "DeploymentName": "<model_name>",
        "Endpoint": "https://models.inference.ai.azure.com/",
        "ApiKey": "<github-token>"
      }
    }
    ```
1. In the `Program.cs` file, make sure the appropriate Azure OpenAI section is uncommented:
    ```csharp
    var aiConfig = builder.Configuration.GetSection("GHComponents");
    var githubPAT = aiConfig["ApiKey"]!;

    var client = new OpenAIClient(
          new ApiKeyCredential(githubPAT), 
          new OpenAIClientOptions { Endpoint = new Uri("https://models.inference.ai.azure.com") });

    builder.Services.AddOpenAIChatCompletion(aiConfig["DeploymentName"]!, client);

    ```

### Use local Ollama backend

1. In the `Program.cs` file, make sure the appropriate Azure OpenAI section is uncommented:
    ```csharp
    #pragma warning disable SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0052

    builder.Services.AddOpenAIChatCompletion(
        modelId: "llama3.1",
        endpoint: new Uri(builder.Configuration["LOCAL_LLM"] ?? "http://localhost:11434/"),
        apiKey: "apikey");
    ```
  **Note:** the apiKey can be anything as it is not used by the local model.
1. You can install [Ollama locally](https://ollama.com/) or have in running in a container. For this demo docker container are used. Execute this first command to get Ollama in a running container:   
    ```bash
    docker run -d -v /path/where/ollama/model/are/downloaded:/root/.ollama -p 11434:11434 --name ollama ollama/ollama:latest
    ```
1. You can now execute this second command to download the model and start the service:
    ```bash
    docker exec -it ollama ollama run llama3.1
    ```

## (Optional) Configure Datadog Agent

You can configure the app to send OpenTelemetry traces to Datadog. To do this, you need to get a Datadog API key. 

1. In the `docker-compose.yml` file, set the value of you key for the environment `DD_API_KEY` (line 12).
1. (optional) If you decided to use Ollama as the AI backend, you can also uncomment the ollama service in the `docker-compose.yml`.
1. Run the following command to start the container(s) the app:
    ```bash
    docker-compose up --build -d
    ```


## Run the app

1. You can access the app at `http://localhost:8080` as it was packaged in a container. Or you can lauch the app from your Code Editor (Visual Studio IDE, Visual Studio Code).
1. Start chatting with the AI assistant. Try asking the AI assistant to change the theme colors of the app.

![Blazor chat app screenshot](https://github.com/user-attachments/assets/a783b26c-433c-43e8-9e4d-84e5c8f60cb8)
