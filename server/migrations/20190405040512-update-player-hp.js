'use strict';

module.exports = {
  up: (queryInterface, Sequelize) => {
    return queryInterface.sequelize.transaction((t) => {
      return Promise.all([
        queryInterface.addColumn('Players', 'hp', {
          type: Sequelize.NUMBER,
          allowNull: false,
          defaultValue: 100
        }, {
          transaction: t
        }),
        queryInterface.addColumn('Players', 'currentHp', {
          type: Sequelize.NUMBER,
          allowNull: false,
          defaultValue: 100
        }, {
          transaction: t
        })
      ])
    })
  },

  down: (queryInterface, Sequelize) => {
    return queryInterface.sequelize.transaction((t) => {
      return Promise.all([
        queryInterface.removeColumn('Players', 'hp', {
          transaction: t
        }),
        queryInterface.removeColumn('Players', 'currentHp', {
          transaction: t
        })
      ])
    })
  }
};