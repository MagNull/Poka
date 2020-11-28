using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMethods : MonoBehaviour
{
    public bool isDebugMode = false;

    [Header("Level Fields")]
    private LevelConfig _currentLevel;
    [SerializeField] private LevelConfig[] levels;
    [SerializeField] private ButtonLighter[] levelButtons;
    private int _moneyBalance;

    [Header("UI Fields")]
    public GameObject gameUI;
    public GameObject winScreen;
    [SerializeField] private GameObject chooseLevelMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private TMPro.TextMeshProUGUI balanceText;
    [SerializeField] private ButtonLighter[] unitButtons;
    private Button[] _buttons;

    [SerializeField] private Unit[] units;
    [SerializeField] private GameStateController gameStateController;
    [SerializeField] private CameraMover cameraMover;

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

    public Button[] Buttons => _buttons;

    private void Start()
    {
        if (isDebugMode)
        {
            _moneyBalance = 9999;
        }
        balanceText.text = _moneyBalance.ToString();
        if (!isDebugMode)
        {
            for (var i = 1; i < levelButtons.Length; i++)
            {
                levelButtons[i].ChangeButtonState(false);
            }
            CheckLevels();
        }
        _buttons = FindObjectsOfType<Button>();
    }
    
    private void CheckButton() 
    {
        if (!GameStateController.IsGameStart)
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
    
    public void SetDebugState(bool state)
    {
        isDebugMode = state;
        for (var i = 1; i < levelButtons.Length; i++)
        {
            levelButtons[i].ChangeButtonState(isDebugMode);
        }
        CheckLevels();
        _buttons = FindObjectsOfType<Button>();
    }
    

    
    private void CheckLevels() 
    {
        for (var i = 1; i <= PlayerPrefs.GetInt("Levels Complete"); i++)
        {
            levelButtons[i].ChangeButtonState(true);
        }
    }
    
    private void CheckButtonOnLevel() 
    {
        if (!isDebugMode)
        {
            unitButtons[0].gameObject.SetActive(_currentLevel.Archer);
            unitButtons[1].gameObject.SetActive(_currentLevel.Warrior);
            unitButtons[2].gameObject.SetActive(_currentLevel.Shielder);
            unitButtons[3].gameObject.SetActive(_currentLevel.Spearman);
            unitButtons[4].gameObject.SetActive(_currentLevel.Catapult);
        }
        else
        {
            foreach (ButtonLighter button in unitButtons)
            {
                button.gameObject.SetActive(true);
            }
        }
    }
    
    public void Win() 
    {
        OpenNextLevel();
        gameStateController.Grid.ClearUnits();
        gameUI.SetActive(false);
        winScreen.SetActive(true);
    }
    private void OpenNextLevel() 
    {
        if(!levelButtons[Array.IndexOf(levels, _currentLevel) + 1].GetButtonState())
        {
            levelButtons[Array.IndexOf(levels, _currentLevel) + 1].ChangeButtonState(true);
        }
        PlayerPrefs.SetInt("Levels Complete", Array.IndexOf(levels, _currentLevel) + 1);
    }


    public void GoToNextLevel() 
    {
        gameStateController.EnemyGrid.SetLevelConfig(levels[Array.IndexOf(levels, _currentLevel) + 1]);
        _currentLevel = levels[Array.IndexOf(levels, _currentLevel) + 1];
        RefreshBalance();
        winScreen.SetActive(false);
        gameUI.SetActive(true);
        gameStateController.SetGameState(false);
        CheckButtonOnLevel();
    }
    
    public void RefreshBalance() 
    {
        if (isDebugMode)
        {
            MoneyBalance = 9999;
        }
        else
        {
            MoneyBalance = _currentLevel.LevelMoneyBalance;
        }
    }
    
    public void Play()
    {
        chooseLevelMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    

    public void ChooseLevel(int index) 
    {
        gameStateController.EnemyGrid.SetLevelConfig(levels[index - 1]);
        _currentLevel = levels[index - 1];
        chooseLevelMenu.SetActive(false);
        gameUI.SetActive(true);
        RefreshBalance();
        _buttons = FindObjectsOfType<UnityEngine.UI.Button>();
        CheckButtonOnLevel();
    }


    public void BackToChoose() 
    {
        gameStateController.SetGameState(false);
        gameStateController.Grid.ClearUnits();
        gameStateController.EnemyGrid.ClearUnits();
        gameUI.SetActive(false);
        chooseLevelMenu.SetActive(true);
        cameraMover.SwapCamera(true);
    }


    public void BackToMainMenu() 
    {
        gameUI.SetActive(false);
        winScreen.SetActive(false);
        chooseLevelMenu.SetActive(false);
        mainMenu.SetActive(true);
        cameraMover.SwapCamera(true);
        gameStateController.SetGameState(false);
    }


    public void Exit() 
    {
        Application.Quit();
    }
}
