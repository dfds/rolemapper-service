FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-stretch-slim

WORKDIR /app
COPY ./output/app ./

ENTRYPOINT [ "dotnet", "K8sJanitor.WebApi.dll" ]
