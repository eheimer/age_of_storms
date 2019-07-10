'use strict';
module.exports = (sequelize, DataTypes) => {
  const Player = sequelize.define('Player', {
    name: DataTypes.STRING,
    userid: DataTypes.STRING,
    password: DataTypes.STRING,
    x: DataTypes.NUMBER,
    y: DataTypes.NUMBER,
    hp: DataTypes.NUMBER,
    currentHp: DataTypes.NUMBER,
    gold: DataTypes.NUMBER,
  }, {});
  Player.associate = function (models) {
    // associations can be defined here
    Player.hasOne(models.Encounter, {
      as: 'encounter'
    });
  };
  return Player;
};