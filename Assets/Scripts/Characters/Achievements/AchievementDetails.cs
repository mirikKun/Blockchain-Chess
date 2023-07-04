using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
[CreateAssetMenu(fileName = "New achievement", menuName = "Achievement")]

public class AchievementDetails : ScriptableObject
{
    public int id;
    public string achievementName;
    public AchievementStep[] achievementSteps;

    public Class classOfKilled;
    public Role roleOfKilled;
    private int _curValue;
    public bool inOneGame;

    public void SetCurValue(int value)
    {
        _curValue = value;
    }

    public void SetRewardClaimed(bool[] steps)
    {
        for (int i = 0; i < achievementSteps.Length; i++)
        {
            achievementSteps[i].claimed = steps[i];
        }
    }
    public int GetCurValue()
    {
        return _curValue;
    }
    [Serializable]
    public class AchievementStep
    {
        public string description;
        public int goal;
        public int expReward;
        public bool claimed;
    }
}
