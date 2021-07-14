using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Cantidad de puntos que se obtienen al derrotar al enemigo")]
    public int pointsAmount = 10;

    private void Awake()
    {
        var life = GetComponent<Life>();
        life.onDeath.AddListener(DestroyEnemy);
    }


    private void Start()
    {
        EnemyManager.SharedInstance.AddEnemy(this);
    }


    private void DestroyEnemy()
    {
        
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Play Die");
        
        Invoke("PlayDestruction", 0.2f);
        
        var life = GetComponent<Life>();
        life.onDeath.RemoveListener(DestroyEnemy);
        
        Destroy(gameObject, 1);

        EnemyManager.SharedInstance.RemoveEnemy(this);
        ScoreManager.SharedInstance.Amount += pointsAmount;
    }

    void PlayDestruction()
    {
        ParticleSystem explosion = GetComponentInChildren<ParticleSystem>();
        explosion.Play();
    }
}
