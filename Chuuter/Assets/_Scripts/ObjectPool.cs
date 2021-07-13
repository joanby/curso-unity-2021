using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance; 

    public GameObject prefab;

    public List<GameObject> pooledObjects;

    public int amountToPool;

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            Debug.LogError("YA HAY OTRO POOL EN PANTALLA");
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(prefab);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetFirstPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }
    
}
