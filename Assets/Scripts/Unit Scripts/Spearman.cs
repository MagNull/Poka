using UI_Scripts;
using UnityEngine;

namespace Unit_Scripts
{
    public class Spearman : Unit
    {
        [SerializeField] private float backStepSpeed;
        [SerializeField] private float minDistToBackStep;

        protected override void Work()
        {
            FindTarget();
            if (Target != null)
            {
                LookAtEnemy();
                if ((gameObject.transform.position - Target.transform.position).sqrMagnitude <= Mathf.Pow(MINDistToAttack, 2))
                {
                    _targetHP = Target.GetComponent<HealthPoints>();
                    if((gameObject.transform.position - Target.transform.position).sqrMagnitude <= Mathf.Pow(minDistToBackStep, 2))
                    {
                        _rb.velocity = -transform.forward * backStepSpeed;
                    }
                    else
                    {
                        _rb.velocity = Vector3.zero;
                    }
                    if (_animator.GetInteger(_attackAnimationName) == 0)
                    {
                        _animator.SetInteger(_attackAnimationName, UnityEngine.Random.Range(1, numberOfAttackAnimations + 1));
                        _animator.speed = 2 / attackDelay;
                    }
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
                _rb.velocity = Vector3.zero;
                _animator.SetInteger(_attackAnimationName, 0);
                if (UnitSide == UnitSide.PLAYER)
                {
                    _uiMethods.Win();
                }
            }
        }
    }
}
