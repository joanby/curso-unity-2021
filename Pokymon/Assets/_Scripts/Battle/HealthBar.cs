using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    public GameObject healthBar;

    /*private void Start()
    {
        healthBar.transform.localScale = new Vector3(0.5f, 1.0f);
    }*/


    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1.0f);
    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;
        while (currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1);
            yield return null;
        }
        
        healthBar.transform.localScale = new Vector3(normalizedValue, 1);
    }
}
