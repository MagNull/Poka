using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : Unit
{
    [SerializeField] private float _shieldAttackDist;
    public override void Attack()
    {
        base.Attack();
        if(target != null)
        {
            Vector3 newTargetPos = target.transform.position + transform.forward * _shieldAttackDist;
            StartCoroutine(ShieldBash(newTargetPos));
        }
    }

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
                if (_animator?.GetInteger("Attack") == 0)
                {
                    _animator?.SetInteger("Attack", UnityEngine.Random.Range(1, numberOfAttackAnimations + 1));
                    _animator.speed = 3 / attackDelay;
                }
            }
            else
            {
                _animator?.SetInteger("Attack", 0);
                _rb.velocity = movementDirection * _speed;
            }
        }
        else
        {
            if (gameObject.tag == "Player")
            {
                FindObjectOfType<GameManger>().Win();
            }
            _animator?.SetInteger("Attack", 0);
            _rb.velocity = Vector3.zero;
        }
    }
    private IEnumerator ShieldBash(Vector3 newPos)
    {
        if(target)
        {
            Transform targ = target.transform;
            if (targ == null)
            {
                yield return null;
            }
            Rigidbody targRb = target.GetComponent<Rigidbody>();
            Vector3 firstPos = targ.position;
            BoxCollider targetCol = target.GetComponent<BoxCollider>();
            targetCol.enabled = false;
            while (targ && (targ.position - firstPos).magnitude < (newPos - firstPos).magnitude)
            {
                if (target.transform)
                {
                    if (targRb == null)
                    {
                        break;
                    }
                    targRb.velocity = (newPos - firstPos).normalized * _shieldAttackDist;
                    yield return null;
                }
                else
                {
                    break;
                }
            }
            targetCol.enabled = true;
        }
        
    }
}
