# 1) Build 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy
COPY ["SupportChat.sln", "./"]
COPY ["src/SupportChat.Api/SupportChat.Api.csproj", "src/SupportChat.Api/"]
COPY ["src/SupportChat.Application/SupportChat.Application.csproj", "src/SupportChat.Application/"]
COPY ["src/SupportChat.Domain/SupportChat.Domain.csproj", "src/SupportChat.Domain/"]
COPY ["src/SupportChat.Infrastructure/SupportChat.Infrastructure.csproj", "src/SupportChat.Infrastructure/"]
COPY ["test/SupportChat.Test/SupportChat.Test.csproj", "test/SupportChat.Test/"]

# Restore
RUN dotnet restore "SupportChat.sln"

# coppy publish
COPY . .
WORKDIR "/src/src/SupportChat.Api"
RUN dotnet publish -c Release -o /app/publish

# Runtime 
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# environment
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:7000
EXPOSE 7000

#
COPY --from=build /app/publish .

# start
ENTRYPOINT ["dotnet", "SupportChat.Api.dll"]
