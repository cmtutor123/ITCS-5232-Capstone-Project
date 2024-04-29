using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool debug = true;

    public ProjectileSpriteManager projectileSpriteManager;
    public Camera cam;

    public LayerMask layerMaskSightBlock;

    public static GameManager instance;
    public List<MatchRoom> matchRooms;
    public bool loadedData = false;
    public PlayerData playerData;
    public List<MenuState> menuStates;
    public float fadeTime = 0.5f;

    public List<ClassData> classData;
    public List<StageData> stageData;
    public List<TileGenerator> tileGenerators;

    public GameObject prefabPlayerProjectile;
    public GameObject prefabProjectileSlash, prefabProjectileCircle, prefabProjectileRectangle;

    public int currentCharacter, currentStage, currentDifficulty, currentSlot;

    public GameObject roomObjectPrefab;

    public PerkData perkNone;
    public Sprite perkLocked;

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
    public PerkDescription perkTooltip;
    public GameObject matchStartButton;
    [Header("Loadout Information")]
    public int[,] loadoutData = new int[PlayerData.CLASS_COUNT, 10];
    public string[] loadoutSlots = { "NormalMain", "NormalMod", "SpecialMain", "SpecialMod", "ChargedMain", "ChargedMod", "PassiveA1", "PassiveA2", "PassiveB1", "PassiveB1" };
    public int[] loadoutDefaults = { 0, -1, 0, -1, 0, -1, -1, -1, -1, -1 };

    public List<MatchEnemy> aliveEnemies = new List<MatchEnemy>();
    public MatchPlayer matchPlayer;
    public GameObject prefabMatchPlayer;
    public GameObject prefabMatchEnemy;
    public List<BaseStats> baseStats;

    public float spawnDelay = 2.5f;

    public List<MatchEnemy> matchEnemies = new List<MatchEnemy>();

    public PathfindingHelper pathfindingHelper;

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
        LoadLoadoutData();
        InitializeButtonIndex();
        SetMenu(MenuState.MainMenu);
    }

    void Update()
    {
        if (matchPlayer != null && cam != null)
        {
            cam.transform.position = matchPlayer.transform.position + new Vector3(0, 0, -10);
        }
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
        SaveLoadoutData();
    }

    public void SaveLoadoutData()
    {
        CheckLoadoutData();
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
            CheckLoadoutPerk(c, 0, loadoutData[c, 0], 0, false);
            CheckLoadoutPerk(c, 1, loadoutData[c, 1], 0, true, true);
            CheckLoadoutPerk(c, 2, loadoutData[c, 2], 12, false);
            CheckLoadoutPerk(c, 3, loadoutData[c, 3], 12, true, true);
            CheckLoadoutPerk(c, 4, loadoutData[c, 4], 24, false);
            CheckLoadoutPerk(c, 5, loadoutData[c, 5], 24, true, true);
            CheckLoadoutPerk(c, 6, loadoutData[c, 6], 36);
            CheckLoadoutPerk(c, 7, loadoutData[c, 7], 36);
            CheckLoadoutPerk(c, 8, loadoutData[c, 8], 48);
            CheckLoadoutPerk(c, 9, loadoutData[c, 9], 48);
        }
    }

    public void CheckLoadoutPerk(int cIndex, int sIndex, int pIndex, int pOffset, bool canBeEmpty = true, bool isModSlot = false)
    {
        if (pIndex == -1)
        {
            if (!canBeEmpty)
            {
                loadoutData[cIndex, sIndex] = 0;
            }
        }
        else if (playerData.perks[cIndex, pIndex + pOffset] != 1)
        {
            loadoutData[cIndex, sIndex] = canBeEmpty ? -1 : 0;
        }
        else
        {
            if (isModSlot)
            {
                int modNumber = pIndex % 4;
                int modBase = pIndex - modNumber;
                if (loadoutData[cIndex, modBase + pOffset] != modBase)
                {
                    loadoutData[cIndex, sIndex] = -1;
                }
                if (modNumber == 0)
                {
                    loadoutData[cIndex, sIndex] = -1;
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
            abilityBase = (perkIndex / 4) * 4;
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
        for (float t = 0; t < 1; t += Time.unscaledDeltaTime / seconds)
        {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, Mathf.Lerp(0, 1, t));
            yield return null;
        }
        fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);
    }

    public IEnumerator FadeIn(float seconds)
    {
        for (float t = 0; t < 1; t += Time.unscaledDeltaTime / seconds)
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
            PerkTooltip perkTooltip = button.gameObject.AddComponent<PerkTooltip>();
            perkTooltip.UpdatePerk(perkNone);
        }
        index = 0;
        foreach (PerkButton button in perkSelection.perkButtons)
        {
            ButtonIndex buttonIndex = button.gameObject.AddComponent<ButtonIndex>();
            buttonIndex.SetIndex(index++);
            button.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() => gameObject.GetComponent<ButtonManager>().ButtonPerkSelection(buttonIndex.GetIndex()));
            PerkTooltip perkTooltip = button.gameObject.AddComponent<PerkTooltip>();
            perkTooltip.UpdatePerk(perkNone);
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
        powerMeter.SetPoints(playerData.points[currentCharacter]);
        powerMeter.SetMeter(CheckLoadoutPower(), playerData.level[currentCharacter]);
    }

    public int CheckLoadoutPower()
    {
        int total = 0;
        for (int i = 0; i < 10; i++)
        {
            int pIndex = loadoutData[currentCharacter, i];
            if (pIndex != -1)
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
                total += slotPerks[pIndex].cost;
            }
        }
        if (total <= playerData.level[currentCharacter])
        {
            matchStartButton.SetActive(true);
        }
        else
        {
            matchStartButton.SetActive(false);
        }
        return total;
    }

    public void UpdatePerkLoadout()
    {
        perkSelection.gameObject.SetActive(false);
        for (int i = 0; i < 10; i++)
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
            if (loadoutData[currentCharacter, i] == -1)
            {
                perkLoadout.perkButtons[i].SetPerk(perkNone);
                perkLoadout.perkButtons[i].GetComponent<PerkTooltip>().UpdatePerk(perkNone);
            }
            else
            {
                perkLoadout.perkButtons[i].SetPerk(slotPerks[loadoutData[currentCharacter, i]]);
                perkLoadout.perkButtons[i].GetComponent<PerkTooltip>().UpdatePerk(slotPerks[loadoutData[currentCharacter, i]]);
            }
        }
    }

    public void UpdatePerkSelection(int slotIndex)
    {
        currentSlot = slotIndex;
        perkSelection.gameObject.SetActive(true);
        int slot = slotIndex / 2;
        PerkData[] slotPerks = new PerkData[12];
        int perkOffset = 0;
        switch (slot)
        {
            case 0:
                slotPerks = classData[currentCharacter].normalAbility;
                break;
            case 1:
                slotPerks = classData[currentCharacter].specialAbility;
                perkOffset = 12;
                break;
            case 2:
                slotPerks = classData[currentCharacter].chargedAbility;
                perkOffset = 24;
                break;
            case 3:
                slotPerks = classData[currentCharacter].passiveAbilityA;
                perkOffset = 36;
                break;
            case 4:
                slotPerks = classData[currentCharacter].passiveAbilityB;
                perkOffset = 48;
                break;
        }
        for (int i = 0; i < 12; i++)
        {
            perkSelection.perkButtons[i].ChangeColor(classData[currentCharacter].classColorDark);
            perkSelection.perkButtons[i].SetPerk(slotPerks[i]);
            perkSelection.perkButtons[i].GetComponent<PerkTooltip>().UpdatePerk(slotPerks[i]);
            if (playerData.perks[currentCharacter, perkOffset + i] != 1)
            {
                perkSelection.perkButtons[i].ChangeIcon(perkLocked);
            }
        }
    }

    public void ShowPerkTooltip(PerkData perk)
    {
        perkTooltip.gameObject.SetActive(true);
        perkTooltip.SetPerk(perk);
    }

    public void HidePerkTooltip()
    {
        perkTooltip.gameObject.SetActive(false);
    }

    public int GetPoints()
    {
        return playerData.points[currentCharacter];
    }    

    public bool PerkLocked(int slot, int index)
    {
        int pIndex = 12 * slot + index;
        return playerData.perks[currentCharacter, pIndex] != 1;
    }

    public void UnlockPerk(int slot, int index)
    {
        int pIndex = 12 * slot + index;
        playerData.perks[currentCharacter, pIndex] = 1;
        FileManager.SavePlayerData(playerData);
    }

    public void StartGame()
    {
        LoadStage();
        LoadPlayer();
        SetMenu(MenuState.Match);
    }

    public void LoadStage()
    {
        StageData currentStageData = stageData[currentStage];
        int roomCount = currentStageData.roomCount;
        int powerLevel = 0;
        if (currentStageData.powerLevels.Length > 0)
        {
            powerLevel = currentStageData.powerLevels[currentStageData.powerLevels.Length - 1];
        }
        if (currentStageData.powerLevels.Length > currentDifficulty)
        {
            powerLevel = currentStageData.powerLevels[currentDifficulty];
        }
        List<EnemyData[]> stageEnemyData = new List<EnemyData[]>();
        stageEnemyData.Add(currentStageData.normalEnemies);
        stageEnemyData.Add(currentStageData.strongEnemies);
        stageEnemyData.Add(currentStageData.minibossEnemies);
        int[] rooms = new int[roomCount];
        List<EnemyData>[] roomEnemies = new List<EnemyData>[roomCount];
        for (int i = 0; i < roomCount; i++)
        {
            roomEnemies[i] = new List<EnemyData>();
        }
        int remainingPower = powerLevel;
        while (remainingPower > 0)
        {
            int lowest = GetLowestIndex(rooms);
            EnemyData enemy = GetRandomEnemy(powerLevel, stageEnemyData);
            roomEnemies[lowest].Add(enemy);
            rooms[lowest] += enemy.enemyLevel;
            remainingPower -= enemy.enemyLevel;
        }
        (int[], List<EnemyData>[]) reorderedRooms = ReorderRooms(rooms, roomEnemies);
        rooms = reorderedRooms.Item1;
        roomEnemies = reorderedRooms.Item2;
        RoomData[] roomData = new RoomData[roomCount + 2];
        RoomData startingRoom = new RoomData();
        startingRoom.SetStartRoom();
        roomData[0] = startingRoom;
        RoomData bossRoom = new RoomData();
        bossRoom.SetBossRoom();
        bossRoom.SetBossEnemy(currentStageData.bossEnemies);
        roomData[roomCount + 1] = bossRoom;
        for (int i = 0; i < roomCount; i++)
        {
            RoomData currentRoomData = new RoomData();
            currentRoomData.index = i;
            currentRoomData.SetRoomShape(rooms[i]);
            currentRoomData.SetRoomEnemies(roomEnemies[i]);
            roomData[i + 1] = currentRoomData;
        }
        for (int i = 1; i < roomData.Length; i++)
        {
            (int, int) lastRoomOffset = roomData[i - 1].roomOffset;
            (int, int) lastExitOffset = roomData[i - 1].nextOffset;
            (int, int) currentOffset = (lastRoomOffset.Item1 + lastExitOffset.Item1, lastRoomOffset.Item2 + lastExitOffset.Item2);
            roomData[i].roomOffset = currentOffset;
        }
        foreach (RoomData room in roomData)
        {
            (int, int) roomOffset = room.roomOffset;
            foreach (TileData tile in room.tileData)
            {
                tile.AddRoomOffset(roomOffset);
            }
        }
        GenerateStage(roomData);
        //
    }

    public void GenerateStage(RoomData[] roomData)
    {
        matchRooms = new List<MatchRoom>();
        TileGenerator generator = tileGenerators[currentStage];
        foreach (RoomData data in roomData)
        {
            matchRooms.Add(new MatchRoom(data, generator));
        }
        foreach (MatchRoom room in matchRooms)
        {
            room.GenerateRoom();
            foreach (TileData tile in room.roomData.tileData)
            {
                pathfindingHelper.AddTile(tile);
            }
        }
    }

    public int GetLowestIndex(int[] array)
    {
        int lowest = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < array[lowest])
            {
                lowest = i;
            }
        }
        return lowest;
    }

    public EnemyData GetRandomEnemy(int powerLevel, List<EnemyData[]> enemies)
    {
        float weightTotal = 0;
        foreach (EnemyData[] array in enemies)
        {
            foreach (EnemyData data in array)
            {
                float level = data.enemyLevel;
                float weight = 1.0f / level;
                weightTotal += weight;
            }
        }
        float random = Random.Range(0, weightTotal);
        foreach (EnemyData[] array in enemies)
        {
            foreach (EnemyData data in array)
            {
                float level = data.enemyLevel;
                float weight = 1.0f / level;
                random -= weight;
                if (random <= 0)
                {
                    return data;
                }
            }
        }
        return enemies[enemies.Count - 1][enemies[enemies.Count - 1].Length - 1];
    }
    
    public (int[], List<EnemyData>[]) ReorderRooms(int[] rooms, List<EnemyData>[] roomEnemies)
    {
        for (int i = 0; i < rooms.Length - 1; i++)
        {
            for (int j = 0; j < rooms.Length - i - 1; j++)
            {
                if (rooms[j] > rooms[j + 1])
                {
                    int tempInt = rooms[j + 1];
                    List<EnemyData> tempData = roomEnemies[j + 1];
                    rooms[j + 1] = rooms[j];
                    roomEnemies[j + 1] = roomEnemies[j];
                    rooms[j] = tempInt;
                    roomEnemies[j] = tempData;
                }
            }
        }
        return (rooms, roomEnemies);
    }

    public void LoadPlayer()
    {
        matchPlayer = Instantiate(prefabMatchPlayer).GetComponent<MatchPlayer>();
        for (int i = 0; i < 10; i++)
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
            if (loadoutData[currentCharacter, i] != -1)
            {
                matchPlayer.AddPerk(slotPerks[loadoutData[currentCharacter, i]].perkId);
            }
        }
        matchPlayer.ProcessPerks();
    }

    public void TryAbility(AbilityType ability)
    {
        if (matchPlayer != null)
        {
            matchPlayer.TriggerAbilityUse(ability);
        }
    }

    public void TryStartCharge(AbilityType ability)
    {
        if (matchPlayer != null)
        {
            matchPlayer.TriggerChargeStart(ability);
        }
    }

    public void TryEndCharge(AbilityType ability)
    {
        if (matchPlayer != null)
        {
            matchPlayer.TriggerChargeEnd(ability);
        }
    }

    public void TryDash()
    {
        if (matchPlayer != null)
        {
            matchPlayer.TriggerDash();
        }
    }

    public void TryMove(Vector2 direction)
    {
        if (matchPlayer != null)
        {
            matchPlayer.TriggerMove(direction);
        }
    }

    bool pauseToggle = true;

    public void TryPause()
    {
        if (!pauseToggle) return;
        pauseToggle = false;
        if (GetMenu() == MenuState.Match)
        {
            Time.timeScale = 0;
            SwitchMenu(MenuState.Pause);
        }
        else if (GetMenu() == MenuState.Pause)
        {
            RemoveMenu();
            Time.timeScale = 1;
        }
        pauseToggle = true;
    }

    public GameObject GetProjectilePrefab(ProjectileShape shape)
    {
        switch (shape)
        {
            case ProjectileShape.Slash:
                return prefabProjectileSlash;
            case ProjectileShape.Circle:
                return prefabProjectileCircle;
            case ProjectileShape.Rectangle:
                return prefabProjectileRectangle;
        }
        return null;
    }

    public float HomingDistance(float moveSpeed, float rotationSpeed, Vector2 direction, Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 toTarget = endPoint - startPoint;
        float angle = Vector2.SignedAngle(direction, toTarget);
        bool isLeft = angle > 0;
        float radius = moveSpeed / rotationSpeed;
        Vector2 center = startPoint + Vector2.Perpendicular(direction) * radius * (isLeft ? -1 : 1);
        Vector2 toCenter = center - endPoint;
        if (toCenter.magnitude <= radius)
        {
            return float.PositiveInfinity;
        }
        float distanceToCenter = toCenter.magnitude;
        float tangentDistance = Mathf.Sqrt(distanceToCenter * distanceToCenter - radius * radius);
        float rotation = Mathf.PI * 2 * rotationSpeed;
        Vector2 tangentDirection = RotateVector(toCenter, rotation);
        Vector2 tangentPoint = endPoint + (tangentDirection.normalized * tangentDistance);
        float arcDistance = Mathf.PI * 2 * radius * Vector2.Angle(startPoint - center, tangentPoint - center) / 360f;
        float totalDistance = arcDistance + (endPoint - tangentPoint).magnitude;
        return totalDistance;
    }

    public Vector2 RotateVector(Vector2 v, float rotation)
    {
        return new Vector2(v.x * Mathf.Cos(rotation) - v.y * Mathf.Sin(rotation), v.x * Mathf.Sin(rotation) + v.y * Mathf.Cos(rotation));
    }

    public List<MatchEnemy> EnemiesInRange(Vector2 point, float range)
    {
        List<MatchEnemy> inRange = new List<MatchEnemy>();
        foreach (MatchEnemy enemy in aliveEnemies)
        {
            if (enemy != null && (point - (Vector2)enemy.transform.position).magnitude <= range)
            {
                inRange.Add(enemy);
            }    
        }
        return inRange;
    }

    public BaseStats GetBaseStats()
    {
        return baseStats[currentCharacter];
    }

    public int GetDifficultyLevel()
    {
        return stageData[currentStage].enemyLevels[currentDifficulty];
    }

    public void TriggerWave(int wave)
    {
        matchRooms[wave + 1].SpawnWave(wave);
    }

    public void RegisterEnemy(MatchEnemy enemy)
    {
        matchEnemies.Add(enemy);
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

public enum AbilityType
{
    Normal,
    Special,
    Charged
}