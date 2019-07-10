const Player = require('../models').Player;

//handle player login
module.exports = async function (io, client, map, data) {
  // data looks like:
  //{ username: "", password: "" }
  console.log({
    io: io.TEST,
    io2: io.TEST2,
  });
  let player = await Player.findOne({
    where: {
      userid: data.username,
      password: data.password
    }
  });
  if (player) {
    if (!player.x || !player.y || (player.x && player.y && !map.tileIsPassable(player.x, player.y))) {
      let startingPosition = map.getStartingPosition();
      player = await player.update({
        x: startingPosition.x,
        y: startingPosition.y
      });
    }
    client.player = player;
    io.sendMessage(client, io.authSuccessMessage(player.id, player.name));
    io.sendMessage(client, io.playerStatusMessage(player.hp, player.currentHp, player.gold, player.x, player.y));
    //broadcast the player's location to all other clients
    io.sendMessage(client, io.playerMessage(player.id, player.x, player.y), true);
  } else {
    io.sendMessage(client, io.authErrorMessage());
  }
}