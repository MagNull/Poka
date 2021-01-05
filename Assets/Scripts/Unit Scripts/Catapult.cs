using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : Archer
{
    protected override void Work()
    {
        FindTarget();
        if (_target != null)
        {
            Vector3 lookTarget = (new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z) - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget), Time.deltaTime);
            if (Vector3.Distance(gameObject.transform.position, _target.transform.position) <= minDistToAttack)
            {
                if (Vector3.Distance(gameObject.transform.position, _target.transform.position) <= meleeAttackDist)
                {
                    _animator.SetInteger("Attack", 2);
                    _targHP = _target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    _animator.speed = 5 / attackDelay;
                }
                else
                {
                    _animator.SetInteger("Attack", 1);
                    _targHP = _target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    _animator.speed = 2 / attackDelay;
                }
            }
            else
            {
                _animator?.SetInteger("Attack", 0);
                Vector3 movementDirection = (_target.transform.position - transform.position).normalized;
                _rb.velocity = movementDirection * speed;
            }
        }
        else
        {
            if (gameObject.tag == "Player")
            {
                FindObjectOfType<UIMethods>().Win();
            }
            _rb.velocity = Vector3.zero;
            _animator?.SetInteger("Attack", 0);
        }
    }
    public override void Attack()
    {
        if (_animator.GetInteger("Attack") == 1)
        {
            Rigidbody arrowRb = Instantiate(base.arrowRb, spawnTransform.position, Quaternion.identity);
            float speed = CalculateTrajectory(_targHP);
            arrowRb.velocity = spawnTransform.forward * speed;
        }
        else if(_animator.GetInteger("Attack") == 2)
        {
            _targHP?.TakeDamage(meleeDamage);
        }
    }
}
