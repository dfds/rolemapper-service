#!/bin/bash

# Defaults:
# INTEGRATION_TEST_KAFKA_PAYLOAD_DIR : PROJECT_ROOT_DIR/src/K8sJanitor.WebApi.IntegrationTests/Kafka/payloads
# INTEGRATION_TEST_KAFKA_RUN : true
# INTEGRATION_TEST_KAFKA_TOPIC : build.capabilities
# INTEGRATION_TEST_KAFKA_FAKE-SERVER : localhost:50901 

KUBERNETES_SERVICE_KAFKA_BOOTSTRAP_SERVERS=localhost:9092 \
KUBERNETES_SERVICE_KAFKA_GROUP_ID=kubernetes-consumer \
KUBERNETES_SERVICE_KAFKA_ENABLE_AUTO_COMMIT=false \
INTEGRATION_TEST_KAFKA_TOPIC=build.capabilities \
INTEGRATION_TEST_KAFKA_FAKE_SERVER_HOST=localhost:50901 \
INTEGRATION_TEST_KAFKA_PAYLOAD_DIR=../../../Kafka/payloads \
EXECUTE_AGAINST_KAFKA=true \
EXECUTE_AGAINST_K8S=true \
dotnet test ./../src/K8sJanitor.WebApi.IntegrationTests/K8sJanitor.WebApi.IntegrationTests.csproj
