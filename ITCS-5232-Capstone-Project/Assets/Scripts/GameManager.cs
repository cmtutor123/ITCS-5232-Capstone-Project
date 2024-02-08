using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool managerExists = false;
    public static bool loadedData = false;
    public static GameState gameState = GameState.Title;

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
