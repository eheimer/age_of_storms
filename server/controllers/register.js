const Player = require('../models').Player;

//create a new player
module.exports = async function (io, client, map, data) {
  // data looks like:
  //{ playerName: "", username: "", password: "" }
  //validate that name and email are not already used
  let existingName = await Player.findAll({
    where: {
      name: data.playerName
    }
  });
  let existingId = await Player.findAll({
    where: {
      userid: data.username
    }
  });
  if (existingId.length > 0) {
    //return error
    io.sendMessage(client, io.registerErrorMessage('username', 'already in use'));
  } else if (existingName.length > 0) {
    //return error
    io.sendMessage(client, io.registerErrorMessage('playerName', 'already in use'));
  } else {
    //create a new player
    let player = await Player.create({
      name: data.playerName,
      userid: data.username,
      password: data.password,
      hp: 100,
      currentHp: 100
    });
    io.sendMessage(client, io.registerSuccessMessage(player.id));
  }
}