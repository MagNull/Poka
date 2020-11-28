using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : Unit
{
    public float ShieldAttackDist;
    public bool canAttack = true;
    private float time = 0;

    protected override void Work()
    {
        FindTarget();
        if (target != null)
        {
            Vector3 movementDirection = (target.transform.position - transform.position).normalized;
            Vector3 lookTarget = (new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(lookTarget),Time.deltaTime);
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= _minDistToAttack)
            {
                targHP = target.GetComponent<HealthPoints>();
                _rb.velocity = Vector3.zero;
                if (canAttack)
                {
                    _animator?.SetInteger("Attack", UnityEngine.Random.Range(1, numberOfAttackAnimations + 1));
                }
                else
                {
                    _animator?.SetInteger("Attack",0);
                }
            }
            else
            {
                _rb.velocity = movementDirection * _speed;
            }
        }
        else
        {
            if (gameObject.tag == "Player")
            {
                FindObjectOfType<UIMethods>().Win();
            }
            _animator?.SetInteger("Attack", 0);
            _rb.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (time >= attackDelay)
        {
            time = 0;
            canAttack = true;
        }
        else if(!canAttack)
        {
            time += Time.deltaTime;
        }
    }
}
