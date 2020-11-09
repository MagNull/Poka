using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private Unit[,] grid;
    [SerializeField] private LevelConfig _levelConfig;

    [Header("Units Prefabs")]
    [SerializeField] private Unit warrior;
    [SerializeField] private Unit archer;
    [SerializeField] private Unit shielder;
    [SerializeField] private Unit spearman;
    [SerializeField] private Unit catapult;

    private void Awake()
    {
        grid = new Unit[_gridSize.x, _gridSize.y];
    }

    public void SpawnEnemys()
    {
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                switch (_levelConfig.GetUnit(x, y))
                {
                    case "0":
                        {
                            if(grid[x,y] != null)
                            {
                                Destroy(grid[x, y].gameObject);
                                grid[x, y] = null;
                            }
                            break;
                        }
                    case "1":
                        {
                            if (grid[x, y] == null)
                            {
                                grid[x, y] = Instantiate(warrior, new Vector3(x + transform.position.x,
                                                                                     warrior.transform.position.y, y + transform.position.z), warrior.transform.rotation);
                            }

                            break;
                        }
                    case "2":
                        {
                            if (grid[x, y] == null)
                            {
                                grid[x, y] = Instantiate(archer, new Vector3(x + transform.position.x,
                                                                                     archer.transform.position.y, y + transform.position.z), archer.transform.rotation);
                            }

                            break;
                        }
                    case "3":
                        {
                            if (grid[x, y] == null)
                            {
                                grid[x, y] = Instantiate(spearman, new Vector3(x + transform.position.x,
                                                                                     spearman.transform.position.y, y + transform.position.z), spearman.transform.rotation);
                            }

                            break;
                        }
                    case "4":
                        {
                            if (grid[x, y] == null)
                            {
                                grid[x, y] = Instantiate(shielder, new Vector3(x + transform.position.x,
                                                                                     shielder.transform.position.y, y + transform.position.z), shielder.transform.rotation);
                            }

                            break;
                        }
                    case "5":
                        {
                            if (grid[x, y] == null)
                            {
                                grid[x, y] = Instantiate(catapult, new Vector3(x + transform.position.x,
                                                                                     catapult.transform.position.y, y + transform.position.z), catapult.transform.rotation);
                            }

                            break;
                        }
                }
                    

                    


            }
        }
    }

    public void SetLevelConfig(LevelConfig config)
    {
        _levelConfig = config;
        SpawnEnemys();
    }

    public void ClearUnits()
    {
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                if(grid[x,y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                Gizmos.DrawCube(new Vector3(x + transform.position.x,
                                           transform.position.y, y + transform.position.z), new Vector3(.8f, .8f, .8f));
            }
        }
    }
}
