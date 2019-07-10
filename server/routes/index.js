module.exports = (app) => {
  app.get('/', function (req, res, next) {
    res.sendFile(__dirname + '/../public/index.html');
  });
};