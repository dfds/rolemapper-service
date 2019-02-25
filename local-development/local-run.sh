#!/bin/bash

docker-compose up &

dotnet watch --project ./../src/RolemapperService.WebApi/RolemapperService.WebApi.csproj run
