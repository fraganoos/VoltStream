# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ✅ wait-for-it.sh faylni nusxalab, bajariladigan qilamiz
COPY src/backend/VoltStream.WebApi/wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

# ✅ Project fayllarni nusxalash
COPY ["src/backend/VoltStream.WebApi/VoltStream.WebApi.csproj", "src/backend/VoltStream.WebApi/"]
COPY ["src/backend/VoltStream.Application/VoltStream.Application.csproj", "src/backend/VoltStream.Application/"]
COPY ["src/backend/VoltStream.Domain/VoltStream.Domain.csproj", "src/backend/VoltStream.Domain/"]
COPY ["src/backend/VoltStream.Infrastructure/VoltStream.Infrastructure.csproj", "src/backend/VoltStream.Infrastructure/"]

# ✅ Restore
RUN dotnet restore "src/backend/VoltStream.WebApi/VoltStream.WebApi.csproj"

# ✅ Source fayllarni nusxalash
COPY . .

# ✅ Build
WORKDIR "/src/src/backend/VoltStream.WebApi"
RUN dotnet build "VoltStream.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VoltStream.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app

# ✅ Faylni build stage’dan nusxalaymiz (chmod allaqachon bajarilgan)
COPY --from=build /wait-for-it.sh /wait-for-it.sh

# ✅ Publish qilingan fayllarni nusxalaymiz
COPY --from=publish /app/publish .

# ❗ ENTRYPOINT o‘zgartirilmaydi — docker-compose.yml orqali command beriladi
# ENTRYPOINT ["dotnet", "VoltStream.WebApi.dll"]
