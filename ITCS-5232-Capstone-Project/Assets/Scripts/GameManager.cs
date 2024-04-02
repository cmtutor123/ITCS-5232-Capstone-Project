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
    public List<StageData> stageData;

    public int currentCharacter, currentStage, currentDifficulty;

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
    public List<LockableButton> difficultyButtons;
    public TextButton stageBack;
    public TextButton difficultyBack;
    public EmblemButton stageEmblem;
    public PowerMeter powerMeter;
    public PerkLoadout perkLoadout;
    public PerkSelection perkSelection;
    public EmblemButton perkEmblem;
    [Header("Loadout Information")]
    public int[,] loadoutData = new int[PlayerData.CLASS_COUNT, 10];
    public string[] loadoutSlots = { "NormalMain", "NormalMod", "SpecialMain", "SpecialMod", "ChargedMain", "ChargedMod", "PassiveA1", "PassiveA2", "PassiveB1", "PassiveB1" };
    public int[] loadoutDefaults = { 0, -1, 0, -1, 0, -1, -1, -1, -1, -1 };

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

    public void LoadLoadoutData()
    {
        for (int c = 0; c < classData.Count; c++)
        {
            string className = classData[c].className;
            for (int s = 0; s < loadoutSlots.Length; s++)
            {
                loadoutData[c, s] = PlayerPrefs.GetInt(className + loadoutSlots[s], loadoutDefaults[s]);
            }
        }
        CheckLoadoutData();
        SaveLoadoutData();
    }

    public void SaveLoadoutData()
    {
        for (int c = 0; c < classData.Count; c++)
        {
            string className = classData[c].className;
            for (int s = 0; s < loadoutSlots.Length; s++)
            {
                PlayerPrefs.SetInt(className + loadoutSlots[s], loadoutData[c, s]);
            }
        }
    }

    public void CheckLoadoutData()
    {
        for (int c = 0; c < classData.Count; c++)
        {
            CheckLoadoutPerk(c, loadoutData[c, 0], 0, 0, false);
            CheckLoadoutPerk(c, loadoutData[c, 1], 0, -1, true, false);
            CheckLoadoutPerk(c, loadoutData[c, 2], 12, 0, false);
            CheckLoadoutPerk(c, loadoutData[c, 3], 12, -1, true, false);
            CheckLoadoutPerk(c, loadoutData[c, 4], 24, 0, false);
            CheckLoadoutPerk(c, loadoutData[c, 5], 24, -1, true, false);
            CheckLoadoutPerk(c, loadoutData[c, 6], 36, -1);
            CheckLoadoutPerk(c, loadoutData[c, 7], 36, -1);
            CheckLoadoutPerk(c, loadoutData[c, 8], 48, -1);
            CheckLoadoutPerk(c, loadoutData[c, 9], 48, -1);
        }
    }

    public void CheckLoadoutPerk(int cIndex, int sIndex, int pOffset, int defaultValue, bool canBeEmpty = true, bool isModSlot = false)
    {
        if (sIndex == -1)
        {
            if (!canBeEmpty)
            {
                loadoutData[cIndex, sIndex] = defaultValue;
            }
        }
        else if (playerData.perks[cIndex, sIndex + pOffset] != 1)
        {
            loadoutData[cIndex, sIndex] = defaultValue;
        }
        else
        {
            if (isModSlot)
            {
                int modNumber = sIndex % 4;
                int modBase = sIndex - modNumber;
                if (loadoutData[cIndex, modBase + pOffset] != modBase)
                {
                    loadoutData[cIndex, sIndex] = defaultValue;
                }
            }
        }
    }

    public void UpdateLoadoutPerk(int loadoutIndex, int perkIndex)
    {
        if (loadoutIndex >= 0 && loadoutIndex <= 5)
        {
            UpdateLoadoutAbility(loadoutIndex, perkIndex);
        }
        else if (loadoutIndex >= 6 && loadoutIndex <= 9)
        {
            UpdateLoadoutPassive(loadoutIndex, perkIndex);
        }
    }

    public void UpdateLoadoutAbility(int loadoutIndex, int perkIndex)
    {
        bool isSlotBase = loadoutIndex % 2 == 0;
        bool isAbilityBase = perkIndex % 4 == 0;
        int slotBase, slotMod;
        if (isSlotBase)
        {
            slotBase = loadoutIndex;
            slotMod = loadoutIndex + 1;
        }
        else
        {
            slotBase = loadoutIndex - 1;
            slotMod = loadoutIndex;
        }
        int abilityBase, abilityMod;
        if (isAbilityBase)
        {
            abilityBase = perkIndex;
            abilityMod = -1;
        }
        else
        {
            abilityBase = perkIndex / 4;
            abilityMod = perkIndex;
        }
        SetLoadoutPerk(slotBase, abilityBase);
        SetLoadoutPerk(slotMod, abilityMod);
    }

    public void UpdateLoadoutPassive(int loadoutIndex, int perkIndex)
    {
        int otherSlot = loadoutIndex + (loadoutIndex % 2 == 0 ? 1 : -1);
        if (loadoutData[currentCharacter, otherSlot] == perkIndex)
        {
            SetLoadoutPerk(otherSlot, -1);
            SetLoadoutPerk(loadoutIndex, perkIndex);
        }
        else
        {
            SetLoadoutPerk(loadoutIndex, perkIndex);
        }
    }

    public void SetLoadoutPerk(int loadoutIndex, int perkIndex)
    {
        loadoutData[currentCharacter, loadoutIndex] = perkIndex;
        SaveLoadoutData();
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
        index = 0;
        foreach (LockableButton button in stageButtons)
        {
            ButtonIndex buttonIndex = button.gameObject.AddComponent<ButtonIndex>();
            buttonIndex.SetIndex(index++);
            button.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() => gameObject.GetComponent<ButtonManager>().ButtonSelectStage(buttonIndex.GetIndex()));
        }
        index = 0;
        foreach (LockableButton button in difficultyButtons)
        {
            ButtonIndex buttonIndex = button.gameObject.AddComponent<ButtonIndex>();
            buttonIndex.SetIndex(index++);
            button.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() => gameObject.GetComponent<ButtonManager>().ButtonSelectDifficulty(buttonIndex.GetIndex()));
        }
        index = 0;
        foreach (PerkButton button in perkLoadout.perkButtons)
        {
            ButtonIndex buttonIndex = button.gameObject.AddComponent<ButtonIndex>();
            buttonIndex.SetIndex(index++);
            button.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() => gameObject.GetComponent<ButtonManager>().ButtonPerkLoadout(buttonIndex.GetIndex()));
        }
        index = 0;
        foreach (PerkButton button in perkSelection.perkButtons)
        {
            ButtonIndex buttonIndex = button.gameObject.AddComponent<ButtonIndex>();
            buttonIndex.SetIndex(index++);
            button.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() => gameObject.GetComponent<ButtonManager>().ButtonPerkSelection(buttonIndex.GetIndex()));
        }
    }

    public void UpdateCharacterSelect()
    {
        for (int i = 0; i < 6; i++)
        {
            UpdateCharacterEmblem(characterButtons[i], i);
        }
    }

    public void UpdateCharacterEmblem(EmblemButton emblemButton, int characterIndex)
    {
        ClassData data = classData[characterIndex];
        emblemButton.ChangeText(data.className);
        emblemButton.ChangeLevel(playerData.level[characterIndex]);
        emblemButton.ChangeColor(data.classColorLight, data.classColorDark);
    }

    public void SetCurrentCharacter(int character)
    {
        currentCharacter = character;
        UpdateCharacterEmblem(stageEmblem, character);
        UpdateStageSelect();
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
        foreach (LockableButton button in difficultyButtons)
        {
            button.gameObject.SetActive(false);
        }
        difficultyBack.gameObject.SetActive(false);
        stageBack.gameObject.SetActive(true);
        foreach (LockableButton button in stageButtons)
        {
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<Button>().enabled = true;
            int stageIndex = button.GetComponent<ButtonIndex>().GetIndex();
            if (stageIndex < PlayerData.STAGE_COUNT)
            {
                button.gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.SetActive(false);
                return;
            }
            int diffIndex = playerData.stages[currentCharacter, stageIndex];
            if (diffIndex > -1)
            {
                button.SetUnlocked(stageData[stageIndex].stageName);
            }
            else
            {
                button.SetLocked();
            }
        }
    }

    public void SetCurrentStage(int stage)
    {
        currentStage = stage;
        UpdateDifficultySelect();
    }

    public void UpdateDifficultySelect()
    {
        foreach (LockableButton button in stageButtons)
        {
            button.GetComponentInChildren<Button>().enabled = false;
            if (button.GetComponent<ButtonIndex>().GetIndex() != currentStage)
            {
                button.gameObject.SetActive(false);
            }
        }
        difficultyBack.gameObject.SetActive(true);
        stageBack.gameObject.SetActive(false);
        int diffIndex = playerData.stages[currentCharacter, currentStage];
        foreach (LockableButton button in difficultyButtons)
        {
            button.gameObject.SetActive(true);
            int index = button.GetComponent<ButtonIndex>().GetIndex();
            if (index <= diffIndex)
            {
                button.SetUnlocked("Difficulty " + (index + 1).ToString());
            }
            else
            {
                button.SetLocked();
            }
        }
    }

    public void SetCurrentDifficulty(int difficulty)
    {
        currentDifficulty = difficulty;
        UpdatePerkSelect();
    }

    public void UpdatePerkSelect()
    {
        UpdatePowerMeter();
        UpdatePerkLoadout();
        UpdateCharacterEmblem(perkEmblem, currentCharacter);
    }

    public void UpdatePowerMeter()
    {
        powerMeter.SetMeter(CheckLoadoutPower(), playerData.level[currentCharacter]);
    }

    public int CheckLoadoutPower()
    {
        return 0;
    }

    public void UpdatePerkLoadout()
    {
        for (int i = 0; i < 0; i++)
        {
            int slot = i / 2;
            PerkData[] slotPerks = new PerkData[12];
            switch (slot)
            {
                case 0:
                    slotPerks = classData[currentCharacter].normalAbility;
                    break;
                case 1:
                    slotPerks = classData[currentCharacter].specialAbility;
                    break;
                case 2:
                    slotPerks = classData[currentCharacter].chargedAbility;
                    break;
                case 3:
                    slotPerks = classData[currentCharacter].passiveAbilityA;
                    break;
                case 4:
                    slotPerks = classData[currentCharacter].passiveAbilityB;
                    break;
            }
            perkLoadout.perkButtons[i].ChangeColor(classData[currentCharacter].classColorDark);
            perkLoadout.perkButtons[i].SetPerk(slotPerks[loadoutData[currentCharacter, i]]);
        }
    }

    public void UpdatePerkSelection(int slotIndex)
    {
        int slot = slotIndex / 2;
        PerkData[] slotPerks = new PerkData[12];
        switch (slot)
        {
            case 0:
                slotPerks = classData[currentCharacter].normalAbility;
                break;
            case 1:
                slotPerks = classData[currentCharacter].specialAbility;
                break;
            case 2:
                slotPerks = classData[currentCharacter].chargedAbility;
                break;
            case 3:
                slotPerks = classData[currentCharacter].passiveAbilityA;
                break;
            case 4:
                slotPerks = classData[currentCharacter].passiveAbilityB;
                break;
        }
        for (int i = 0; i < 12; i++)
        {
            perkSelection.perkButtons[i].ChangeColor(classData[currentCharacter].classColorDark);
            perkSelection.perkButtons[i].SetPerk(slotPerks[i]);
        }
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