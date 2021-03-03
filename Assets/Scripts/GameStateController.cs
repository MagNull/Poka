using System;
using System.Collections;
using System.Collections.Generic;
using UI_Scripts;
using Unit_Scripts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameStateController : MonoBehaviour
{
    [SerializeField] private Button activeButton;
    private CameraMover _cameraMover;
    private UIMethods _uiMethods;
    private UnitBank _unitBank;
    
    [Header("Grid Fields")]
    private Grid _grid;
    private EnemyGrid _enemyGrid;
    
    private static bool _isGameStart;

    public bool IsGameStart => _isGameStart;
    public Grid Grid => _grid;

    public EnemyGrid EnemyGrid => _enemyGrid;

    [Inject]
    public void Construct(CameraMover cameraMover, UIMethods uiMethods, UnitBank unitBank)
    {
        _cameraMover = cameraMover;
        _uiMethods = uiMethods;
        _unitBank = unitBank;
    }

    private void Start()
    {
        InitializeGrids();
    }
    
    private void InitializeGrids()
    {
        _grid = FindObjectOfType<Grid>();
        _enemyGrid = FindObjectOfType<EnemyGrid>();
    }

    public void SetGameState(bool state) 
    {
        _isGameStart = state;
        _cameraMover.SwapCamera(true);
        Unit[] soldiers = FindObjectsOfType<Unit>();
        foreach (Unit soldier in soldiers)
        {
            soldier.enabled = state;
            soldier.GetComponent<BoxCollider>().isTrigger = !state;
        }
        foreach (Button button in _uiMethods.Buttons)
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
        _enemyGrid.SpawnEnemies();
        _uiMethods.RefreshBalance();
        if(_uiMethods.gameUI.activeSelf == false)
        {
            _uiMethods.gameUI.SetActive(true);
        } 
        _uiMethods.winScreen.SetActive(false);
        Projectile[] proj = FindObjectsOfType<Projectile>();
        for(var i = 0; i < proj.Length; i++)
        {
            Destroy(proj[i].gameObject);
        }
    }
}
