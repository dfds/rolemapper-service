#!/bin/bash

HARALD_KAFKA_BOOTSTRAP_SERVERS=localhost:9092 \
HARALD_KAFKA_GROUP_ID=harald-consumer \
HARALD_KAFKA_ENABLE_AUTO_COMMIT=false \
dotnet watch --project ./../src/RolemapperService.WebApi/RolemapperService.WebApi.csproj run
