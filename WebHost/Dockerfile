FROM microsoft/aspnetcore-build:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY WebHost/WebHost.csproj WebHost/
COPY Data/Data.csproj Data/
COPY BLL/BLL.csproj BLL/
COPY Common/Common.csproj Common/
COPY Toggl/Toggl.csproj Toggl/
COPY Tempo/Tempo.csproj Tempo/

RUN dotnet restore WebHost/WebHost.csproj
COPY . .
WORKDIR /src/WebHost
RUN npm install -g webpack@2.5.1
RUN npm install
RUN node node_modules/webpack/bin/webpack.js --config webpack.config.vendor.js --env.prod
RUN node node_modules/webpack/bin/webpack.js --env.prod
RUN dotnet build WebHost.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebHost.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebHost.dll"]
