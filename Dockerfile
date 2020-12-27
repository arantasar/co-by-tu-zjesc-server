FROM mcr.microsoft.com/dotnet/sdk:3.1.404-alpine3.12 AS build
WORKDIR /src

COPY *.sln .
COPY ./API/*.csproj ./API/
COPY ./Domain/*.csproj ./Domain/
COPY ./Persistence/*.csproj ./Persistence/

RUN dotnet restore
COPY . .

RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:3.1.10-alpine3.12
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "API.dll"]