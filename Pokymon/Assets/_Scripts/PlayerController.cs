using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private bool isMoving;
    
    public float speed;
    private Vector2 input;

    private Animator _animator;

    public LayerMask solidObjectsLayer;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            /*input = new Vector2(Input.GetAxisRaw("Horizontal"), 
                Input.GetAxisRaw("Vertical"));*/
            
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }
            
            if (input != Vector2.zero)
            {
                _animator.SetFloat("Move X", input.x);
                _animator.SetFloat("Move Y", input.y);
                
                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (IsAvailable(targetPosition))
                {
                    StartCoroutine(MoveTowards(targetPosition));
                }
                
            }
        }
    }


    private void LateUpdate()
    {
        _animator.SetBool("Is Moving", isMoving);
    }

    IEnumerator MoveTowards(Vector3 destination)
    {
        isMoving = true;
        
        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                destination, speed * Time.deltaTime);
            yield return null;
        }
        
        transform.position = destination;
        isMoving = false;
        
    }

/// <summary>
/// El método comprueba que la zona a la que queremos acceder, esté disponible
/// </summary>
/// <param name="target">Zona a la que queremos acceder</param>
/// <returns>Devuelve true, si el target está disponible y false  en caso contrario</returns>
    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.2f, solidObjectsLayer)!=null)
        {
            return false;
        }

        return true;
    }

}
