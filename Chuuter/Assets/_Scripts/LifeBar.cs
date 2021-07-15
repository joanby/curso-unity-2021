using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LifeBar : MonoBehaviour
{
    [Tooltip("Vida objetivo que reflejará la barra")]
    public Life targetLife;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }


    private void Update()
    {
        _image.fillAmount = targetLife.Amount / targetLife.maximumLife;
    }
}
