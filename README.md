# k8s-janitor Service

[![Build Status](https://dfds.visualstudio.com/DevelopmentExcellence/_apis/build/status/k8s-janitorservice-ci)](https://dfds.visualstudio.com/DevelopmentExcellence/_build/latest?definitionId=967)

Service managing k8s cluster with our business logics.

## Development

### Prerequisites

- .NET Core 2.2 SDK ([download](https://dotnet.microsoft.com/download/dotnet-core/2.2))
- Docker (any relatively new version will do)
- All scripts are written in bash. You can use gitbash on windows

Other than the above prerequisites this application requires no additional
special setup on your machine. Open the solution or root folder in your
editor of choice and start cracking!

## Access to Kubernetes

The The application operates on a Kubernetes cluster. The credentials from current-context in ./kube/config will be used if the environment variables `KUBERNETES_SERVICE_HOST` & `KUBERNETES_SERVICE_PORT` are not set.

## Running the application locally

The folder local-development contains bash scripts that enables you to run the application locally while developing.

- start-dependencies.sh starts a kafka cluster
- watch-run.sh starts the api project with environment variables set for local development and will rebuild on file change
- watch-run-unit-tests.sh runs unit tests and will rerun them on file change

The file `rest-request.http` contains some rest request you can use for development, you can use the vsts plugin `humao.rest-client` to execute the requests

### Environment variables

The application requires the following environment variables when running locally:

| Name | Description |
|------|-------------|
| KUBERNETES_SERVICE_KAFKA_BOOTSTRAP_SERVERS | A list of host/port pairs to use for establishing the initial connection to the Kafka cluster.
| KUBERNETES_SERVICE_KAFKA_GROUP_ID | Id of the consumer group that the application will join. Only a single consumer in a group will read a message.
| KUBERNETES_SERVICE_KAFKA_ENABLE_AUTO_COMMIT | commit the Offset on Consumer fetches or manually.

### Running in a container

To run the application you first need to execute a script located in the repository
root. This will restore any dependencies and build both the application and also a
container image using Docker. Run the following on your command line in the repository
root:

```shell
./pipeline.sh
```
Now you should be able to start a container by running the following on your command line:

```shell
docker run -it --rm -p 8080:80 k8s-janitor
```

You should be able to navigate to `http://localhost:8080` in a browser.

## Running in production

### Environment variables

The application requires the following environment variables when running in production:

| Name | Description |
|------|-------------|
| AWS_S3_BUCKET_REGION | The region the buckets exist in|
| AWS_S3_BUCKET_NAME_CONFIG_MAP | name of the bucket containing the config map file
| CONFIG_MAP_FILE_NAME | Name of the file the configmap will be stored in.
| KUBERNETES_SERVICE_KAFKA_BOOTSTRAP_SERVERS | A list of host/port pairs to use for establishing the initial connection to the Kafka cluster.
| KUBERNETES_SERVICE_KAFKA_GROUP_ID | Id of the consumer group that the application will join. Only a single consumer in a group will read a message.
| KUBERNETES_SERVICE_KAFKA_SASL_PASSWORD | Kafka Simple Authentication and Security Layer password
| KUBERNETES_SERVICE_KAFKA_SASL_USERNAME |  Kafka Simple Authentication and Security Layer username