using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager SharedInstance;

    public List<WaveSpawner> waves;

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
            waves = new List<WaveSpawner>();
        }
        else
        {
            Destroy(this);
        }
    }
}
