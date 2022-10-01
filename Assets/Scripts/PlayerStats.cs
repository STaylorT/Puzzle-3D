using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    // array of times for each level

    private List<float> BestTimes = new List<float>();


    public void addBestTime(float time)
    {
        BestTimes.Add(time);
    }
}
