module.exports = {
  Server,
};

const handler = require('../controllers');
const Player = require('../models').Player;

function Server(port, map) {
  console.log('Socket.io server initialized...');
  this.io = require('socket.io')(port);
  let server = this;

  this.io.on('connection', function (client) {
    console.log('Socket.io client connected...');
    //create a new player, requires username, email, password
    client.on('register', function (data) {
      handler.register(server, client, map, data);
    })
    //the player moves
    client.on('move', function (data) {
      handler.move(server, client, map, data);
    });
    //player log in
    client.on('auth', function (data) {
      handler.auth(server, client, map, data);
    });
    client.on('encounteraction', function (data) {
      handler.encounteraction(server, client, map, data);
    });
  });

  this.sendMessage = async function (client, message, broadcast) {
    if (broadcast) {
      client.broadcast.emit(message.event, message.data);
    } else {
      if (!message.data.map) {
        console.log("sending", {
          event: message.event,
          data: message.data
        });
      }
      client.emit(message.event, message.data);
    }
    //update the leaderboard
    let top10 = await Player.findAll({
      attributes: ['name', 'gold'],
      order: [
        ['gold', 'DESC']
      ],
      limit: 10,
    });
    this.sendMessageToAll(this.leaderboardMessage(top10.map((item) => {
      return item.dataValues;
    })));
  }

  this.sendMessageToAll = function (message) {
    console.log(message.data);
    this.io.emit(message.event, message.data);
  }

  this.playerStatusMessage = function (hp, currentHp, gold, x, y) {
    return {
      event: "playerstatus",
      data: {
        hp,
        currentHp,
        gold,
        x,
        y
      }
    };
  }

  this.authSuccessMessage = function (playerId, name) {
    return {
      event: "authsuccess",
      data: {
        playerId,
        name
      }
    }
  }

  this.playerMessage = function (playerId, x, y) {
    return {
      event: "player",
      data: {
        playerId,
        x,
        y
      }
    }
  }

  this.authErrorMessage = function () {
    return {
      event: "autherror",
      data: {
        error: "Username or password invalid"
      }
    }
  }

  this.fightResultMessage = function (enemyHp, playerDamaged) {
    return {
      event: "fightresult",
      data: {
        enemyHp,
        playerDamaged
      }
    }
  }

  this.fleeResultMessage = function (success) {
    return {
      event: "fleeresult",
      data: {
        success,
      }
    }
  }

  this.mapMessage = function (map) {
    return {
      event: "map",
      data: {
        map
      }
    }
  }

  this.encounterMessage = function (hp, ambush) {
    return {
      event: "encounter",
      data: {
        hp,
        ambush
      }
    }
  }

  this.loginErrorMessage = function () {
    return {
      event: "loginerror",
      data: {
        error: "Connection is not logged in."
      }
    }
  }

  this.registerErrorMessage = function (field, error) {
    return {
      event: "registererror",
      data: {
        field,
        error
      }
    }
  }

  this.registerSuccessMessage = function (playerId) {
    return {
      event: "registersuccess",
      data: {
        playerId
      }
    }
  }

  this.leaderboardMessage = function (leaders) {
    return {
      event: "leaderboard",
      data: {
        leaders
      }
    }
  }
}