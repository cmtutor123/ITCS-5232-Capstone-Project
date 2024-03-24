using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool managerExists = false;
    public static bool loadedData = false;
    public static GameState gameState = GameState.Title;
    public static PlayerData playerData;
    public static List<MenuState> menuStates;

    void Start()
    {
        if (managerExists)
        {
            Destroy(gameObject);
        }
        managerExists = true;
        DontDestroyOnLoad(gameObject);
        menuStates = new List<MenuState>();
        SetMenu(MenuState.Title);
        if (!loadedData)
        {
            LoadData();
            loadedData = true;
        }
        SetMenu(MenuState.MainMenu);
    }

    public void LoadData()
    {
        playerData = FileManager.LoadPlayerData();
    }

    public void SwitchMenu(MenuState menu)
    {
        menuStates.Add(menu);
        LoadMenu();
    }

    public void SetMenu(params MenuState[] menus)
    {
        menuStates.Clear();
        foreach (MenuState menu in menus)
        {
            menuStates.Add(menu);
        }
        LoadMenu();
    }

    public MenuState GetMenu()
    {
        if (menuStates == null || menuStates.Count == 0) return MenuState.None;
        else return menuStates[menuStates.Count - 1];
    }

    public void LoadMenu()
    {
        MenuState menu = GetMenu();
        HideAllUI();
        switch (menu)
        {
            case MenuState.Title:
                break;
        }
    }

    public void HideAllUI()
    {

    }
}

public enum GameState
{
    Switching,
    Title,
    MainMenu
}

public enum MenuState
{
    None,
    Title,
    MainMenu,
    Options,
    CharacterSelect,
    StageSelect,
    PerkSelect,
    Match,
    Results,
    Pause
}

public enum PlayerClass
{
    Berserker,
    Druid,
    Necromancer,
    Paladin,
    Rogue,
    Wizard
}