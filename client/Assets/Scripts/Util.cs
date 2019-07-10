using UnityEngine;

public class Util
{
  public static int WeightedRandom(float[] weights)
  {
    float sumOfWeight = 0;
    for (int i = 0; i < weights.Length; i++)
    {
      sumOfWeight += weights[i];
    }
    float rnd = Mathf.FloorToInt(Random.Range(0f, 0.999f) * sumOfWeight);
    for (int i = 0; i < weights.Length; i++)
    {
      if (rnd < weights[i])
      {
        return i;
      }
      rnd -= weights[i];
    }
    return 0; //this shouldn't ever happen;
  }
}