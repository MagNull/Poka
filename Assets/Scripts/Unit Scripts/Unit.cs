using System;
using System.Collections.Generic;
using System.Linq;
using UI_Scripts;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Unit_Scripts
{
    [RequireComponent(typeof(HealthPoints))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public abstract class Unit : MonoBehaviour
    {
        public int UnitPrice;
        public int Damage = 1;
        public UnitSide UnitSide;
        [SerializeField] protected float minDistToAttack = 5;
        [SerializeField] protected float speed;
        [SerializeField] protected float attackDelay;
        [SerializeField] protected int numberOfAttackAnimations;
        protected Animator _animator;
        protected Rigidbody _rb;
        private Unit _target;
        protected HealthPoints _targetHP;
        [SerializeField] private List<Unit> _enemies = new List<Unit>();
        protected string _attackAnimationName = "Attack";
        protected UIMethods _uiMethods;

        public Unit Target => _target;

        public float MINDistToAttack => minDistToAttack;

        [Inject]
        public void Construct(UIMethods uiMethods, UnitBank unitBank)
        {
            _uiMethods = uiMethods;
        }
        protected virtual void Awake() //TODO: Add UniRX features.
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            InitializeEnemy();
            FindTarget();
        }

        private void InitializeEnemy()
        {
            Unit[] units = FindObjectsOfType<Unit>();
            for (var i = 0; i < units.Length; i++)
            {
                if (units[i].tag != gameObject.tag)
                {
                    _enemies.Add(units[i]);
                }
            }
        }

        public void FindTarget()
        {
            Unit nearest = null;
            foreach(Unit enemy in _enemies)
            {
                if (enemy != null)
                {
                    if (nearest != null)
                        nearest = Vector3.Distance(gameObject.transform.position, nearest.transform.position) >
                                  Vector3.Distance(gameObject.transform.position, enemy.transform.position) ? enemy : nearest;
                    else
                        nearest = enemy;
                }
            }
            _target = nearest;
        }

        protected virtual void Work()
        {
            FindTarget();
            if(_target != null)
            {
                LookAtEnemy();
                if ((gameObject.transform.position - Target.transform.position).sqrMagnitude <= Mathf.Pow(MINDistToAttack, 2))
                {
                    _targetHP = _target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    if(_animator.GetInteger(_attackAnimationName) == 0) Attack();
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

        protected void LookAtEnemy()
        {
            Vector3 lookTarget =
                (new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z) -
                 transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget), Time.deltaTime);
        }

        protected virtual void Attack()
        {
            _animator.SetInteger(_attackAnimationName, Random.Range(1, numberOfAttackAnimations + 1));
        }

        protected virtual void Update()
        {
            Work();
        }
        
    }
}