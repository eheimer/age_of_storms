'use strict';
const fs = require('fs');
const path = require('path');
const basename = path.basename(module.filename);
const controllers = {};

let files = fs.readdirSync(__dirname).filter(file => (file.indexOf('.') !== 0) &&
  (file !== basename) &&
  (file.slice(-3) === '.js'));

for (let file of files) {
  const name = file.slice(0, file.length - 3);
  controllers[name] = require('./' + file);
}

module.exports = controllers;