using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autodestroy : MonoBehaviour
{
    [Tooltip("Tiempo después del cual se destruye el objeto")]
    public float destructionDelay;
    // Start is called before the first frame update
    void OnEnable()
    {
        //Destroy(gameObject, destructionDelay);
        Invoke("HideObject", destructionDelay);
    }

    private void HideObject()
    {
        gameObject.SetActive(false);
    }
}
