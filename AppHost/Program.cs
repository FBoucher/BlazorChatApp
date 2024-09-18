using Aspire.Hosting;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Projects;
using System.Data;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BlazorChatApp>("blazorchatapp")
            .WithEnvironment("OTEL_SERVICE_NAME", "blazorchatapp")
            .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317")
            .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
            .WithEnvironment("OTEL_RESOURCE_ATTRIBUTES", "deployment.environment = docker,host.name = otelcol - docker")
            .WithEnvironment("DD_AGENT_HOST", "datadog");

builder.Build().Run();
