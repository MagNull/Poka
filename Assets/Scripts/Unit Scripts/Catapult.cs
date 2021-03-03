using UI_Scripts;
using UnityEngine;

namespace Unit_Scripts
{
    public class Catapult : Archer
    {
        protected override void Work()
        {
            FindTarget();
            if (Target != null)
            {
                LookAtEnemy();
                if ((gameObject.transform.position - Target.transform.position).sqrMagnitude <= Mathf.Pow(MINDistToAttack, 2))
                {
                    if ((gameObject.transform.position - Target.transform.position).sqrMagnitude <= Mathf.Pow(meleeAttackDist, 2))
                    {
                        _animator.SetInteger(_attackAnimationName, 2);
                    }
                    else
                    {
                        _animator.SetInteger(_attackAnimationName, 1);
                    }
                    _targetHP = Target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    _animator.speed = 2 / attackDelay;
                }
                else
                {
                    _animator.SetInteger(_attackAnimationName, 0);
                    Vector3 movementDirection = (Target.transform.position - transform.position).normalized;
                    _rb.velocity = movementDirection * speed;
                }
            }
            else
            {
                if (UnitSide == UnitSide.PLAYER)
                {
                    _uiMethods.Win();
                }
                _rb.velocity = Vector3.zero;
                _animator.SetInteger(_attackAnimationName, 0);
            }
        }
        protected override void Attack()
        {
            if (_animator.GetInteger(_attackAnimationName) == 1)
            {
                Rigidbody arrowRb = Instantiate(base.arrowRb, arrowSpawnTransform.position, Quaternion.identity);
                arrowRb.GetComponent<Projectile>().UnitSide = UnitSide;
                float speed = CalculateTrajectory(_targetHP);
                arrowRb.velocity = arrowSpawnTransform.forward * speed;
            }
            else if(_animator.GetInteger(_attackAnimationName) == 2)
            {
                _targetHP.TakeDamage(meleeDamage);
            }
        }
    }
}
