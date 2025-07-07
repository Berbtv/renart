# ASP.NET çalıştırmak için gerekli temel imaj
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# SDK içeren imajla build işlemi
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve restore et
COPY ["Proje.csproj", "./"]
RUN dotnet restore "./Proje.csproj"

# Diğer tüm dosyaları kopyala ve yayına hazırla
COPY . .
RUN dotnet publish "Proje.csproj" -c Release -o /app/publish

# Yayınlanmış uygulamayı çalıştır
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Proje.dll"]
