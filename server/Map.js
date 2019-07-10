module.exports.Map = class Map {

  //The map array is configured as map[row][col] and starts in
  //the lower left hand corner (map[0][0]) and ends in the upper
  //right hand corner (map[height][width]);
  constructor(width, height, adj) {
    this.map = [];
    this.width = width;
    this.height = height;
    // adj = 0
    // 0 is completely random
    // anything > 0 increases the probability that like tiles will be 
    // next to each other.
    if (!adj) adj = 0;
    for (let y = 0; y < height; y++) {
      this.map[y] = [];
      for (let x = 0; x < width; x++) {
        let weights = [3, 1, 1, 1];
        //random value from 0 to 3
        let left = this.map[y][x - 1];
        if (left || left === 0) weights[left] = weights[left] + adj;
        if (y > 0) {
          let downleft = this.map[y - 1][x - 1];
          if (downleft || downleft === 0) weights[downleft] = weights[downleft] + adj;
          let down = this.map[y - 1][x];
          if (down || down === 0) weights[down] = weights[down] + adj;
          let downright = this.map[y - 1][x + 1];
          if (downright || downright === 0) weights[downright] = weights[downright] + adj;
        }
        this.map[y][x] = this.weightedRandom(weights);
      }
    }
  }

  getTile(x, y) {
    return this.map[y][x]
  }

  tileIs(x, y, value) {
    return this.map[y][x] === value;
  }

  tileIsPassable(x, y) {
    return this.tileIs(x, y, 0) || this.tileIs(x, y, 2);
  }

  mapColumns() {
    if (!this.map || !this.mapRows() === 0) return 0;
    return this.map[0].length;
  }

  mapRows() {
    return this.map.length;
  }

  getStartingPosition() {
    let x;
    let y;
    do {
      x = Math.floor(Math.random() * this.width);
      y = Math.floor(Math.random() * this.height);
      console.log("starting position", {
        x
      }, {
        y
      }, this.getTile(x, y));
    } while (!this.tileIsPassable(x, y));
    return {
      x,
      y
    };
  }

  getFleePosition(x, y) {
    let moveX = 0;
    let moveY = 0;
    while (moveX === moveY === 0 || !this.tileIsPassable(x + moveX, y + moveY)) {
      moveX = Math.floor(Math.random() * 3 - 1);
      moveY = Math.floor(Math.random() * 3 - 1);
    }
    return {
      x: x + moveX,
      y: y + moveY
    }
  }


  getMap(centerX, centerY, width, height) {
    let startX = centerX - Math.floor(width / 2);
    let startY = centerY - Math.floor(height / 2);
    let localMap = [];
    for (let y = 0; y < height; y++) {
      localMap[y] = [];
      let thisY = startY + y;
      for (let x = 0; x < width; x++) {
        let thisX = startX + x;
        if (thisX < 0 || thisY < 0 || thisX >= this.mapColumns() || thisY >= this.mapRows()) {
          localMap[y][x] = null;
        } else {
          localMap[y][x] = this.getTile(thisX, thisY);
        }
      }
    }
    return localMap;
  }

  /**
   * Takes an array of weights and returns a random element id
   *
   * @param {*} weights array of weights
   */
  weightedRandom(weights) {
    var sumOfWeight = 0;
    for (var i = 0; i < weights.length; i++) {
      sumOfWeight += weights[i];
    }
    var rnd = Math.floor(Math.random() * sumOfWeight);
    for (i = 0; i < weights.length; i++) {
      if (rnd < weights[i]) {
        return i;
      }
      rnd -= weights[i];
    }
  }

}