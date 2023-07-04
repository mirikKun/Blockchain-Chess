using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera characterCamera;


    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private TextMeshProUGUI roleText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI clanName;

    [SerializeField] private Image avatar;

    [SerializeField] private Image flag;
    [SerializeField] private Image[] colorTitles;
    [SerializeField] private TextMeshProUGUI[] textTitles;


    [SerializeField] private CharacterClanElements[] charactersClanElements;
    [SerializeField] private AchievementManager achievementManager;

    [SerializeField] private TextMeshProUGUI levelTotal;
    [SerializeField] private TextMeshProUGUI expTotal;
    [SerializeField] private TextMeshProUGUI expNeeded;

    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defence;
    [SerializeField] private TextMeshProUGUI energy;
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI dexterity;
    [SerializeField] private TextMeshProUGUI endurance;
    [SerializeField] private TextMeshProUGUI intelligence;
    [SerializeField] private TextMeshProUGUI totalStatPoints;
    [SerializeField] private TextMeshProUGUI totalMasteryPoints;


    [SerializeField] private Slider expSlider;


    [SerializeField] private AchievementGrader[] achievementGraders;

    // [SerializeField] private CharacterModelPreview characterModelPreview;
    [SerializeField] private CharacterModelObserver characterModelObserver;
    [SerializeField] private Sprite[] weaponSprites;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponName;
    
    [SerializeField] private Transform rewardsHolder;
    [SerializeField] private Reward expRewardPrefab;
    [SerializeField] private Reward statPointsRewardPrefab;
    [SerializeField] private Reward masteryPointsRewardPrefab;
    [SerializeField] private Slider masteryPointsSlider;
    public bool inMainMenu;
    private List<Reward> _rewards = new List<Reward>();
    private Character _curCharacter;
    private PlayerType _curPlayerType;
    private int _lastLevel;


    [Serializable]
    public class CharacterClanElements
    {
        public Sprite titleSprite;
        public Sprite flagSprite;
        public bool colorInverted;
    }

    private void OnEnable()
    {
        if (_curCharacter)
        {
            SetupCharacterScreen(_curCharacter, _curPlayerType);
        }

        if (mainCamera && characterCamera)
        {
            mainCamera.enabled = false;
            characterCamera.enabled = true;
        }

        _lastLevel = _curCharacter.characterProgress.Level;
    }

    private void OnDisable()
    {
        foreach (var reward in _rewards)
        {
            Destroy(reward.gameObject);
        }

        _rewards = new List<Reward>();
    }


    public void GoToCharacterScreen()
    {
        // mainCamera.enabled = false;
        //
        // if (_curCharacter)
        // {
        //     SetupCharacterScreen(_curCharacter, _curPlayerType);
        // }
        //
        // characterCamera.enabled = true;
    }

    public void GoOutOfCharacterScreen()
    {
        mainCamera.enabled = true;
        characterCamera.enabled = false;
        //  characterModelPreview.DeleteLastCharacter();
        characterModelObserver.DeleteLastCharacter();
    }

    public void ChooseCharacter(Character character, PlayerType playerType)
    {
        _curCharacter = character;
        _curPlayerType = playerType;
    }

    private void SetupCharacterScreen(Character character, PlayerType playerType)
    {
        weaponIcon.sprite = weaponSprites[(int)character.characterWeapon];
        weaponName.text= character.GetCharacterWeaponString().ToUpper();
        classText.text = character.GetCharacterClassString();
        roleText.text = character.characterRole.ToString();
        nameText.text = character.characterName;
        avatar.sprite = character.characterSprite;
        flag.sprite = charactersClanElements[(int)character.characterClan].flagSprite;

        // characterModelPreview.SetupCharacterModel(character.figurePrefab);

        characterModelObserver.SetupCharacterModel(character.figurePrefab);
        CharacterProgress curCharacterProgress;
        if (playerType == PlayerType.Player)
        {
            curCharacterProgress = character.characterProgress;
        }
        else
        {
            curCharacterProgress = character.enemyCharacterProgress;
        }


        UpdateLevel(curCharacterProgress, playerType);
        UpdateWeaponMasteryPoints();

        attack.text = curCharacterProgress.CurDamage + "/100";
        defence.text = curCharacterProgress.CurArmor + "/100";
        UpdateStats(curCharacterProgress);

        foreach (var colorTitle in colorTitles)
        {
            colorTitle.sprite = charactersClanElements[(int)character.characterClan].titleSprite;
        }

        foreach (var textTitle in textTitles)
        {
            if (charactersClanElements[(int)character.characterClan].colorInverted)
            {
                textTitle.color = Color.black;
            }
            else
            {
                textTitle.color = Color.white;
            }
        }

        clanName.text = character.characterClan.ToString().ToUpper() + " CLAN";

        UpdateRewards();

        UpdateAchievements();
    }

    public void UpdateAchievements()
    {
        var achievementDetails = achievementManager.GetAchievementDetails(_curCharacter);
      
        for (int i = 0; i < achievementGraders.Length; i++)
        {
            achievementGraders[i].AchievementsSetup(achievementDetails);
        }
    }

    public void UpdateLevel(CharacterProgress curCharacterProgress, PlayerType playerType)
    {
        if (playerType != PlayerType.Computer)
        {
            var level = 1;
            var experience = 0;

            experience = curCharacterProgress.Experience;


            level = curCharacterProgress.Level;


            levelTotal.text = "LEVEL " + level;
            expTotal.text = "TOTAL XP " + experience;
            int expLastEdge = curCharacterProgress.GetLevelEdge(level - 1);
            int expNextEdge = curCharacterProgress.GetLevelEdge(level);
            expNeeded.text = (expNextEdge - experience) + " NEEDED";
            expSlider.value = ((float)experience - expLastEdge) / (expNextEdge - expLastEdge);
        }
        else
        {
            levelTotal.text = "LEVEL" + 1;

            expTotal.text = "TOTAL XP " + 0;

            expNeeded.text = 0 + " NEEDED";
            expSlider.value = 1;
        }

        totalStatPoints.text = (curCharacterProgress.TotalStatPoints - curCharacterProgress.SpentStatPoints).ToString();
        totalMasteryPoints.text =
            (curCharacterProgress.TotalWeaponMasteryPoints - curCharacterProgress.SpentWeaponMasteryPoints).ToString();
    }

    private void UpdateRewards()
    {
        foreach (var reward in _curCharacter.characterProgress.expRewardsToBeClaimed)
        {
            Reward newReward = Instantiate(expRewardPrefab, rewardsHolder);
            newReward.SetupReward(_curCharacter, reward);
            _rewards.Add(newReward);
        }

        UpdatePointRewards();
    }


    private void UpdateWeaponMasteryPoints()
    {
        masteryPointsSlider.value = (float)(_curCharacter.characterProgress.CurFirstWeaponMasteryPoints - 1) /
                                    _curCharacter.characterProgress.MaxFirstWeaponMasteryPoints;
        totalMasteryPoints.text =
            (_curCharacter.characterProgress.TotalWeaponMasteryPoints -
             _curCharacter.characterProgress.SpentWeaponMasteryPoints).ToString();
    }

    public void IncreaseWeaponMasteryPoint()
    {
        if (!inMainMenu)
            return;
        CharacterProgress progress = _curCharacter.characterProgress;
        //progress.CheckForNewPoints();
        if (progress.TotalWeaponMasteryPoints - progress.SpentWeaponMasteryPoints <= 0 ||
            progress.CurFirstWeaponMasteryPoints >= progress.MaxFirstWeaponMasteryPoints)
        {
            return;
        }

        progress.SpentWeaponMasteryPoints++;
        progress.CurFirstWeaponMasteryPoints++;


        UpdateWeaponMasteryPoints();

        CharacterProgress.saveCharacterProgress(progress,
            "Character" + PlayerData.Account.AccountIndex + _curCharacter.index, PlayerData.Account.AccountIndex);
    }

    public void ClaimAchievement(Role achievementRole, Class achievementClass, int index, int reward)
    {
        achievementManager.SetAchievementsClaimedData(_curCharacter, achievementRole, achievementClass, index);
        _curCharacter.characterProgress.expRewardsToBeClaimed.Add(reward);
        var expPointsReward = reward;
        Reward newExpPointsReward = Instantiate(expRewardPrefab, rewardsHolder);
        newExpPointsReward.SetupReward(_curCharacter, expPointsReward);
        _rewards.Add(newExpPointsReward);
    }

    public void UpdatePointRewards()
    {
        for (var i = 0; i < _curCharacter.characterProgress.statPointRewardsToBeClaimed.Count; i++)
        {
            var reward = _curCharacter.characterProgress.statPointRewardsToBeClaimed[i];
            Reward newReward = Instantiate(statPointsRewardPrefab, rewardsHolder);
            newReward.SetupReward(_curCharacter, reward);
            _rewards.Add(newReward);
        }

        for (var i = 0; i < _curCharacter.characterProgress.masteryPointRewardsToBeClaimed.Count; i++)
        {
            var reward = _curCharacter.characterProgress.masteryPointRewardsToBeClaimed[i];
            Reward newReward = Instantiate(masteryPointsRewardPrefab, rewardsHolder);
            newReward.SetupReward(_curCharacter, reward);
            _rewards.Add(newReward);
        }
    }

    private void UpdateStats(CharacterProgress curCharacterProgress)
    {
        strength.text = curCharacterProgress.CurStrength.ToString();
        dexterity.text = curCharacterProgress.CurDexterity.ToString();
        endurance.text = curCharacterProgress.CurEndurance.ToString();
        intelligence.text = curCharacterProgress.CurIntelligence.ToString();
    }

    public void IncreaseStat(int statIndex)
    {
        if (!inMainMenu)
            return;
        CharacterProgress progress = _curCharacter.characterProgress;
        //progress.CheckForNewPoints();
        if (progress.TotalStatPoints - progress.SpentStatPoints <= 0)
        {
            return;
        }

        progress.SpentStatPoints++;
        if (statIndex == 0)
        {
            progress.CurStrength++;
        }
        else if (statIndex == 1)
        {
            progress.CurDexterity++;
        }
        else if (statIndex == 2)
        {
            progress.CurEndurance++;
        }
        else if (statIndex == 3)
        {
            progress.CurIntelligence++;
        }

        UpdateStats(progress);
        totalStatPoints.text = (progress.TotalStatPoints - progress.SpentStatPoints).ToString();

        CharacterProgress.saveCharacterProgress(progress,
            "Character" + PlayerData.Account.AccountIndex + _curCharacter.index, PlayerData.Account.AccountIndex);
    }
}