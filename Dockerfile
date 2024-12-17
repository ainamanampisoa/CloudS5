# Étape 1 : Construire l'application .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le fichier projet
COPY *.csproj ./ 

# Ajouter les packages nécessaires
RUN dotnet add package Npgsql
RUN dotnet add package MailKit
RUN dotnet add package MimeKit

# Restaurer les dépendances
RUN dotnet restore

# Copier le reste du code source et publier l'application
COPY . ./ 
RUN dotnet publish -c Release -o /app

# Étape 2 : Configurer l'image runtime pour l'application .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copier l'application construite
COPY --from=build /app ./

# Exposer le port de l'application
EXPOSE 80

# Démarrer l'application
ENTRYPOINT ["dotnet", "CloudS5.dll"]
