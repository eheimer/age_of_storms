const Encounter = require('../models').Encounter;

//handle player encounter action
module.exports = async function (io, client, map, data) {
  // data looks like:
  //{ action: <fight/flee> }
  let encounter = await Encounter.findOne({
    where: {
      playerId: client.player.id
    },
    attributes: ["id", "hp", "ambush"]
  });
  if (data.action === "fight") {
    let playerHits = Math.floor(Math.random() * 2) === 0;
    let enemyHits = Math.floor(Math.random() * 2) === 0;
    if (playerHits) {
      if (encounter) {
        encounter = await encounter.update({
          hp: encounter.hp - 10
        });
      }
    }
    let playerDead = false;
    let enemyDead = encounter.hp <= 0;
    if (!enemyDead && enemyHits) {
      let newPlayerHp = client.player.currentHp - 10;
      playerDead = newPlayerHp <= 0;
      if (playerDead) {
        await client.player.update({
          currentHp: client.player.hp,
          gold: Math.floor(client.player.gold / 2),
        });
        client.player = undefined;
      } else {
        client.player = await client.player.update({
          currentHp: newPlayerHp,
        });
      }
    }
    if (!playerDead) {
      io.sendMessage(client, io.fightResultMessage(encounter.hp, enemyHits));
      if (enemyDead) {
        client.player = await client.player.update({
          gold: client.player.gold + 10
        });
      }
    }
    if (playerDead || enemyDead) {
      await encounter.destroy();
    }
    io.sendMessage(client, io.playerStatusMessage(undefined, client.player ? client.player.currentHp : 0, client.player ? client.player.gold : undefined, undefined, undefined));
  } else if (data.action === "flee") {
    let canflee = Math.floor(Math.random() * 2) === 0;
    if (canflee) {
      let pos = map.getFleePosition(client.player.x, client.player.y);
      await client.player.update({
        x: pos.x,
        y: pos.y
      });
      await encounter.destroy();
    }
    io.sendMessage(client, io.fleeResultMessage(canflee));
    if (canflee) {
      io.sendMessage(client, io.playerStatusMessage(undefined, undefined, undefined, client.player.x, client.player.y));
    }
  }
}