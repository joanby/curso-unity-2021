using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    public float damage;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        //Destroy(gameObject); PROHIBIDO, OBJECT POOLING ES MEJOR
        gameObject.SetActive(false);//Desactiva la bala
        
        /*if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            Destroy(other.gameObject); //Destruye el otro objeto (solo player o enemigo)
        }*/

        Life life = other.GetComponent<Life>();

        if (life != null)
        {
            life.Amount -= damage; // life.amount = life.amount - damage;
        }
    }
}
