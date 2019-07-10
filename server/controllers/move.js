const Encounter = require('../models').Encounter;
/**
 * when a player moves, we update their position
 * then send them a new map, then send their new
 * position to all of the other connected clients
 * TODO: validate the movement, and send error
 * back to client if invalid.
 */
module.exports = async function (io, client, map, data) {
  // data looks like:
  //{ playerId: 1, x: 0, y: 0, width: 20, height: 12 }
  if (client.player) {
    //TODO: validate movement, if not valid, send 'moveerror' response
    // client.emit('moveerror', { error: 'you cannot move to the requested location' });
    //player heals 1 hp per move
    if (client.player.currentHp < client.player.hp) {
      client.player.currentHp++;
    }
    client.player = await client.player.update({
      x: data.x,
      y: data.y,
      currentHp: client.player.currentHp
    });
    io.sendMessage(client, io.playerStatusMessage(undefined, client.player.currentHp, undefined, undefined, undefined));
    //send a new map to the player
    io.sendMessage(client, io.mapMessage(map.getMap(data.x, data.y, data.width, data.height)));
    //20% chance of encounter
    if (Math.floor((Math.random() * 5)) === 0) {
      let ambush = Math.floor((Math.random() * 5)) === 0;
      let hp = Math.floor((Math.random() * 8) + 18);
      await Encounter.create({
        hp,
        ambush,
        playerId: client.player.id
      });
      io.sendMessage(client, io.encounterMessage(hp, ambush));
    }
    //broadcast the player's location to the rest of the clients
    io.sendMessage(client, io.playerMessage(client.player.id, Number(client.player.x), Number(client.player.y)), true);
  } else {
    io.sendMessage(client, io.loginErrorMessage());
  }
}