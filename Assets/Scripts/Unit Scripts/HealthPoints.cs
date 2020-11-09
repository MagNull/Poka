using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour
{
    [SerializeField] private float _health = 5;
    [SerializeField] private ParticleSystem _blood;

    public void TakeDamage(float damage,ref bool flag)
    {
        try
        {
            if (gameObject)
            {
                if (GetComponent<Unit>() && GetComponent<Unit>().enabled == true)
                {
                    _health -= damage;
                    if (_health <= 0)
                    {
                        flag = false;
                        GetComponent<Animator>().SetInteger("Attack", -1);
                        Destroy(GetComponent<Unit>());
                        GetComponent<Rigidbody>().isKinematic = true;
                        Destroy(gameObject, 1);
                    }
                    _blood.Play();
                }
            }
        }
        catch
        {

        }
        
        
        
    }
    public void TakeDamage(float damage)
    {
        try
        {
            if (GetComponent<Unit>() && GetComponent<Unit>().enabled == true)
            {
                _health -= damage;
                if (_health <= 0)
                {
                    GetComponent<Animator>().SetInteger("Attack", -1);
                    Destroy(GetComponent<Unit>());
                    Destroy(GetComponent<Rigidbody>());
                    Destroy(gameObject, 1);
                }
                else
                {
                    _blood.Play();
                }
            }
        }
        
        catch
        {

        }

    }

    private void Die()
    {
        try
        {
            Destroy(gameObject);
        }
        catch
        {

        }
    }
}
