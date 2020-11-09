using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid Params")]
    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private GridUnit[,] _grid;
    [SerializeField] private GameObject _cellPrefab;

    private GridUnit _flyingUnit;
    private GameObject[,] _cells;


    private List<Renderer> _activeCells = new List<Renderer>();

    [Header("Soldiers Prefabs")]
    [SerializeField] private GridUnit warriorPrefab;
    [SerializeField] private GridUnit archerPrefab;
    [SerializeField] private GridUnit shielderPrefab;
    [SerializeField] private GridUnit spearmanPrefab;
    [SerializeField] private GridUnit catapultPrefab;

    [SerializeField] private GridUnit currentUnit;

    private int x;
    private int y;

    private Camera currentCamera;

    private Color origCol;
    private bool available;
    private GameManger _gameManger;
    public bool InTest;

    private bool _isPlacementMode = true;

    public bool IsPlacementMode { get => _isPlacementMode; set => _isPlacementMode = value; }
    public GridUnit FlyingUnit { 
        get => _flyingUnit; 
        
        set
        { 
            _flyingUnit = value;
            if(_flyingUnit)
            {
                _flyingUnit.GetComponentInChildren<BoxCollider>().enabled = false;
            }
        }

        }

    public Camera CurrentCamera { get => currentCamera; set => currentCamera = value; }

    private void Start()
    {
        CurrentCamera = Camera.main;
        _gameManger = FindObjectOfType<GameManger>();
        origCol = _cellPrefab.GetComponent<Renderer>().sharedMaterial.color;
        _grid = new GridUnit[_gridSize.x, _gridSize.y];
        _cells = new GameObject[_gridSize.x, _gridSize.y];
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                _cells[x,y] = Instantiate(_cellPrefab, new Vector3(x + transform.position.x,
                            transform.position.y, y + transform.position.z), Quaternion.identity);
            }
        }
    }
    

    public void StartPlacingUnit()
    {
        if(InTest)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (_flyingUnit != null)
                {
                    Destroy(_flyingUnit.gameObject);
                }
                _flyingUnit = Instantiate(warriorPrefab);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (_flyingUnit != null)
                {
                    Destroy(_flyingUnit.gameObject);
                }
                _flyingUnit = Instantiate(archerPrefab);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (_flyingUnit != null)
                {
                    Destroy(_flyingUnit.gameObject);
                }
                _flyingUnit = Instantiate(shielderPrefab);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (_flyingUnit != null)
                {
                    Destroy(_flyingUnit.gameObject);
                }
                _flyingUnit = Instantiate(spearmanPrefab);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (_flyingUnit != null)
                {
                    Destroy(_flyingUnit.gameObject);
                }
                _flyingUnit = Instantiate(catapultPrefab);
            }
        }
    } //Установка юнитов по клавишам
    private void Update()
    {
        Placement();
       //MoveUnit();
    }

    private void MoveUnit()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(FlyingUnit != null)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    for (int y = 0; y < _gridSize.y; y++)
                    {
                        if (_cells[x, y].GetComponent<Renderer>().material.color != origCol)
                        {
                            _cells[x, y].GetComponent<Renderer>().material.color = origCol;
                        }
                    }
                }
                Destroy(_flyingUnit.gameObject);
                _flyingUnit = null;
            }
        }
        if(Input.GetMouseButtonDown(1) && _flyingUnit == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit hit))
            {
                GridUnit unit = hit.collider.gameObject.GetComponentInParent<GridUnit>();
                if (unit && unit != _flyingUnit)
                {
                    unit.GetComponentInChildren<Unit>().UnitPrice = 0;
                    for (int x = 0; x < _gridSize.x; x++)
                    {
                        for (int y = 0; y < _gridSize.y; y++)
                        { 
                            if(_grid[x,y] == unit)
                            {
                                _grid[x,y] = null;
                                _cells[x,y].GetComponent<Renderer>().material.color = origCol;
                            }
                        }
                    }
                    FlyingUnit = unit;
                }
            }
        }
    }

    private void Placement() // Изменение позиции юнита
    {
        StartPlacingUnit();
        if (_flyingUnit != null)
        {
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            Ray ray = CurrentCamera.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);
                if(CheckAvialable(Mathf.RoundToInt(worldPosition.x) - (int)transform.position.x, 
                                  Mathf.RoundToInt(worldPosition.z) - (int)transform.position.z))
                {
                    x = Mathf.RoundToInt(worldPosition.x) - (int)transform.position.x;
                    y = Mathf.RoundToInt(worldPosition.z) - (int)transform.position.z;
                }
                
                available = CheckAvialable(x, y);

                _flyingUnit.SetTransparent(available);
                ChangeColor(x, y, available,_flyingUnit.Size);
            }

        }
    }

    public void SetCurrentUnit(GridUnit unit) //Установка текущего выбранного юнита
    {
        currentUnit = unit;
        if(_flyingUnit != null)
        {
            Destroy(_flyingUnit.gameObject);
        }
        _flyingUnit = Instantiate(currentUnit);
    }

    private void ChangeColor(int x, int y, bool available, Vector2 size)
    {
        if (available && !IsPlaceTaken(x, y))
        {
            if (_activeCells != null)
            {
                foreach(Renderer cell in _activeCells)
                {
                    cell.material.color = origCol;
                }
            }
            _activeCells.Clear();
            if((size.x * size.y) % 2 != 0)
            {
                for (var x1 = 0; x1 < size.x; x1++)
                {
                    for (var y1 = 0; y1 < size.y; y1++)
                    {
                        if (x - x1 / 2 >= 0 && y - y1 / 2 >= 0 && x + x1 / 2 < _gridSize.x && y + y1 / 2 < _gridSize.y)
                        {
                            if (!_grid[x - x1 / 2, y - y1 / 2])
                            {
                                _activeCells.Add(_cells[x - x1 / 2, y - y1 / 2].GetComponent<MeshRenderer>());
                            }
                            if(!_grid[x + x1 / 2, y - y1 / 2])
                            {
                                _activeCells.Add(_cells[x + x1 / 2, y - y1 / 2].GetComponent<MeshRenderer>());
                            }
                            if(!_grid[x - x1 / 2, y + y1 / 2])
                            {
                                _activeCells.Add(_cells[x - x1 / 2, y + y1 / 2].GetComponent<MeshRenderer>());
                            }
                            if(!_grid[x + x1 / 2, y + y1 / 2])
                            {
                                _activeCells.Add(_cells[x + x1 / 2, y + y1 / 2].GetComponent<MeshRenderer>());
                            }    
                        }
                    }
                } 
            }
            else
            {
                for (var x1 = 0; x1 < size.x; x1++)
                {
                    for (var y1 = 0; y1 < size.y; y1++)
                    {
                        _activeCells.Add(_cells[x + x1, y + y1].GetComponent<MeshRenderer>());
                    }
                }
            }
            foreach (Renderer cell in _activeCells)
            {
                cell.material.color = new Color(Color.yellow.r,Color.yellow.g,Color.yellow.b,.75f);
            }
        }
        else
        {
            if (_activeCells != null)
            {
                foreach (Renderer cell in _activeCells)
                {
                    cell.material.color = origCol;
                }
            }
        }
    }//Изменение цвета клетки

    private bool CheckAvialable(int x, int y)
    {
        bool available = true;
        if(_flyingUnit.Size.x * _flyingUnit.Size.y % 2 != 0)
        {
            if (x < 0 || x + _flyingUnit.Size.x / 2 + 1 > _gridSize.x  || x -_flyingUnit.Size.x / 2  < 0 )
            {
                available = false;
            }
            if (y < 0 || y + _flyingUnit.Size.y / 2 + 1 > _gridSize.y || y - _flyingUnit.Size.y / 2  < 0)
            {
                available = false;
            }
        }
        else
        {
            if (x < 0 || x > _gridSize.x - _flyingUnit.Size.x)
            {
                available = false;
            }
            if (y < 0 || y > _gridSize.y - _flyingUnit.Size.y)
            {
                available = false;
            }
        }
        if (available && IsPlaceTaken(x, y))
        {
            available = false;
        }

        if (!(x < _gridSize.x && x >= 0 && y < _gridSize.y && y >= 0))
        {
            _flyingUnit.transform.position = new Vector3(x - 0.27f + (int)transform.position.x, _flyingUnit.transform.position.y,
                                                         y + (int)transform.position.z);
        }
        else
        {
            if (!IsPlaceTaken(x, y))
            {
                _flyingUnit.transform.position = new Vector3(x - 0.27f + (int)transform.position.x, _flyingUnit.transform.position.y,
                                                             y + (int)transform.position.z);
            }
        }

        return available;
    } //Проверка координат на возможность установки

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        if (_flyingUnit.Size.x * _flyingUnit.Size.y % 2 != 0 && _flyingUnit.Size.x * _flyingUnit.Size.y != 1)
        {
            for (int x1= 0; x1 < _flyingUnit.Size.x; x1++)
            {
                for (int y1 = 0; y1 < _flyingUnit.Size.y; y1++)
                {
                    if (_gridSize.x > (placeX + x1 / 2) && _gridSize.y > (placeY + y1 / 2) && (placeY + y1 / 2) > 0
                        && (placeX + x1 / 2) > 0)
                    {
                        try
                        {
                            if (_grid[placeX + x1 / 2, placeY + y1 / 2] ||
                                                        _grid[placeX - x1 / 2, placeY - y1 / 2] ||
                                                        _grid[placeX + x1 / 2, placeY - y1 / 2] ||
                                                        _grid[placeX - x1 / 2, placeY + y1 / 2] ) return true;
                        }
                        catch
                        {

                        }//TODO: Fix
                    }
                }
            }
        }
        else
        {
            for (int x1 = 0; x1 < _flyingUnit.Size.x; x1++)
            {
                for (int y1 = 0; y1 < _flyingUnit.Size.y; y1++)
                {
                    if (_gridSize.x > placeX + x1 && _gridSize.y > placeY + y1)
                    {
                        if (_grid[placeX + x1, placeY + y1])
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    } //Преверка клетки на занятость другим юнитом

    public void PlaceFlyingUnit() //Установка юнита
    {
        if(available &&_flyingUnit != null && _gameManger.MoneyBalance - _flyingUnit.GetComponentInChildren<Unit>().UnitPrice >= 0)
        {
            if (_flyingUnit.Size.x * _flyingUnit.Size.y % 2 != 0 && _flyingUnit.Size.x * _flyingUnit.Size.y != 1)
            {
                for (int x1 = 0; x1 < _flyingUnit.Size.x; x1++)
                {
                    for (int y1 = 0; y1 < _flyingUnit.Size.y; y1++)
                    {
                        if (x - x1 / 2 >= 0 && y - y1 / 2 >= 0 && x + x1 / 2 < _gridSize.x && y + y1 / 2 < _gridSize.y)
                        {
                            _grid[x + x1 / 2, y + y1 / 2] = _flyingUnit;
                            _grid[x - x1 / 2, y - y1 / 2] = _flyingUnit;
                            _grid[x + x1 / 2, y - y1 / 2] = _flyingUnit;
                            _grid[x - x1 / 2, y + y1 / 2] = _flyingUnit;
                            _cells[x + x1 / 2, y + y1 / 2].GetComponent<Renderer>().material.color = Color.grey;
                            _cells[x - x1 / 2, y - y1 / 2].GetComponent<Renderer>().material.color = Color.grey;
                            _cells[x + x1 / 2, y - y1 / 2].GetComponent<Renderer>().material.color = Color.grey;
                            _cells[x - x1 / 2, y + y1 / 2].GetComponent<Renderer>().material.color = Color.grey;
                        }
                    }
                }
            }
            else
            {
                for (int x1 = 0; x1 < _flyingUnit.Size.x; x1++)
                {
                    for (int y1 = 0; y1 < _flyingUnit.Size.y; y1++)
                    {
                        _grid[x + x1, y + y1] = _flyingUnit;
                        _cells[x + x1, y + y1].GetComponent<Renderer>().material.color = Color.grey;
                    }
                }
            }
            _gameManger.MoneyBalance -= _flyingUnit.GetComponentInChildren<Unit>().UnitPrice;
            _flyingUnit.GetComponentInChildren<BoxCollider>().enabled = true;
            _flyingUnit.SetColorNormal();
            _flyingUnit = Instantiate(currentUnit);
            _activeCells.Clear();
            _isPlacementMode = true;
        }
        
    }

    private void OnDrawGizmos()
    {
        for (int x1 = 0; x1 < _gridSize.x; x1++)
        {
            for (int y1 = 0; y1 < _gridSize.y; y1++)
            {
                Gizmos.DrawCube(new Vector3(x1 + transform.position.x,
                                           transform.position.y, y1 + transform.position.z), new Vector3(.8f, .8f, .8f));
            }
        }
    }

    public void ClearUnits() //Очистка юнитов
    {
        for (int x1 = 0; x1 < _gridSize.x; x1++)
        {
            for (int y1 = 0; y1 < _gridSize.y; y1++)
            {
                if(_grid[x1,y1])
                {
                    Destroy(_grid[x1, y1].gameObject);
                    _cells[x1, y1].GetComponent<Renderer>().material.color = origCol;
                }
            }
        }
        _gameManger.RefreshBalance();
    }
}
