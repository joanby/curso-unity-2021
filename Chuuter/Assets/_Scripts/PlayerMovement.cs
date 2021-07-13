using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    //F = m*a
    [Tooltip("Fuerza de Movimiento del Personaje en N/s")]
    [Range(0, 1000)]
    public float speed;

    [Tooltip("Fuerza de Rotación del Personaje en N/seg")]
    [Range(0, 360)]
    public float rotationSpeed;


    private Rigidbody rb;
    
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        // incr S = V*incr t
        float space = speed * Time.deltaTime;

        float horizontal = Input.GetAxis("Horizontal"); // -1 a 1
        float vertical = Input.GetAxis("Vertical");     // -1 a 1

        Vector3 dir = new Vector3(horizontal, 0, vertical);
        //transform.Translate(dir.normalized*space);
        //FUERZA DE TRANSLACIÓN
        rb.AddRelativeForce(dir.normalized*space);
        
        
        float angle = rotationSpeed * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X"); // -1 a 1
        //transform.Rotate(0,mouseX*angle,0);
        //FUERZA DE ROTACIÓN <-> TORQUE
        rb.AddRelativeTorque(0,mouseX*angle,0);
        
        
        /*
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(0,0,space);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(0,0,-space);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(-space,0,0);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(space,0,0);
        }
        */
    }

}
