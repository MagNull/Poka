using UI_Scripts;
using UnityEngine;

namespace Unit_Scripts
{
    public class Archer : Unit
    {
        [SerializeField] protected Rigidbody arrowRb;
        [Header("Attack Parameters")]
        [SerializeField] protected float minAngle;
        [SerializeField] protected float maxAngle;
        [SerializeField] protected float meleeAttackDist;
        [SerializeField] protected int meleeDamage;
        
        [SerializeField] protected Transform arrowSpawnTransform;

        [Header("Weapons")]
        [SerializeField] protected GameObject bow;
        [SerializeField] protected GameObject knife;
        
        private float _shootAngle = 45;

        protected override void Awake()
        {
            base.Awake();
            _shootAngle = Random.Range(minAngle, maxAngle);
        }
        protected override void Update()
        {
            arrowSpawnTransform.localEulerAngles = new Vector3(-_shootAngle, 0, 0);
            base.Update();
        }
        protected override void Work()
        {
            FindTarget();
            if (Target != null)
            {
                LookAtEnemy();
                if ((gameObject.transform.position - Target.transform.position).sqrMagnitude <= Mathf.Pow(MINDistToAttack, 2))
                {
                    if((gameObject.transform.position - Target.transform.position).sqrMagnitude <= Mathf.Pow(meleeAttackDist, 2))
                    {
                        if(knife.activeInHierarchy == false)
                        {
                            knife.SetActive(true);
                            bow.SetActive(false);
                        }
                        _animator.SetInteger(_attackAnimationName, 2);
                        _animator.speed = 2 / attackDelay;
                    }
                    else
                    {
                        if (bow.activeInHierarchy == false)
                        {
                            knife.SetActive(false);
                            bow.SetActive(true);
                        }
                        if (_animator.GetInteger(_attackAnimationName) == 0)
                        {
                            _animator.SetInteger(_attackAnimationName, UnityEngine.Random.Range(1, numberOfAttackAnimations + 1));
                            _animator.speed = 2 / attackDelay;
                        }
                    }      
                    _targetHP = Target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
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
                _animator.SetInteger(_attackAnimationName, 0);
                if (UnitSide == UnitSide.PLAYER)
                {
                    _uiMethods.Win();
                }
                _rb.velocity = Vector3.zero;
            }
        }
        protected override void Attack()
        {
            if(bow.activeSelf)
            {
                Rigidbody arrowRb = Instantiate(this.arrowRb, arrowSpawnTransform.position, Quaternion.identity);
                arrowRb.GetComponent<Projectile>().UnitSide = UnitSide;
                float speed = CalculateTrajectory(_targetHP);
                arrowRb.velocity = arrowSpawnTransform.forward * speed;
            }
            else
            {
                _targetHP.TakeDamage(meleeDamage);
            }
        
        }

        protected float CalculateTrajectory(HealthPoints target)
        {
            if(target != null)
            {
                Vector3 fromTo = target.transform.position - transform.position;
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
        
        }
    }
}
