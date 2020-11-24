using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Damager
{
    private float bashDist;

    protected override void Awake()
    {
        base.Awake();
        bashDist = GetComponentInParent<Shielder>().ShieldAttackDist;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HealthPoints>() && !other.CompareTag(gameObject.tag) && GetComponentInParent<Shielder>().canAttack)
        {
            Vector3 newTargetPos = other.transform.position - transform.right * bashDist;
            StartCoroutine(ShieldBash(newTargetPos, other.transform));
            other.GetComponent<HealthPoints>().TakeDamage(damage);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<HealthPoints>() && !other.CompareTag(gameObject.tag) && GetComponentInParent<Shielder>().canAttack)
        {
            Vector3 newTargetPos = other.transform.position - transform.right * bashDist;
            StartCoroutine(ShieldBash(newTargetPos, other.transform));
            other.GetComponent<HealthPoints>().TakeDamage(damage);
        }
    }

    private IEnumerator ShieldBash(Vector3 newPos, Transform target)
    {
        if (target)
        {
            Rigidbody targRb = target.GetComponent<Rigidbody>();
            Vector3 firstPos = target.position;
            BoxCollider targetCol = target.GetComponent<BoxCollider>();
            targetCol.enabled = false;
            while (target && (target.position - firstPos).magnitude < (newPos - firstPos).magnitude)
            {
                if (target.transform)
                {
                    if (targRb == null)
                    {
                        break;
                    }

                    targRb.velocity = (newPos - firstPos).normalized * bashDist;
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            targetCol.enabled = true;
            GetComponentInParent<Shielder>().canAttack = false;
        }
    }
}    
