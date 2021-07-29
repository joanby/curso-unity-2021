using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public GameObject healthBar;
    
    public Color BarColor(float finalScale)
    {
        
        if (finalScale < 0.15f)
        {
            return new Color(193f/255, 45f/255, 45f/255);
        }else if (finalScale < 0.5f)
        {
            return new Color(211f/255, 211f/255, 29f/255);
        }
        else
        {
            return new Color(98f/255, 178f/255, 61f/255);
        }
    
    }
    
    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1.0f);
        healthBar.GetComponent<Image>().color = BarColor(normalizedValue);

    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        var seq = DOTween.Sequence();
        seq.Append(healthBar.transform.DOScaleX(normalizedValue, 1f));
        seq.Join(healthBar.GetComponent<Image>().DOColor(BarColor(normalizedValue), 1f));
        yield return seq.WaitForCompletion();
    }
}
