# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivo de proyecto (nota: sin subcarpeta)
COPY DevsuApp.BE.csproj .
RUN dotnet restore "DevsuApp.BE.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar la aplicación
RUN dotnet build "DevsuApp.BE.csproj" -c Release -o /app/build

# Publicar la aplicación
RUN dotnet publish "DevsuApp.BE.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DevsuApp.BE.dll"]
