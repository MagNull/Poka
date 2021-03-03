using System.Collections.Generic;
using UnityEngine;

namespace Unit_Scripts
{
    public class Projectile : MonoBehaviour
    {
        public UnitSide UnitSide;
        [SerializeField] private int damage;
        [SerializeField] private bool isDestroy;
        [SerializeField] private float damageRadius;
        [SerializeField] private ParticleSystem explosionVFX;
        private Rigidbody _rb;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != gameObject.tag && isDestroy)
            {
                other.GetComponent<HealthPoints>()?.TakeDamage(damage);
                Destroy(gameObject);
            }
            if(other.tag == "Floor" && !isDestroy)
            {
                GetComponent<Collider>().isTrigger = false;
                _rb.velocity = Vector3.zero;
                CreateExplosion(other.GetComponent<HealthPoints>());
            }
        }

        private void CreateExplosion(HealthPoints hp)
        {
            List<HealthPoints> enemies = FindEnemys();
            for(var i = 0; i < enemies.Count; i++)
            {
                if(Vector3.Distance(enemies[i].transform.position,transform.position) < damageRadius)
                {
                    enemies[i].TakeDamage(damage *  (1 - (Vector3.Distance(enemies[i].transform.position, transform.position) / 
                        damageRadius)));
                }
            }
            Destroy(_rb);
            explosionVFX.Play();
            Destroy(gameObject, explosionVFX.startLifetime);
        }

        private List<HealthPoints> FindEnemys()
        {
            Unit[] units = FindObjectsOfType<Unit>();
            List<HealthPoints> _enemies = new List<HealthPoints>();
            for (var i = 0; i < units.Length; i++)
            {
                if (units[i].UnitSide != UnitSide)
                {
                    _enemies.Add(units[i].GetComponent<HealthPoints>());
                }
            }
            return _enemies;
        }

        private void Update()
        {
            if(_rb)
            {
                transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized);
            }       
        }
    }
}
