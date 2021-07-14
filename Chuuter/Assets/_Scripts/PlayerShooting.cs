using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerShooting : MonoBehaviour
{
    public GameObject shootingPoint;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _animator.SetTrigger("Shot Bullet");
            
            Invoke("FireBullet", 0.25f);
        }
    }

    void FireBullet()
    {
        GameObject bullet = ObjectPool.SharedInstance.GetFirstPooledObject();
        bullet.layer = LayerMask.NameToLayer("Player Bullet");
        bullet.transform.position = shootingPoint.transform.position;
        bullet.transform.rotation = shootingPoint.transform.rotation;
        bullet.SetActive(true);
    }
    
}
