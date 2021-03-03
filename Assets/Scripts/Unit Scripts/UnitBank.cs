using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace Unit_Scripts
{
    public class UnitBank : MonoBehaviour
    {
        public ReactiveCollection<Unit> PlayerUnitList = new ReactiveCollection<Unit>();
        public ReactiveCollection<Unit> EnemyUnitList = new ReactiveCollection<Unit>();

        public void AddSubscriberToPlayer(UnityAction action)
        {
            PlayerUnitList.ObserveAdd().Subscribe(_ =>
            {
                action.Invoke();
            });
            PlayerUnitList.ObserveRemove().Subscribe(_ =>
            {
                action.Invoke();
            });
        }
    
        public void AddSubscriberToEnemy(UnityAction action)
        {
            EnemyUnitList.ObserveAdd().Subscribe(_ =>
            {
                action.Invoke();
            });
            EnemyUnitList.ObserveRemove().Subscribe(_ =>
            {
                action.Invoke();
            });
        }

        public void ClearAllLists()
        {
            EnemyUnitList.Clear();
            PlayerUnitList.Clear();
        }
    }
}
