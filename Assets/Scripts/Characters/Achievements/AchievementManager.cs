using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private AchievementDetails[] achievementList;


    public AchievementDetails[] GetAchievementDetails(Character character)
    {
        foreach (var achievementDetail in achievementList)
        {
            if (achievementDetail.inOneGame)
            {
                achievementDetail.SetCurValue(character.characterProgress.KillsData
                    [achievementDetail.roleOfKilled][achievementDetail.classOfKilled]);
                achievementDetail.SetRewardClaimed(character.characterProgress.KillsClaimedRewardsData
                    [achievementDetail.roleOfKilled][achievementDetail.classOfKilled]);
            }
            else
            {
                achievementDetail.SetCurValue(character.characterProgress.KillsData
                    [achievementDetail.roleOfKilled][Class.None]);
                achievementDetail.SetRewardClaimed(character.characterProgress.KillsClaimedRewardsData
                    [achievementDetail.roleOfKilled][Class.None]);
            }
        }

        return achievementList;
    }
    public void SetAchievementsClaimedData(Character character,Role achievementRole, Class achievementClass,int index )
    {
        character.characterProgress.KillsClaimedRewardsData[achievementRole][achievementClass][index]=true;

    }

    public void CheckKillAchievement(figureMover killerFigure, Character victim)
    {
        Character killer = killerFigure.thisCharacter;
        // killer.characterDetails.victims.Add(victim);


        killer.characterProgress.KillsPerGame.Add((int) victim.characterClass);

        AchievementDetails curAchievementOneGameDetails =
            FindAchievementDetails(victim.characterRole, victim.characterClass);
        AchievementDetails curAchievementDetails = FindAchievementDetails(victim.characterRole, Class.None);

        int count = killer.characterProgress.KillsPerGame.Count(x =>
            x == (int) victim.characterClass);
        if (count > killer.characterProgress.KillsData[victim.characterRole][victim.characterClass])
        {
            killer.characterProgress.KillsData[victim.characterRole][victim.characterClass]++;
           // CheckKillAchievementOneGameThreshold(killerFigure, curAchievementOneGameDetails);
        }

        curAchievementOneGameDetails.SetCurValue(
            killer.characterProgress.KillsData[victim.characterRole][victim.characterClass]);


        curAchievementDetails.SetCurValue(killer.characterProgress.KillsData[victim.characterRole][Class.None]);


        killer.characterProgress.KillsData[victim.characterRole][Class.None]++;
        //CheckKillAchievementThreshold(killerFigure, curAchievementDetails);
    }

    /// <summary>
    /// Checks if the achievement has been received
    /// </summary>
    /// <param name="character"></param>
    /// <param name="achievementDetails"></param>
    /// <param name="achievementValue"></param>
    private void CheckKillAchievementThreshold(figureMover killerFigure, AchievementDetails achievementDetails)
    {
        Character character = killerFigure.thisCharacter;
        int index = 0;
        foreach (var achievementStep in achievementDetails.achievementSteps)
        {

            if (achievementStep.goal ==
                character.characterProgress.KillsData[achievementDetails.roleOfKilled][Class.None])
            {

                killerFigure.thisCharacter.characterProgress.expRewardsToBeClaimed.Add(achievementStep.expReward);
            }

            index++;
        }
    }

    private void CheckKillAchievementOneGameThreshold(figureMover killerFigure, AchievementDetails achievementDetails)
    {
        Character character = killerFigure.thisCharacter;
        int index = 0;
        foreach (var achievementStep in achievementDetails.achievementSteps)
        {
            if (achievementStep.goal ==
                character.characterProgress.KillsData[achievementDetails.roleOfKilled][achievementDetails.classOfKilled])
            {
                killerFigure.thisCharacter.characterProgress.expRewardsToBeClaimed.Add(achievementStep.expReward);
            }

            index++;
        }
    }

    /// <summary>
    /// Finds achievement details by key
    /// </summary>
    /// <param name="key">Key word</param>
    /// <returns>Details </returns>
    private AchievementDetails FindAchievementDetails(Role curRole, Class curClass)
    {
        foreach (var achievement in achievementList)
        {
            if (achievement.roleOfKilled == curRole && achievement.classOfKilled == curClass)
            {
                return achievement;
            }
        }

        Debug.Log("12312414");
        return null;
    }
}