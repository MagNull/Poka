using UI_Scripts;
using UnityEngine;

namespace Unit_Scripts
{
    public class Shielder : Unit
    {
        public float ShieldAttackDist;
        public bool canAttack = true;
        private float _time = 0;

        protected override void Work()
        {
            FindTarget();
            if (Target != null)
            {
                Vector3 movementDirection = (Target.transform.position - transform.position).normalized;
                LookAtEnemy();
                if ((gameObject.transform.position - Target.transform.position).sqrMagnitude <= Mathf.Pow(MINDistToAttack, 2))
                {
                    _targetHP = Target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    if (canAttack)
                    {
                        _animator?.SetInteger(_attackAnimationName, Random.Range(1, numberOfAttackAnimations + 1));
                    }
                    else
                    {
                        _animator?.SetInteger(_attackAnimationName,0);
                    }
                }
                else
                {
                    _rb.velocity = movementDirection * speed;
                }
            }
            else
            {
                if (UnitSide == UnitSide.PLAYER)
                {
                    _uiMethods.Win();
                }
                _animator.SetInteger(_attackAnimationName, 0);
                _rb.velocity = Vector3.zero;
            }
        }

        private void FixedUpdate()
        {
            if (_time >= attackDelay)
            {
                _time = 0;
                canAttack = true;
            }
            else if(!canAttack)
            {
                _time += Time.deltaTime;
            }
        }
    }
}
