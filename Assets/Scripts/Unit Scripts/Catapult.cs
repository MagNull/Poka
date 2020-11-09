using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : Archer
{
    /*protected override float CalculateTrajectory(HealthPoints target)
    {
        if (target != null)
        {
            Vector3 fromTo = target.transform.position - transform.position - transform.forward * 2;
            Vector3 fromToXZ = new Vector3(fromTo.x, 0, fromTo.z);
            transform.rotation = Quaternion.LookRotation(fromToXZ, Vector3.up);
            float x = fromToXZ.magnitude;
            float y = fromTo.y;
            float speed2 = (Physics.gravity.y * x * x) / (2 * (y - Mathf.Tan(_shootAngle * Mathf.Deg2Rad) * x) *
                                 Mathf.Pow(Mathf.Cos(_shootAngle * Mathf.Deg2Rad), 2));
            float speed = Mathf.Sqrt(Mathf.Abs(speed2));
            return speed;
        }
        return 0;
    }*/
    protected override void Work()
    {
        FindTarget();
        if (target != null)
        {
            Vector3 lookTarget = (new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget), Time.deltaTime);
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= _minDistToAttack)
            {
                if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= _meleeAttackDist)
                {
                    _animator.SetInteger("Attack", 2);
                    targHP = target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    _animator.speed = 5 / attackDelay;
                }
                else
                {
                    _animator.SetInteger("Attack", 1);
                    targHP = target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    _animator.speed = 2 / attackDelay;
                }
            }
            else
            {
                _animator?.SetInteger("Attack", 0);
                Vector3 movementDirection = (target.transform.position - transform.position).normalized;
                _rb.velocity = movementDirection * _speed;
            }
        }
        else
        {
            if (gameObject.tag == "Player")
            {
                FindObjectOfType<GameManger>().Win();
            }
            _rb.velocity = Vector3.zero;
            _animator?.SetInteger("Attack", 0);
        }
    }
    public override void Attack()
    {
        if (_animator.GetInteger("Attack") == 1)
        {
            Rigidbody arrowRb = Instantiate(_arrowRb, _spawnTransform.position, Quaternion.identity);
            float speed = CalculateTrajectory(targHP);
            arrowRb.velocity = _spawnTransform.forward * speed;
        }
        else if(_animator.GetInteger("Attack") == 2)
        {
            targHP?.TakeDamage(_meleeDamage);
        }
    }
}
