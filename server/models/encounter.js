'use strict';
module.exports = (sequelize, DataTypes) => {
  const Encounter = sequelize.define('Encounter', {
    hp: DataTypes.NUMBER,
    ambush: DataTypes.BOOLEAN,
    playerId: DataTypes.NUMBER,
  }, {});
  Encounter.associate = function (models) {
    // associations can be defined here
    Encounter.belongsTo(models.Player, {
      foreignKey: "playerId"
    });
  };
  return Encounter;
};