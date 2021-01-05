using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthPoints))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Unit : MonoBehaviour
{
    public int UnitPrice;
    public int Damage = 1;
    [SerializeField] protected float minDistToAttack = 5;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float minAttackDelay;
    [SerializeField] protected float maxAttackDelay;
    [SerializeField] protected int numberOfAttackAnimations;
    [SerializeField] private float offsetSpeed = 1;
    protected Animator _animator;
    protected Rigidbody _rb;
    protected Unit _target;
    protected HealthPoints _targHP;
    private int _offsetDirection;
    protected List<Unit> _enemys = new List<Unit>();

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
        _target = nearest;
    }

    protected virtual void Work()
    {
        FindTarget();
        if(_target != null)
        {
            Vector3 lookTarget = (new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z) - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget), Time.deltaTime);
            if (Vector3.Distance(gameObject.transform.position, _target.transform.position) <= minDistToAttack)
            {
                _targHP = _target.GetComponent<HealthPoints>();
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
            _animator.SetInteger("Attack", 0);
        }
        
    }

    public virtual void Attack()
    {
        _animator.SetInteger("Attack", UnityEngine.Random.Range(1, numberOfAttackAnimations + 1));
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
        if (_target != null)
        {
            if (collision.transform.tag == gameObject.tag 
                    && 
                Vector3.Distance(transform.position, _target.transform.position) > minDistToAttack
                    &&
                Vector3.Distance(collision.transform.position, _target.transform.position) <= minDistToAttack)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.right * offsetSpeed * _offsetDirection, Time.deltaTime);
            }
        }
    }

    protected virtual void Update()
    {
        Work();
    }
}
