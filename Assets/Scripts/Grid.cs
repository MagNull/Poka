using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool InTest;
    [Header("Grid Params")]
    [SerializeField] private Vector2Int gridSize;
    private GridUnit[,] _grid;
    [SerializeField] private GameObject cellPrefab;

    private GridUnit _flyingUnit;
    private MeshRenderer[,] _cells;


    private List<Renderer> _activeCells = new List<Renderer>();

    [Header("Soldiers Prefabs")]
    [SerializeField] private GridUnit warriorPrefab;
    [SerializeField] private GridUnit archerPrefab;
    [SerializeField] private GridUnit shielderPrefab;
    [SerializeField] private GridUnit spearmanPrefab;
    [SerializeField] private GridUnit catapultPrefab;

    private GridUnit _currentUnit;

    private int x;
    private int y;

    private Camera currentCamera;

    private Color origCol;
    private bool available;
    private UIMethods _ui;

    private bool _isPlacementMode = true;
    private float _offsetToCentreUnit = 0.27f;

    public bool IsPlacementMode { get => _isPlacementMode; set => _isPlacementMode = value; }
    public GridUnit FlyingUnit 
    { 
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

    public MeshRenderer[,] Cells => _cells;

    private void Start()
    {
        CurrentCamera = Camera.main;
        _ui = FindObjectOfType<UIMethods>();
        origCol = cellPrefab.GetComponent<Renderer>().sharedMaterial.color;
        _grid = new GridUnit[gridSize.x, gridSize.y];
        _cells = new MeshRenderer[gridSize.x, gridSize.y];
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Cells[x, y] = Instantiate(cellPrefab, 
                    new Vector3(x + transform.position.x,
                                          transform.position.y, 
                                         y + transform.position.z), 
                    Quaternion.identity).GetComponent<MeshRenderer>();
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
    } 
    private void Update()
    {
        Placement();
        MoveUnit();
        if (Input.GetMouseButtonDown(0))
        {
            PlaceFlyingUnit();
        }
    }

    private void MoveUnit()    
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(FlyingUnit != null)
            {
                ResetCellsColor();
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
                    for (int x = 0; x < gridSize.x; x++)
                    {
                        for (int y = 0; y < gridSize.y; y++)
                        { 
                            if(_grid[x,y] == unit)
                            {
                                _grid[x,y] = null;
                                Cells[x,y].material.color = origCol;
                            }
                        }
                    }
                    FlyingUnit = unit;
                }
            }
        }
    }

    private void ResetCellsColor()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (Cells[x, y].material.color != origCol && !_grid[x, y])
                {
                    Cells[x, y].material.color = origCol;
                }
            }
        }
    }

    private void Placement() 
    {
        StartPlacingUnit();
        if (_flyingUnit != null)
        {
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            Ray ray = CurrentCamera.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);
                x = Mathf.RoundToInt(worldPosition.x) - (int)transform.position.x;
                y = Mathf.RoundToInt(worldPosition.z) - (int)transform.position.z;

                available = CheckAvialable(x, y);
                _flyingUnit.SetTransparent(available);
                ChangeCellsColor(x, y, available,_flyingUnit.Size);
            }

        }
    }

    public void SetCurrentUnit(GridUnit unit) 
    {
        _currentUnit = unit;
        if(_flyingUnit != null)
        {
            Destroy(_flyingUnit.gameObject);
        }
        _flyingUnit = Instantiate(_currentUnit);
    }

    private void ChangeCellsColor(int x, int y, bool available, Vector2 size)
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
                        if (x - x1 / 2 >= 0 && y - y1 / 2 >= 0 && x + x1 / 2 < gridSize.x && y + y1 / 2 < gridSize.y)
                        {
                            if (!_grid[x - x1 / 2, y - y1 / 2])
                            {
                                _activeCells.Add(Cells[x - x1 / 2, y - y1 / 2]);
                            }
                            if(!_grid[x + x1 / 2, y - y1 / 2])
                            {
                                _activeCells.Add(Cells[x + x1 / 2, y - y1 / 2]);
                            }
                            if(!_grid[x - x1 / 2, y + y1 / 2])
                            {
                                _activeCells.Add(Cells[x - x1 / 2, y + y1 / 2]);
                            }
                            if(!_grid[x + x1 / 2, y + y1 / 2])
                            {
                                _activeCells.Add(Cells[x + x1 / 2, y + y1 / 2]);
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
                        _activeCells.Add(Cells[x + x1, y + y1]);
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
    }

    private bool CheckAvialable(int x, int y)
    {
        bool available = true;
        if(_flyingUnit.Size.x * _flyingUnit.Size.y % 2 != 0)
        {
            if (x < 0 || x + _flyingUnit.Size.x / 2 + 1 > gridSize.x  || x -_flyingUnit.Size.x / 2  < 0 )
            {
                available = false;
            }
            if (y < 0 || y + _flyingUnit.Size.y / 2 + 1 > gridSize.y || y - _flyingUnit.Size.y / 2  < 0)
            {
                available = false;
            }
        }
        else
        {
            if (x < 0 || x > gridSize.x - _flyingUnit.Size.x)
            {
                available = false;
            }
            if (y < 0 || y > gridSize.y - _flyingUnit.Size.y)
            {
                available = false;
            }
        }
        if (available && IsPlaceTaken(x, y))
        {
            available = false;
        }

        Vector3 newUnitPosition = new Vector3(x - _offsetToCentreUnit + (int)transform.position.x, _flyingUnit.transform.position.y,
            y + (int)transform.position.z);
        _flyingUnit.transform.position = newUnitPosition;

        return available;
    } 

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        if (_flyingUnit.Size.x * _flyingUnit.Size.y % 2 != 0 && _flyingUnit.Size.x * _flyingUnit.Size.y != 1)
        {
            for (int x1= 0; x1 < _flyingUnit.Size.x; x1++)
            {
                for (int y1 = 0; y1 < _flyingUnit.Size.y; y1++)
                {
                    if (gridSize.x > (placeX + x1 / 2) && gridSize.y > (placeY + y1 / 2) && (placeY + y1 / 2) > 0
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
                    if (gridSize.x > placeX + x1 && gridSize.y > placeY + y1)
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
    }

    public void PlaceFlyingUnit() 
    {
        if(available &&_flyingUnit != null && _ui.MoneyBalance - _flyingUnit.GetComponentInChildren<Unit>().UnitPrice >= 0)
        {
            if (_flyingUnit.Size.x * _flyingUnit.Size.y % 2 != 0 && _flyingUnit.Size.x * _flyingUnit.Size.y != 1)
            {
                for (int x1 = 0; x1 < _flyingUnit.Size.x; x1++)
                {
                    for (int y1 = 0; y1 < _flyingUnit.Size.y; y1++)
                    {
                        if (x - x1 / 2 >= 0 && y - y1 / 2 >= 0 && x + x1 / 2 < gridSize.x && y + y1 / 2 < gridSize.y)
                        {
                            AssignUnitToCell(x1 / 2,y1 / 2);
                            AssignUnitToCell(-x1 / 2, -y1 / 2);
                            AssignUnitToCell(x1 / 2, -y1 / 2);
                            AssignUnitToCell(-x1 / 2, y1 / 2);
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
                        Cells[x + x1, y + y1].material.color = Color.grey;
                    }
                }
            }
            _ui.MoneyBalance -= _flyingUnit.GetComponentInChildren<Unit>().UnitPrice;
            _flyingUnit.GetComponentInChildren<BoxCollider>().enabled = true;
            _flyingUnit.SetColorNormal();
            Vector3 unitPosition = new Vector3(x - _offsetToCentreUnit + (int)transform.position.x, _flyingUnit.transform.position.y,
                y + (int)transform.position.z);
            _flyingUnit = Instantiate(_currentUnit,unitPosition,Quaternion.identity);
            _activeCells.Clear();
            _isPlacementMode = true;
        }
        
    }

    private void AssignUnitToCell(int x1, int y1)
    {
        _grid[x + x1, y + y1] = _flyingUnit;
        Cells[x + x1,y + y1].material.color = Color.grey;
    }
    

    private void OnDrawGizmos()
    {
        for (int x1 = 0; x1 < gridSize.x; x1++)
        {
            for (int y1 = 0; y1 < gridSize.y; y1++)
            {
                Gizmos.DrawCube(new Vector3(x1 + transform.position.x,
                                           transform.position.y, y1 + transform.position.z), new Vector3(.8f, .8f, .8f));
            }
        }
    }

    public void ClearUnits() 
    {
        for (int x1 = 0; x1 < gridSize.x; x1++)
        {
            for (int y1 = 0; y1 < gridSize.y; y1++)
            {
                if(_grid[x1,y1])
                {
                    Destroy(_grid[x1, y1].gameObject);
                    Cells[x1, y1].material.color = origCol;
                }
            }
        }
        _ui.RefreshBalance();
    }
}
