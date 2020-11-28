using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateController : MonoBehaviour
{
    [SerializeField] private Button activeButton;
    [SerializeField] private CameraMover cameraMover;
    [SerializeField] private UIMethods UI;
    
    [Header("Grid Fields")]
    private GameObject[] _cells;
    private Grid _grid;
    private EnemyGrid _enemyGrid;
    
    private static bool _isGameStart;

    public static bool IsGameStart => _isGameStart;
    public Grid Grid => _grid;

    public EnemyGrid EnemyGrid => _enemyGrid;

    private void Start()
    {
        InitializeGrids();
    }
    
    private void InitializeGrids()
    {
        _cells = GameObject.FindGameObjectsWithTag("Cell");
        _grid = FindObjectOfType<Grid>();
        _enemyGrid = FindObjectOfType<EnemyGrid>();
    }

    public void SetGameState(bool state) 
    {
        _isGameStart = state;
        cameraMover.SwapCamera(true);
        Unit[] soldiers = FindObjectsOfType<Unit>();
        foreach (Unit soldier in soldiers)
        {
            soldier.enabled = state;
            soldier.GetComponent<BoxCollider>().isTrigger = !state;
        }
        foreach (Button button in UI.Buttons)
        {
            button.GetComponent<ButtonLighter>()?.ChangeButtonState(!state);
        }
        foreach (MeshRenderer cell in _grid.Cells)
        {
            cell.material.color = new Color(0.1268245f, 0.4716981f, 0.1268245f);
            cell.gameObject.SetActive(!state);
        }
        activeButton.interactable = state;
        _grid.IsPlacementMode = !state;
        if(state)
        {
            if(_grid.FlyingUnit)
            {
                Destroy(_grid.FlyingUnit.gameObject);
                _grid.FlyingUnit = null;
            } 
        }
    }

    
    public void Restart() 
    {
        SetGameState(false);
        _grid.ClearUnits();
        _enemyGrid.ClearUnits();
        _enemyGrid.SpawnEnemys();
        UI.RefreshBalance();
        if(UI.gameUI.activeSelf == false)
        {
            UI.gameUI.SetActive(true);
        } 
        UI.winScreen.SetActive(false);
        Projectile[] proj = FindObjectsOfType<Projectile>();
        for(var i = 0; i < proj.Length; i++)
        {
            Destroy(proj[i].gameObject);
        }
    }
}
