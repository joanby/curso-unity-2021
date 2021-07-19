using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    private float lastShootTime;
    
    public float shootRate;

    public GameObject shootingPoint;

    public ParticleSystem fireEffect;

    public AudioSource shootSound;

    private string layerName;
    
    public bool ShootBullet(string layerName, float delay)
    {
        if (Time.timeScale>0)
        {
            var timeSinceLastShoot = Time.time - lastShootTime;
            if (timeSinceLastShoot < shootRate)
            {
                return false;
            }
            
            lastShootTime = Time.time;
            this.layerName = layerName;
            Invoke("FireBullet", delay);

            return true;
        }

        return false;
    }


    void FireBullet()
    {
        var bullet = ObjectPool.SharedInstance.GetFirstPooledObject();
        bullet.layer = LayerMask.NameToLayer(layerName);
        bullet.transform.position = shootingPoint.transform.position;
        bullet.transform.rotation = shootingPoint.transform.rotation;
        bullet.SetActive(true);
            
        if (fireEffect!=null)
        {
            fireEffect.Play();
        }

        if (shootSound != null)
        {
            Instantiate(shootSound, transform.position, transform.rotation).
                GetComponent<AudioSource>().Play();
        }
        
    }
}
