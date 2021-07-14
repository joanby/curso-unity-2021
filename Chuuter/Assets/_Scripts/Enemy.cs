using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Cantidad de puntos que se obtienen al derrotar al enemigo")]
    public int pointsAmount = 10;
    
    private void OnDestroy()
    {
        ScoreManager.SharedInstance.Amount += pointsAmount;
    }
}
