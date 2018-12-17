FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-stretch-slim

WORKDIR /app
COPY ./output/app ./

ENTRYPOINT [ "dotnet", "RolemapperService.WebApi.dll" ]