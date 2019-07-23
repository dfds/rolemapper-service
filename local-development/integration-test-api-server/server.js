const express = require("express");
const fs = require("fs");
const util = require("util");

const port = process.env.port || 3000;

const app = express();
app.use(express.json());

const readFile = util.promisify(fs.readFile);
const writeFile = util.promisify(fs.writeFile);
const serialize = (data) => JSON.stringify(data, null, 2);
const deserialize = (text) => JSON.parse(text);

var counter = 0;

app.get("/api-calls-received", (req, res) => {
    counter = counter + 1;
    return res.json({apiCallsReceived: counter, success: true});
});

app.get("/api-calls-reset", (req, res) => {
    counter = 0;
    return res.json({success: true});
});


app.listen(port, () => {
    console.log("Fake team service is listening on port " + port);
});