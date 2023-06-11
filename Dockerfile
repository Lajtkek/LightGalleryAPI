FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /LightGallery

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /LightGallery
COPY --from=build-env /LightGallery/out .
ENTRYPOINT ["dotnet", "LightGallery.dll"]