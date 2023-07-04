using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementGrader : MonoBehaviour
{
    [SerializeField] private AchievementsSet[] achievementsSets;

    [SerializeField] private bool changeParent;

    [Serializable]
    public class AchievementsSet
    {
        public AchievementHolder[] allAchievementHolders;

        public Transform inProgressHolder;
        public Transform completedHolder;
        public Transform inProgressEmptySpace;
        public Transform completedEmptySpace;
    }

    public void AchievementsSetup(AchievementDetails[] achievementDetailsArray)
    {
        int achievementIndex = 0;

        for (int j = 0; j < achievementsSets.Length; j++)
        {
            for (int i = 0; i < achievementsSets[j].allAchievementHolders.Length; i++)
            {
                if (changeParent)
                {
                    achievementsSets[j].allAchievementHolders[i].SetupAchievement(
                        achievementDetailsArray[achievementIndex],
                        achievementsSets[j].inProgressHolder, achievementsSets[j].completedHolder);
                }
                else
                {
                    achievementsSets[j].allAchievementHolders[i]
                        .SetupAchievement(achievementDetailsArray[achievementIndex]);
                }

                achievementIndex++;
            }

            achievementsSets[j].inProgressEmptySpace.SetAsLastSibling();
            achievementsSets[j].completedEmptySpace.SetAsLastSibling();
        }
    }
}