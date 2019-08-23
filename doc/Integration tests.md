# Integration tests

## Setup

In order to run the tests, access to certain ENV VARS from wherever the tests is being run from must be ensured. The tests will if nothing is configured by the user, use the defaults specified below.

## Enviroment variables (ENV VARS)

The integration tests for K8sJanitor expects certain environment variables in order to function.

#### Toggling tests:
* *EXECUTE_AGAINST_KAFKA* when set to **true** will run integration tests for Kafka. If **false**, tests for Kafka will be skipped.
    
    **Default**: true
* *EXECUTE_AGAINST_K8S* when set to **true** will run integration tests for Kubernetes. If **false**, tests for Kubernetes will be skipped.
    
    **Default**: true

#### Kafka settings:
* *INTEGRATION_TEST_KAFKA_TOPIC* is used to set the topic that the tests uses for consuming and publishing Kafka messages.
    
    **Default**: 'build.capabilities'
* *INTEGRATION_TEST_KAFKA_FAKE_SERVER_HOST* is the port that 'integration-test-api-server' from local-development docker-compose is listening on.
    
    **Default**: 'localhost:50901'
* *INTEGRATION_TEST_KAFKA_PAYLOAD_DIR* path to directory where .json event payloads used in tests can be found.
    
    **Default**: '../../../Kafka/payloads'
* *KUBERNETES_SERVICE_KAFKA_BOOTSTRAP_SERVERS* Kafka bootstrap server host.
    
    **Default**: 'localhost:9092'
* *KUBERNETES_SERVICE_KAFKA_GROUP_ID* Kafka group id used for tests.
    
    **Default**: 'kubernetes-consumer'
* *KUBERNETES_SERVICE_KAFKA_ENABLE_AUTO_COMMIT* Let Kafka client library handle committing the message.
    
    **Default**: false