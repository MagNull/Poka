using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthPoints))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Unit : MonoBehaviour
{
    protected List<Unit> _enemys = new List<Unit>();
    [SerializeField] protected float _minDistToAttack = 5;
    [SerializeField] protected float _speed;
    [SerializeField] protected int _damage = 1;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float minAttackDelay;
    [SerializeField] protected float maxAttackDelay;
    protected Animator _animator;
    protected Coroutine coroutine;
    protected Rigidbody _rb;
    protected Unit target;
    protected bool isAttack = true;
    protected HealthPoints targHP;
    [SerializeField] protected int numberOfAttackAnimations;
    public int UnitPrice;
    [SerializeField] private float _offsetSpeed = 1;
    private int _offsetDirection;

    protected virtual void Awake()
    {
        InitializeEnemy();
        attackDelay = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);
        _rb = GetComponent<Rigidbody>();
        if(GetComponent<Animator>())
        {
            _animator = GetComponent<Animator>();
        }
        int a = UnityEngine.Random.Range(0, 2);
        if (a == 0)
        {
            _offsetDirection = 1;
        }
        else
        {
            _offsetDirection = -1;
        }
        FindTarget();
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
                _enemys.Add(units[i]);
            }
        }
    }

    public void FindTarget()
    {
        Unit nearest = null;
        foreach(Unit enemy in _enemys)
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
        target = nearest;
    }

    public void FindArcher()
    {
        Unit nearest = null;
        foreach (Unit enemy in _enemys)
        {
            if (enemy != null && enemy.GetComponent<Archer>() != null)
            {
                if (nearest != null)
                    nearest = Vector3.Distance(gameObject.transform.position, nearest.transform.position) >
                              Vector3.Distance(gameObject.transform.position, enemy.transform.position) ? enemy : nearest;
                else
                {
                    nearest = enemy;
                }    
            }
        }
        target = nearest;
    }

    protected virtual void Work()
    {
        FindTarget();
        if(target != null)
        {
            Vector3 lookTarget = (new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget), Time.deltaTime);
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= _minDistToAttack)
            {
                targHP = target.GetComponent<HealthPoints>();
                _rb.velocity = Vector3.zero;
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
            if (gameObject.tag == "Player")
            {
                FindObjectOfType<GameManger>().Win();
            }
            _rb.velocity = Vector3.zero;
            _animator?.SetInteger("Attack", 0);
        }
        
    }

    public virtual void Attack()
    {
        targHP?.TakeDamage(_damage,ref isAttack);
        _animator?.SetInteger("Attack", UnityEngine.Random.Range(1, numberOfAttackAnimations + 1));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Offensive(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        Offensive(collision);
    }

    private void Offensive(Collision collision)
    {
        if (target != null)
        {
            if (collision.transform.tag == gameObject.tag && Vector3.Distance(transform.position, target.transform.position) > _minDistToAttack
                    &&
                Vector3.Distance(collision.transform.position, target.transform.position) <= _minDistToAttack)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.right * _offsetSpeed * _offsetDirection, Time.deltaTime);
            }
        }
    }

    protected virtual void Update()
    {
        Work();
    }
}
