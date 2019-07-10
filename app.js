// app.js
const express = require('express');
const logger = require('morgan');
const bodyParser = require('body-parser');
const path = require('path');

var app = express();
app.use(logger('dev'));

//app.use(express.static(__dirname + '/node_modules'));

// Parse incoming requests data (https://github.com/expressjs/body-parser)
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
  extended: false
}));

app.use(express.static(path.join(__dirname, 'server', 'public')));

require('./server/routes')(app);
// Setup a default catch-all route that sends back a welcome message in JSON format.
app.get('*', (req, res) => res.status(200).send({
  message: 'Welcome to the beginning of nothingness.',
}));

module.exports = app;