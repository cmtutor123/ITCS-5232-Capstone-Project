using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool loadedData = false;
    public PlayerData playerData;
    public List<MenuState> menuStates;
    public float fadeTime = 0.5f;

    public List<ClassData> classData;

    private Dictionary<MenuState, GameObject> menuUi = new Dictionary<MenuState, GameObject>();
    [Header("Fade")]
    public Image fade;
    [Header("UI Canvas")]
    public GameObject uiLoad;
    public GameObject uiMainMenu;
    public GameObject uiOptions;
    public GameObject uiCharacterSelect;
    public GameObject uiStageSelect;
    public GameObject uiPerkSelect;
    public GameObject uiMatch;
    public GameObject uiResults;
    public GameObject uiPause;
    [Header("UI Buttons")]
    public List<EmblemButton> characterButtons;
    public ClassDescription classTooltip;
    public List<LockableButton> stageButtons;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        InitializeMenuUiDictionary();
        menuStates = new List<MenuState>();
        SetMenu(MenuState.Load);
        if (!loadedData)
        {
            LoadData();
            loadedData = true;
        }
        InitializeButtonIndex();
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

    public IEnumerator FadeOut(float seconds)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / seconds)
        {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, Mathf.Lerp(0, 1, t));
            yield return null;
        }
        fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);
    }

    public IEnumerator FadeIn(float seconds)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / seconds)
        {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, Mathf.Lerp(1, 0, t));
            yield return null;
        }
        fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 0);
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
        StartCoroutine(LoadMenuCoroutine());
    }

    public IEnumerator LoadMenuCoroutine()
    {
        yield return FadeOut(fadeTime);
        MenuState menu = GetMenu();
        HideAllUI();
        if (menuUi.ContainsKey(menu))
        {
            menuUi[menu].SetActive(true);
        }
        StartCoroutine(FadeIn(fadeTime));
    }

    public void HideAllUI()
    {
        foreach (GameObject menu in menuUi.Values)
        {
            menu.SetActive(false);
        }
    }

    public void InitializeMenuUiDictionary()
    {
        menuUi.Add(MenuState.Load, uiLoad);
        menuUi.Add(MenuState.MainMenu, uiMainMenu);
        menuUi.Add(MenuState.Options, uiOptions);
        menuUi.Add(MenuState.CharacterSelect, uiCharacterSelect);
        menuUi.Add(MenuState.StageSelect, uiStageSelect);
        menuUi.Add(MenuState.PerkSelect, uiPerkSelect);
        menuUi.Add(MenuState.Match, uiMatch);
        menuUi.Add(MenuState.Results, uiResults);
        menuUi.Add(MenuState.Pause, uiPause);
    }

    public void RemoveMenu()
    {
        menuStates.RemoveAt(menuStates.Count - 1);
        LoadMenu();
    }

    public void InitializeButtonIndex()
    {
        int index = 0;
        foreach (EmblemButton button in characterButtons)
        {
            ButtonIndex buttonIndex = button.gameObject.AddComponent<ButtonIndex>();
            buttonIndex.SetIndex(index++);
            button.gameObject.AddComponent<ClassTooltip>();
            button.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() => gameObject.GetComponent<ButtonManager>().ButtonSelectCharacter(buttonIndex.GetIndex()));
        }
    }

    public void UpdateCharacterSelect()
    {
        for (int i = 0; i < 6; i++)
        {
            ClassData currentData = classData[i];
            EmblemButton currentButton = characterButtons[i];
            string currentName = currentData.className;
            int currentLevel = playerData.level[i];
            Color currentLightColor = currentData.classColorLight;
            Color currentDarkColor = currentData.classColorDark;
            currentButton.ChangeText(currentName);
            currentButton.ChangeLevel(currentLevel);
            currentButton.ChangeColor(currentLightColor, currentDarkColor);
        }
    }

    public void SetCurrentCharacter(int character)
    {

    }

    public void ShowClassTooltip(int index)
    {
        ClassData currentData = classData[index];
        classTooltip.ChangeSprite(currentData.classSprite);
        classTooltip.ChangeName(currentData.className);
        classTooltip.ChangeDescription(currentData.classDescription);
        classTooltip.gameObject.SetActive(true);
    }

    public void HideClassTooltip()
    {
        classTooltip.gameObject.SetActive(false);
    }

    public void UpdateStageSelect()
    {

    }

    public void SetCurrentStage(int stage)
    {

    }

    public void SetCurrentDifficulty(int difficulty)
    {

    }

    public void UpdatePerkSelect()
    {
        
    }
}

public enum MenuState
{
    None,
    Load,
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