using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public GameObject healthBar;



    public Color BarColor
    {
        get
        {
            var localScale = healthBar.transform.localScale.x;
            if (localScale < 0.15f)
            {
                return new Color(193f/255, 45f/255, 45f/255);
            }else if (localScale < 0.5f)
            {
                return new Color(211f/255, 211f/255, 29f/255);
            }
            else
            {
                return new Color(98f/255, 178f/255, 61f/255);
            }
        }
    }
    
    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1.0f);
        healthBar.GetComponent<Image>().color = BarColor;

    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;
        while (currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1);
            healthBar.GetComponent<Image>().color = BarColor;
            yield return null;
        }
        
        healthBar.transform.localScale = new Vector3(normalizedValue, 1);
    }
}
