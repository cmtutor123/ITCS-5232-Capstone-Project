using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool managerExists = false;
    public static bool loadedData = false;
    public static GameState gameState = GameState.Title;
    public static PlayerData playerData;

    void Start()
    {
        if (managerExists)
        {
            Destroy(gameObject);
        }
        managerExists = true;
        DontDestroyOnLoad(gameObject);
        if (!loadedData)
        {
            LoadData();
            loadedData = true;
        }
        SwitchToMainMenu();
    }

    public void LoadData()
    {
        playerData = FileManager.LoadPlayerData();
    }

    public void SwitchToMainMenu()
    {
        gameState = GameState.Switching;
        gameState = GameState.MainMenu;
    }
}

public enum GameState
{
    Switching,
    Title,
    MainMenu
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