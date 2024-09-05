FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
 
# Download the latest version of the tracer but don't install yet
RUN curl -Lo /tmp/datadog-dotnet-apm.deb https://github.com/DataDog/dd-trace-dotnet/releases/download/v3.2.0/datadog-dotnet-apm_3.2.0_arm64.deb
 
WORKDIR /src
COPY ["BlazorChatApp/BlazorChatApp.csproj", "BlazorChatApp/"]
COPY ["BlazorChatApp.ServiceDefaults/BlazorChatApp.ServiceDefaults.csproj", "BlazorChatApp.ServiceDefaults/"]
RUN dotnet restore "BlazorChatApp/BlazorChatApp.csproj"
COPY . .
WORKDIR "/src/BlazorChatApp"
RUN dotnet build "BlazorChatApp.csproj" -c Release -o /app/build
 
FROM build AS publish
RUN dotnet publish "BlazorChatApp.csproj" -c Release -o /app/publish
 
FROM base AS final
 
# Copy the tracer from build target
COPY --from=build /tmp/datadog-dotnet-apm.deb /tmp/datadog-dotnet-apm.deb
# Install the tracer
RUN mkdir -p /opt/datadog \
    && mkdir -p /var/log/datadog \
    && dpkg -i /tmp/datadog-dotnet-apm.deb \
    && rm /tmp/datadog-dotnet-apm.deb
 
# Enable the tracer
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorChatApp.dll"]