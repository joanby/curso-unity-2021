using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float speed;

    private CharacterAnimator _animator;

    public bool IsMoving { get; private set; }
    public CharacterAnimator Animator => _animator;
    
    private void Awake()
    {
        _animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator MoveTowards(Vector2 moveVector, Action OnMoveFinish = null)
    {

        if (moveVector.x != 0) moveVector.y = 0;
        
       _animator.MoveX = Mathf.Clamp(moveVector.x, -1, 1);
       _animator.MoveY = Mathf.Clamp(moveVector.y, -1, 1);
            
       var targetPosition = transform.position;
       targetPosition.x += moveVector.x;
       targetPosition.y += moveVector.y;
       
       if (!IsPathAvailable(targetPosition))
       {
           yield break;
       }
       
       IsMoving = true;
       
       while (Vector3.Distance(transform.position, targetPosition) > Mathf.Epsilon)
       {
           transform.position = Vector3.MoveTowards(transform.position,
               targetPosition, speed * Time.deltaTime);
           yield return null;
       }
       
       transform.position = targetPosition;
       IsMoving = false;

      OnMoveFinish?.Invoke();
   
       }

    public void LookTowards(Vector3 target)
    {
        var diff = target - transform.position;
        var xdiff = Mathf.FloorToInt(diff.x);
        var ydiff = Mathf.FloorToInt(diff.y);

        if (xdiff == 0 || ydiff==0)
        {
            _animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            _animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
        {
            Debug.LogError("ERROR: El personaje no puede moverse ni mirar en diagonal...");
        }
    }

    public void HandleUpdate()
    {
        _animator.IsMoving = IsMoving;
    }


    private bool IsPathAvailable(Vector3 target)
    {
        var path = target - transform.position;
        var direction = path.normalized;

        return !Physics2D.BoxCast(transform.position + direction, 
            new Vector2(0.3f, 0.3f), 0f,
            direction, path.magnitude - 1,
            GameLayers.SharedInstance.CollisionLayers);

    }
    
       /// <summary>
       /// El método comprueba que la zona a la que queremos acceder, esté disponible
       /// </summary>
       /// <param name="target">Zona a la que queremos acceder</param>
       /// <returns>Devuelve true, si el target está disponible y false  en caso contrario</returns>
       private bool IsAvailable(Vector3 target)
       {
           if (Physics2D.OverlapCircle(target, 0.2f, 
               GameLayers.SharedInstance.SolidObjectsLayer | 
               GameLayers.SharedInstance.InteractableLayer)!=null)
           {
               return false;
           }

           return true;
       }
}
