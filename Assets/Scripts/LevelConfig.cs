using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private string[] enemys = new string[10];
    public int LevelMoneyBalance;
    public bool Warrior;
    public bool Archer;
    public bool Shielder;
    public bool Spearman;
    public bool Catapult;

    public string GetUnit(int x,int y)
    {
        if(x < enemys.Length && y < enemys[x].Length)
        {
            return enemys[x][y].ToString();
        }
        return null;
    }
}
