using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    [SerializeField] private Image back;
    [SerializeField] private Sprite red;
    [SerializeField] private GameObject claimedBack;
    [SerializeField] private GameObject claimedButtonBack;
    [SerializeField] private TextMeshProUGUI rewardCount;
    [SerializeField] private RewardType rewardType;
    private Character _character;
    private int _reward;


    public enum RewardType
    {
        exp,
        statPoints,
        masteryPoints
    }

    public void SetupReward(Character newCharacter, int newReward)
    {
        _reward = newReward;
        _character = newCharacter;
        rewardCount.text = newReward.ToString();
    }

    public void ClaimReward()
    {
        CharacterPanel characterPanel = GetComponentInParent<CharacterPanel>();
        if (!characterPanel.inMainMenu)
        {
            return;
        }

        back.sprite = red;
        claimedBack.SetActive(true);
        claimedButtonBack.SetActive(true);

        if (rewardType == RewardType.exp)
        {
            ClaimExp(characterPanel);
        }
        else if (rewardType == RewardType.statPoints)
        {
            ClaimStatPoints();
        }
        else if (rewardType == RewardType.masteryPoints)
        {
            ClaimMasteryPoints();
        }

        characterPanel.UpdateLevel(_character.characterProgress, PlayerType.Player);

        CharacterProgress.saveCharacterProgress(_character.characterProgress,
            "Character" + PlayerData.Account.AccountIndex + _character.index, PlayerData.Account.AccountIndex);
    }

    private void ClaimExp(CharacterPanel characterPanel)
    {
        //ExperienceAccounter.GainExperience(_character, _reward);
        _character.AddExperience(_reward);

        for (int i = 0; i < _character.characterProgress.expRewardsToBeClaimed.Count; i++)
        {
            if (_character.characterProgress.expRewardsToBeClaimed[i] == _reward)
            {
                _character.characterProgress.expRewardsToBeClaimed.RemoveAt(i);
            }
        }

    }

    private void ClaimStatPoints()
    {
        _character.characterProgress.TotalStatPoints += _reward;
        for (int i = 0; i < _character.characterProgress.statPointRewardsToBeClaimed.Count; i++)
        {
            if (_character.characterProgress.statPointRewardsToBeClaimed[i] == _reward)
            {
                _character.characterProgress.statPointRewardsToBeClaimed.RemoveAt(i);
            }
        }
    }

    private void ClaimMasteryPoints()
    {
        _character.characterProgress.TotalWeaponMasteryPoints += _reward;

        for (int i = 0; i < _character.characterProgress.statPointRewardsToBeClaimed.Count; i++)
        {
            if (_character.characterProgress.statPointRewardsToBeClaimed[i] == _reward)
            {
                _character.characterProgress.statPointRewardsToBeClaimed.RemoveAt(i);
            }
        }
    }
}