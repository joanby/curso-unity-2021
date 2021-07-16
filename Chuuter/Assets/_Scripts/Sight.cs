using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public float distance;
    public float angle;
    
    public LayerMask targetLayers;
    public LayerMask obstacleLayers;

    public Collider detectedTarget;
    
    private void Update()
    {
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance, targetLayers);

        detectedTarget = null;
        
        //Hemos pasado el primer filtro: la distancia
        foreach (Collider collider in colliders)
        {
            Vector3 directionToCollider = collider.bounds.center - transform.position; //NO NORM
            directionToCollider = Vector3.Normalize(directionToCollider); //NORM

            //Ángulo que forman el vector visión con el vector objetivo
            float angleToCollider = Vector3.Angle(transform.forward, directionToCollider);
            //cos(angle) = u.v / ||u||.||v||

            //Si el ángulo es menor que el de visión
            if (angleToCollider < angle)
            {
                //Comprobamos que en la línea de visión enemigo -> objetivo no haya obstáculos
                if (!Physics.Linecast(transform.position, collider.bounds.center, obstacleLayers))
                {
                    //Guardamos la referencia del objetivo detectado
                    detectedTarget = collider;
                    break;
                }
            }
        }

        /*for (int i = 0; //INICIALIZACIÓN
            i < colliders.Length; //CONTINUACIÓN
            i++) //ACTUALIZACIÓN
        {
            Collider collider = colliders[i];
            //TODO: hacer cosas al collider
        }*/
        
    }
}
