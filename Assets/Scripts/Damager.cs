using System;
using System.Collections;
using System.Collections.Generic;
using Unit_Scripts;
using UnityEngine;

public class Damager : MonoBehaviour
{
    protected float damage;
    private Unit _unit;

    protected virtual void Awake()
    {
        _unit = GetComponentInParent<Unit>();
        damage = _unit.Damage;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HealthPoints healthPoints) 
            && other.GetComponent<Unit>().UnitSide != _unit.UnitSide)
        {
            healthPoints.TakeDamage(damage);
        }
    }
}
