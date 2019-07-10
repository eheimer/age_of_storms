using System.Collections.Generic;
using System;
using UnityEngine;

public class MapData : JSONSerializable<MapData> {
    public IList<IList<int?>> map { get; set; }

    public int?[,] ToArray() {
        int cols = map[0].Count;
        int rows = map.Count;
        int?[,] arr = new int?[cols, rows];
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < cols; x++) {
                try {
                    arr[x, y] = map[y][x];
                } catch (Exception e) {
                    arr[x, y] = null;
                }
            }
        }
        return arr;
    }
}
