using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    protected float damage;

    protected virtual void Awake()
    {
        damage = GetComponentInParent<Unit>().Damage;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HealthPoints>() && !other.CompareTag(gameObject.tag))
        {
            other.GetComponent<HealthPoints>().TakeDamage(damage);
        }
    }
}
