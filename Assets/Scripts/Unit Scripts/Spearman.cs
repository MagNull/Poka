using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearman : Unit
{
    [SerializeField] private float _backStepSpeed;
    [SerializeField] private float _minDistToBackStep;

    protected override void Work()
    {
        FindTarget();
        if (target != null)
        {
            Vector3 lookTarget = (new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget), Time.deltaTime);
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= _minDistToAttack)
            {
                targHP = target.GetComponent<HealthPoints>();
                if(Vector3.Distance(gameObject.transform.position, target.transform.position) <= _minDistToBackStep)
                {
                    _rb.velocity = -transform.forward * _backStepSpeed;
                }
                else
                {
                    _rb.velocity = Vector3.zero;
                }
                if (_animator?.GetInteger("Attack") == 0)
                {
                    _animator?.SetInteger("Attack", UnityEngine.Random.Range(1, numberOfAttackAnimations + 1));
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
            _rb.velocity = Vector3.zero;
            _animator?.SetInteger("Attack", 0);
            if (gameObject.tag == "Player")
            {
                FindObjectOfType<UIMethods>().Win();
            }
        }
    }
}
