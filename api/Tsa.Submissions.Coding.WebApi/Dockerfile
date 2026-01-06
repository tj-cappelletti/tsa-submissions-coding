#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Tsa.Submissions.Coding.WebApi/Tsa.Submissions.Coding.WebApi.csproj", "Tsa.Submissions.Coding.WebApi/"]
RUN dotnet restore "Tsa.Submissions.Coding.WebApi/Tsa.Submissions.Coding.WebApi.csproj"
COPY . .
WORKDIR "/src/Tsa.Submissions.Coding.WebApi"
RUN dotnet build "Tsa.Submissions.Coding.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tsa.Submissions.Coding.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tsa.Submissions.Coding.WebApi.dll"]