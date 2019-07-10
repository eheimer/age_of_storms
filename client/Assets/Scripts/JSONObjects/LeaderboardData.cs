using System.Collections.Generic;
using System;
using UnityEngine;

public class LeaderboardData : JSONSerializable<LeaderboardData> {
    public class Leader : JSONSerializable<Leader> {
        public string name { get; set; }
        public string gold { get; set; }
    }
    public IList<Leader> leaders { get; set; }

}
