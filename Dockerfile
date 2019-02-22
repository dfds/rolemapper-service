FROM microsoft/dotnet:2.2.1-aspnetcore-runtime

WORKDIR /app
COPY ./output/app ./

ENTRYPOINT [ "dotnet", "RolemapperService.WebApi.dll" ]
