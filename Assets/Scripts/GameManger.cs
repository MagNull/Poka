using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManger : MonoBehaviour
{
    [Header("Grid Fields")]
    private GameObject[] _cells;
    private Grid _grid;
    private EnemyGrid _enemyGrid;

    [Header("Level Fields")]
    [SerializeField] private LevelConfig level;
    [SerializeField] private LevelConfig[] levels;
    [SerializeField] private ButtonLighter[] levelButtons;
    private int _moneyBalance;

    [Header("UI Fields")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject chooseLevelMenu;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TMPro.TextMeshProUGUI balanceText;
    private Button[] _buttons;
    [SerializeField] private Button activeButton;
    [SerializeField] private ButtonLighter[] unitButtons;

    [SerializeField] private Unit[] units;

    [Header("Swap Params")]
    [SerializeField] private float swapSpeed;
    [SerializeField] private float swapRoatationSpeed;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Transform secondTransform;
    private Camera _cameraMain;

    private bool _isGameStart = false;

    private bool _cameraPos = false;

    public bool isDebugMode = false;


    public int MoneyBalance 
    { 
        get => _moneyBalance;

        set 
        {
            _moneyBalance = value;
            CheckButton();
            balanceText.text = _moneyBalance.ToString();
        }
    }

    private void Start()
    {
        _cameraMain = Camera.main;
        if (isDebugMode)
        {
            _moneyBalance = 9999;
        }
        else
        {
            _moneyBalance = level.LevelMoneyBalance;
        }
        balanceText.text = _moneyBalance.ToString();
        InitializeGrids();
        _buttons = FindObjectsOfType<UnityEngine.UI.Button>();
        if (!isDebugMode)
        {
            for (var i = 1; i < levelButtons.Length; i++)
            {
                levelButtons[i].ChangeButtonState(false);
            }
            CheckLevels();
        }
    }

    private void InitializeGrids() //Инициализация сеток
    {
        _cells = GameObject.FindGameObjectsWithTag("Cell");
        _grid = FindObjectOfType<Grid>();
        _enemyGrid = FindObjectOfType<EnemyGrid>();
    }
    

    public void SetDebugState(bool state)
    {
        isDebugMode = state;
        for (var i = 1; i < levelButtons.Length; i++)
        {
            levelButtons[i].ChangeButtonState(isDebugMode);
        }
        CheckLevels();
    }

    private void CheckButtonOnLevel() //Проверка доступности юнита на уровне
    {
        if (!isDebugMode)
        {
            unitButtons[0].gameObject.SetActive(level.Archer ? true : false);
            unitButtons[1].gameObject.SetActive(level.Warrior ? true : false);
            unitButtons[2].gameObject.SetActive(level.Shielder ? true : false);
            unitButtons[3].gameObject.SetActive(level.Spearman ? true : false);
            unitButtons[4].gameObject.SetActive(level.Catapult ? true : false);
        }
        else
        {
            foreach (ButtonLighter button in unitButtons)
            {
                button.gameObject.SetActive(true);
            }
        }
        
    }

    private void CheckLevels() //Проверка пройденых уровней
    {
        for (var i = 1; i <= PlayerPrefs.GetInt("Levels Complete"); i++)
        {
            levelButtons[i].ChangeButtonState(true);
        }
    }


    private void CheckButton() //Проверка доступности покупки юнита
    {
        if (!_isGameStart)
        {
            for (var i = 0; i < unitButtons.Length; i++)
            {
                if (_moneyBalance - units[i].UnitPrice < 0)
                {
                    unitButtons[i].ChangeButtonState(false);
                }
                else
                {
                    unitButtons[i].ChangeButtonState(true);
                }
            }
        }
    }

    public void SwapCamera() //Изменение положения камеры 
    {
        if(_cameraPos)
        {
            StopAllCoroutines();
            StartCoroutine(CameraInterpolate(_cameraMain.transform, mainTransform));
            _cameraPos = false;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(CameraInterpolate(_cameraMain.transform, secondTransform));
            _cameraPos = true;
        }
    }

    public void SwapCamera(bool pos) //Изменение положения камеры 
    {
        if (pos)
        {
            StopAllCoroutines();
            StartCoroutine(CameraInterpolate(_cameraMain.transform, mainTransform));
            _cameraPos = false;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(CameraInterpolate(_cameraMain.transform, secondTransform));
            _cameraPos = true;
        }
    }

    private IEnumerator CameraInterpolate(Transform trans, Transform to) //Корутина движения камеры
    {
        while(trans.position != to.position && trans.rotation != to.rotation)
        {
            trans.position = Vector3.Lerp(trans.position, to.position, Time.deltaTime * swapSpeed);
            trans.rotation = Quaternion.Slerp(trans.rotation, to.rotation, Time.deltaTime * swapRoatationSpeed);
            yield return null;
        }
    }
    public void Win() // Вызов окна победы
    {
        OpenNextLevel();
        _grid.ClearUnits();
        gameUI.SetActive(false);
        winScreen.SetActive(true);
    }
    private void OpenNextLevel() //Открытипе следующего уровня
    {
        if(!levelButtons[Array.IndexOf(levels, level) + 1].GetButtonState())
        {
            levelButtons[Array.IndexOf(levels, level) + 1].ChangeButtonState(true);
        }
        PlayerPrefs.SetInt("Levels Complete", Array.IndexOf(levels, level) + 1);
    }


    public void GoToNextLevel() //Перемещение на следующий уровень
    {
        _enemyGrid.SetLevelConfig(levels[Array.IndexOf(levels, level) + 1]);
        level = levels[Array.IndexOf(levels, level) + 1];
        RefreshBalance();
        winScreen.SetActive(false);
        gameUI.SetActive(true);
        SetGameState(false);
        CheckButtonOnLevel();
    }


    public void SetGameState(bool state) //Изменение состояния игры (режим расстановки/сражения)
    {
        _isGameStart = state;
        SwapCamera(true);
        Unit[] soldiers = FindObjectsOfType<Unit>();
        foreach (Unit soldier in soldiers)
        {
            soldier.enabled = state;
            soldier.GetComponent<BoxCollider>().isTrigger = !state;
        }
        foreach (UnityEngine.UI.Button button in _buttons)
        {
            button.GetComponent<ButtonLighter>()?.ChangeButtonState(!state);
        }
        foreach (GameObject cell in _cells)
        {
            cell.GetComponent<Renderer>().material.color = new Color(0.1268245f, 0.4716981f, 0.1268245f);
            cell.SetActive(!state);
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

    
    public void Restart() //Ну рестарт
    {
        SetGameState(false);
        _grid.ClearUnits();
        _enemyGrid.ClearUnits();
        _enemyGrid.SpawnEnemys();
        RefreshBalance();
        if(gameUI.activeSelf == false)
        {
            gameUI.SetActive(true);
        } 
        winScreen.SetActive(false);
        Projectile[] proj = FindObjectsOfType<Projectile>();
        for(var i = 0; i < proj.Length; i++)
        {
            Destroy(proj[i].gameObject);
        }

    }


    public void RefreshBalance() //Обновление баланса на уровне
    {
        if (isDebugMode)
        {
            MoneyBalance = 9999;
        }
        else
        {
            MoneyBalance = level.LevelMoneyBalance;
        }
    }


    public void Play()
    {
        chooseLevelMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    

    public void ChooseLevel(int index) //Выбор и загрузка уровня
    {
        _enemyGrid.SetLevelConfig(levels[index - 1]);
        level = levels[index - 1];
        chooseLevelMenu.SetActive(false);
        gameUI.SetActive(true);
        RefreshBalance();
        _buttons = FindObjectsOfType<UnityEngine.UI.Button>();
        CheckButtonOnLevel();
    }


    public void BackToChoose() //Возвращение на UI выбора уровней
    {
        SetGameState(false);
        _grid.ClearUnits();
        _enemyGrid.ClearUnits();
        gameUI.SetActive(false);
        chooseLevelMenu.SetActive(true);
        SwapCamera(true);
    }


    public void BackToMainMenu() //Возвращение в основное меню
    {
        gameUI.SetActive(false);
        winScreen.SetActive(false);
        chooseLevelMenu.SetActive(false);
        mainMenu.SetActive(true);
        SwapCamera(true);
        SetGameState(false);
    }


    public void Exit() //Выход
    {
        Application.Quit();
    }
}
