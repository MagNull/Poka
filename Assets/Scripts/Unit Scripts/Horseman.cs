using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horseman : Unit
{
    /*[SerializeField] private float force;

    protected override void Work()
    {
        FindArcher();
        if (target != null)
        {
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= _minDistToAttack)
            {
                HealthPoints targHP = target.GetComponent<HealthPoints>();
                _rb.velocity = Vector3.zero;
                if (coroutine == null)
                {
                    coroutine = StartCoroutine(Attack(targHP));
                }
            }
            else
            {
                Vector3 movementDirection = (target.transform.position - transform.position).normalized;
                _rb.velocity = movementDirection * _speed;
            }
        }
        else
        {
            base.Work();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != gameObject.tag && collision.gameObject.GetComponent<Rigidbody>())
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(collision.transform.right * force);
            print(collision.gameObject.name);
        }
    }*/
}
