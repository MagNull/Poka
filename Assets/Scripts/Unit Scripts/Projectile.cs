using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int _damage;
    private Rigidbody _rb;
    [SerializeField] private bool _isDestroy;
    [SerializeField] private float _damageRadius;
    [SerializeField] private ParticleSystem _explosionVFX;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != gameObject.tag && _isDestroy)
        {
            other.GetComponent<HealthPoints>()?.TakeDamage(_damage);
            Destroy(gameObject);
        }
        if(other.tag == "Floor" && !_isDestroy)
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
            if(Vector3.Distance(enemys[i].transform.position,transform.position) < _damageRadius)
            {
               enemys[i].GetComponent<HealthPoints>()?.TakeDamage(_damage *  (1 - (Vector3.Distance(enemys[i].transform.position, transform.position) / 
                                                                             _damageRadius)));
            }
        }
        Destroy(_rb);
        _explosionVFX.Play();
        Destroy(gameObject, _explosionVFX.startLifetime);
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
