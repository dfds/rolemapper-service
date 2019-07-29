const express = require("express");
const fs = require("fs");
const util = require("util");
const { Kafka } = require('kafkajs')

const port = process.env.port || 50901;

const app = express();
app.use(express.json());

const readFile = util.promisify(fs.readFile);
const writeFile = util.promisify(fs.writeFile);
const serialize = (data) => JSON.stringify(data, null, 2);
const deserialize = (text) => JSON.parse(text);

var counter = 0;
var kafkaMessageReceivedCounter = 0;

app.get("/api-calls-received", (req, res) => {
    counter = counter + 1;
    return res.json({apiCallsReceived: counter, kafkaMessageReceived: kafkaMessageReceivedCounter, success: true});
});

app.get("/api-calls-reset", (req, res) => {
    counter = 0;
    kafkaMessageReceivedCounter = 0;
    return res.json({success: true});
});


(async () => {
    // new kafka code
    const kafka = new Kafka({
        clientId: 'integrationTestApiServer',
        brokers: ['localhost:9092'],
        connectionTimeout: 30000,
        retry: {
            retries: 30,
            initialRetryTime: 3000,
            maxRetryTime: 30000
        }
    })

    const consumer = kafka.consumer({ groupId: 'integrationTestApiServer'});
    await consumer.connect();
    await consumer.subscribe({topic: 'build.capabilities'});

    consumer.run({
        eachMessage: async ({ topic, partition, message}) => {
            console.log({
                value: message.value.toString(),
            })
            kafkaMessageReceivedCounter = kafkaMessageReceivedCounter + 1;
        }
    });
})();



app.listen(port, () => {
    console.log("Fake team service is listening on port " + port);
});