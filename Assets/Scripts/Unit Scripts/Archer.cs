using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    [SerializeField] protected Rigidbody arrowRb;
    [SerializeField] protected float minAngle;
    [SerializeField] protected float maxAngle;
    [SerializeField] protected Transform spawnTransform;
    [SerializeField] protected float meleeAttackDist;
    [SerializeField] protected int meleeDamage;
    [SerializeField] protected GameObject bow;
    [SerializeField] protected GameObject knife;
    protected float _shootAngle = 45;

    protected override void Awake()
    {
        base.Awake();
        _shootAngle = UnityEngine.Random.Range(minAngle, maxAngle);
        FindTarget();
    }
    protected override void Update()
    {
        spawnTransform.localEulerAngles = new Vector3(-_shootAngle, 0, 0);
        base.Update();
    }
    protected override void Work()
    {
        FindTarget();
        if (_target != null)
        {
            Vector3 lookTarget = (new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z) - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget), Time.deltaTime);
            if (Vector3.Distance(gameObject.transform.position, _target.transform.position) <= minDistToAttack)
            {
                if(Vector3.Distance(gameObject.transform.position, _target.transform.position) <= meleeAttackDist)
                {
                    if(knife.activeInHierarchy == false)
                    {
                        knife.SetActive(true);
                        bow.SetActive(false);
                    }
                    _targHP = _target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    _animator?.SetInteger("Attack", 2);
                    _animator.speed = 2 / attackDelay;
                }
                else
                {
                    if (bow.activeInHierarchy == false)
                    {
                        knife.SetActive(false);
                        bow.SetActive(true);
                    }
                    _targHP = _target.GetComponent<HealthPoints>();
                    _rb.velocity = Vector3.zero;
                    if (_animator?.GetInteger("Attack") == 0)
                    {
                        _animator?.SetInteger("Attack", UnityEngine.Random.Range(1, numberOfAttackAnimations + 1));
                        _animator.speed = 2 / attackDelay;
                    }
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
            _animator?.SetInteger("Attack", 0);
            if (gameObject.tag == "Player")
            {
                FindObjectOfType<UIMethods>().Win();
            }
            _rb.velocity = Vector3.zero;
        }
    }
    public override void Attack()
    {
        if(bow.activeSelf)
        {
            Rigidbody arrowRb = Instantiate(this.arrowRb, spawnTransform.position, Quaternion.identity);
            float speed = CalculateTrajectory(_targHP);
            arrowRb.velocity = spawnTransform.forward * speed;
        }
        else
        {
            _targHP?.TakeDamage(meleeDamage);
        }
        
    }

    protected virtual float CalculateTrajectory(HealthPoints target)
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
