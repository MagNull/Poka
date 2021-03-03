using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unit_Scripts
{
    public class UnitBypasser : MonoBehaviour
    {
        private Unit _unit;
        private int _bypassDirection;
        [SerializeField] private float _bypassSpeed = 1;

        private void Awake()
        {
            int a = Random.Range(0, 2);
            if (a == 0)
            {
                _bypassDirection = 1;
            }
            else
            {
                _bypassDirection = -1;
            }

            _unit = GetComponent<Unit>();
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
            if (_unit.Target)
            {
                if (collision.gameObject.TryGetComponent(out Unit unit)
                    &&
                    unit.UnitSide == _unit.UnitSide
                    && 
                    (transform.position - _unit.Target.transform.position).sqrMagnitude > Mathf.Pow(_unit.MINDistToAttack,2)
                    &&
                    (collision.transform.position - _unit.Target.transform.position).sqrMagnitude <= Mathf.Pow(_unit.MINDistToAttack,2))
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + transform.right * _bypassSpeed * _bypassDirection, Time.deltaTime);
                }
            }
            
        }
    }
}
