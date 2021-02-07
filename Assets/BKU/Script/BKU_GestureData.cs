using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BKU_GestureData
{
    float[][] distance;
    float[] angle;
    public float[][] Distance { get => distance; set=> distance = value; }
    public float[] Angle { get => angle; set => angle = value; }
    public BKU_GestureData(int distanceIndex, int angleIndex)
    {
        distance = new float[distanceIndex][];
        angle = new float[angleIndex];
        for (int i = 0; i < distance.Length; i++)
        {
            distance[i] = new float[i + 1];
        }

    }

}
