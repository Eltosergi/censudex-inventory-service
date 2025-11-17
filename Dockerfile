# Etapa 1: Construcción (Build)
# Usamos el SDK de .NET para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia el archivo de proyecto (.csproj) para restaurar dependencias
# Esto optimiza el caché de Docker.
COPY ["censudex-inventory-service.csproj", "."]
RUN dotnet restore "censudex-inventory-service.csproj"

# Copia el resto del código y compila
COPY . .
# Asumo que el proyecto se llama censudex-inventory-service
RUN dotnet publish "censudex-inventory-service.csproj" -c Release -o /app/publish

# Etapa 2: Imagen final (Runtime)
# Usamos el runtime de ASP.NET para la ejecución (imagen más ligera)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia los archivos publicados de la etapa de construcción
COPY --from=build /app/publish .

# Expone el puerto 8080. Es el puerto que usaremos en docker-compose.yml
EXPOSE 8080

# Configuramos la URL de Kestrel para escuchar en el puerto expuesto
ENV ASPNETCORE_URLS=http://+:8080

# El comando de inicio
ENTRYPOINT ["dotnet", "censudex-inventory-service.dll"]