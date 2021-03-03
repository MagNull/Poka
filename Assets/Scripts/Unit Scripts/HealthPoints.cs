using System;
using UnityEngine;

namespace Unit_Scripts
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Unit))]
    public class HealthPoints : MonoBehaviour
    {
        [SerializeField] private float health = 5;
        [SerializeField] private ParticleSystem blood;
        private Animator _animator;
        private Rigidbody _rigidbody;
        private Unit _unit;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _unit = GetComponent<Unit>();
        }

        public void TakeDamage(float damage,ref bool flag)
        {
            health -= damage;
            if (health <= 0)
            {
                flag = false;
                _animator.SetInteger("Attack", -1);
                _rigidbody.isKinematic = true;
                _unit.enabled = false;
                Destroy(gameObject, 1);
            }
            blood.Play();
        }
        
        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                _animator.SetInteger("Attack", -1);
                _rigidbody.isKinematic = true;
                _unit.enabled = false;
                Destroy(gameObject, 1);
            }
            else
            {
                blood.Play();
            }

        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
