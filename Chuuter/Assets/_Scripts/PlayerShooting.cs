using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class PlayerShooting : MonoBehaviour
{
    
    private Animator _animator;

    public int bulletsAmount;

    public Weapon weapon;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Mouse0) && Time.timeScale > 0)
        {
            _animator.SetBool("Shot Bullet Bool", true);
            
            if (bulletsAmount> 0 && weapon.ShootBullet("Player Bullet", 0.25f))
            {
                bulletsAmount--;
                if (bulletsAmount<0)
                {
                    bulletsAmount = 0;
                }
            } else
            {
                //TODO: aqui no tengo balas, buscar sonido acorde a ello
            }
        }
        else
        {
            _animator.SetBool("Shot Bullet Bool", false);
        }
    }

}
