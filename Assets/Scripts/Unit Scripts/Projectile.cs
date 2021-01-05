using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool isDestroy;
    [SerializeField] private float damageRadius;
    [SerializeField] private ParticleSystem explosionVFX;
    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != gameObject.tag && isDestroy)
        {
            other.GetComponent<HealthPoints>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        if(other.tag == "Floor" && !isDestroy)
        {
            GetComponent<Collider>().isTrigger = false;
            _rb.velocity = Vector3.zero;
            CreateExplosion(other.GetComponent<HealthPoints>());
        }
    }

    private void CreateExplosion(HealthPoints hp)
    {
        List<Unit> enemys = FindEnemys();
        for(var i = 0; i < enemys.Count; i++)
        {
            if(Vector3.Distance(enemys[i].transform.position,transform.position) < damageRadius)
            {
               enemys[i].GetComponent<HealthPoints>()?.TakeDamage(damage *  (1 - (Vector3.Distance(enemys[i].transform.position, transform.position) / 
                                                                             damageRadius)));
            }
        }
        Destroy(_rb);
        explosionVFX.Play();
        Destroy(gameObject, explosionVFX.startLifetime);
    }

    private List<Unit> FindEnemys()
    {
        Unit[] units = FindObjectsOfType<Unit>();
        List<Unit> _enemys = new List<Unit>();
        for (var i = 0; i < units.Length; i++)
        {
            if (units[i].tag != gameObject.tag)
            {
                _enemys.Add(units[i]);
            }
        }
        return _enemys;
    }

    private void Update()
    {
        if(_rb)
        {
            transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized);
        }       
    }
}
