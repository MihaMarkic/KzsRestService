FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY KzsRest.sln ./
COPY ["KzsRest/KzsRest.csproj", "KzsRest/"]
COPY ["KzsRest.Engine/KzsRest.Engine.csproj", "KzsRest.Engine/"]
COPY ["KzsRest.Models/KzsRest.Models.csproj", "KzsRest.Models/"]
COPY ["Test/KzsRest.Engine.Test/KzsRest.Engine.Test.csproj", "Test/KzsRest.Engine.Test/"]
# RUN dotnet restore "KzsRest/KzsRest.csproj"
# RUN dotnet restore "KzsRest.Engine/KzsRest.Engine.csproj"
# RUN dotnet restore "KzsRest.Models/KzsRest.Models.csproj"
RUN dotnet restore
COPY . .
RUN dotnet build "KzsRest.sln" -c Release -o /build
RUN dotnet test "Test/KzsRest.Engine.Test/KzsRest.Engine.Test.csproj" --no-restore -c Release

FROM build AS publish
RUN dotnet publish "KzsRest/KzsRest.csproj" --no-restore --no-build -c Release -o /app
#COPY ["KzsRest/Content","/app/Content"]
RUN ls /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN apt-get update && apt-get install -y --no-install-recommends libfontconfig
ENTRYPOINT ["dotnet", "KzsRest.dll"]