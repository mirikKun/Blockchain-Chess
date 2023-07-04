using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using BayatGames.SaveGameFree;
using Characters;
using UnityEngine;

[Serializable]
public class CharacterProgress
{
    public int Experience = 0;
    public int Level = 1;

    public List<int> expRewardsToBeClaimed = new List<int>();
    public List<int> statPointRewardsToBeClaimed = new List<int>();
    public List<int> masteryPointRewardsToBeClaimed = new List<int>();
    public int TotalStatPoints = 0;
    public int SpentStatPoints = 0;
    public int TotalWeaponMasteryPoints = 0;
    public int SpentWeaponMasteryPoints = 0;

    public int CurStrength = 20;
    public int CurDexterity = 20;
    public int CurEndurance = 20;
    public int CurIntelligence = 20;

    public int CurArmor = 10;
    public int CurDamage = 0;

    public int CurFirstWeaponMasteryPoints = 1;
    public int MaxFirstWeaponMasteryPoints = 10;

    // public int TotalBattles = 0;
    // public int TotalBattlesWon = 0;
    // public int TotalBattlesLost = 0;
    // public int TotalKills = 0;
    // public int TotalDeaths = 0;

    public List<int> KillsPerGame = new List<int>();

    public Dictionary<Role, Dictionary<Class, int>> KillsData = CreateRoleKillsData();
    public Dictionary<Role, Dictionary<Class, bool[]>> KillsClaimedRewardsData=CreateRoleClaimedRewardsData();


    private static Dictionary<Role, Dictionary<Class, int>> CreateRoleKillsData()
    {
        return Enum.GetValues(typeof(Role))
            .Cast<Role>()
            .ToDictionary(t => t, t => CreateClassKillsData());
    }

    private static Dictionary<Class, int> CreateClassKillsData()
    {
        return Enum.GetValues(typeof(Class))
            .Cast<Class>()
            .ToDictionary(t => t, t => 0);
    }
    private static Dictionary<Role, Dictionary<Class, bool[]>> CreateRoleClaimedRewardsData()
    {
        return Enum.GetValues(typeof(Role))
            .Cast<Role>()
            .ToDictionary(t => t, t => CreateClassClaimedRewardsData());
    }

    private static Dictionary<Class, bool[]> CreateClassClaimedRewardsData()
    {
        return Enum.GetValues(typeof(Class))
            .Cast<Class>()
            .ToDictionary(t => t, t => new bool[8]);
    }

    private void ListsCheck()
    {
        expRewardsToBeClaimed ??= new List<int>();
        statPointRewardsToBeClaimed ??= new List<int>();
        masteryPointRewardsToBeClaimed ??= new List<int>();
    }

    public int GetLevelEdge(int curLevel)
    {
        return _expEdges[curLevel];
    }

    private readonly int[] _expEdges = new[]
    {
        0, 1000, 2500, 5000, 7500, 10000, 15000, 20000, 25000, 30000, 35000, 40000, 50000, 60000, 70000, 80000, 90000
    };

    public void CheckForNewPoints()
    {
        int statPointsToBeClaimed = 0;
        for (int i = 0; i < statPointRewardsToBeClaimed.Count; i++)
        {
            statPointsToBeClaimed += statPointRewardsToBeClaimed[i];
        }

        if ((Level - 1) * 5 - statPointsToBeClaimed > TotalStatPoints)
        {
            statPointRewardsToBeClaimed.Add((Level - 1) * 5 - statPointsToBeClaimed);
        }
        //TotalStatPoints = (Level - 1) * 5;


        int masteryPointsToBeClaimed = 0;
        for (int i = 0; i < masteryPointRewardsToBeClaimed.Count; i++)
        {
            masteryPointsToBeClaimed += masteryPointRewardsToBeClaimed[i];
        }

        if ((Level - 1) - masteryPointsToBeClaimed > TotalWeaponMasteryPoints)
        {
            masteryPointRewardsToBeClaimed.Add((Level - 1) - masteryPointsToBeClaimed);
        }
        //TotalWeaponMasteryPoints = Level - 1;
    }

    public void SetupLevel()
    {
        int curExperience = Experience;
        int newLevel = 0;
        foreach (var expEdge in _expEdges)
        {
            if (curExperience >= expEdge)
            {
                newLevel++;
            }
            else
            {
                break;
            }
        }

        CheckNewLevel(newLevel);

        Level = newLevel;
    }

    public void CheckNewLevel(int newLevel)
    {
        if (newLevel > Level)
        {
            TotalStatPoints += (newLevel - Level) * 5;
            TotalWeaponMasteryPoints += newLevel - Level;
        }
    }

    public static void saveCharacterProgress(CharacterProgress modelData, string fileName, int accountIndex)
    {
        SaveGame.Save(fileName, modelData);
    }

    public static bool HaveFile(string fileName, int accountIndex)
    {
        return File.Exists(GetFileLocation(accountIndex) + fileName);
    }

    public static string GetFileLocation(int accountIndex)
    {
        if (!File.Exists(Application.persistentDataPath + "/Characters" + accountIndex))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Characters" + accountIndex);
        }

        return Application.persistentDataPath + "/Characters" + accountIndex + "/";
    }

    public static CharacterProgress getCharacterProgress(string fileName, int accountIndex)
    {
        if (SaveGame.Exists(fileName))
        {
            return SaveGame.Load(fileName, new CharacterProgress());
        }

        return new CharacterProgress();
    }


    public static void removeCharacterProgress(string fileName, int accountIndex)
    {
        if (SaveGame.IsFilePath(fileName))
        {
            SaveGame.Delete(fileName);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte[] Serialize(object obj)
    {
        if (obj == null)
            return null;
        MemoryStream ms = new MemoryStream();
        SaveGame.Serializer.Serialize(obj,ms,Encoding.Default );
        return ms.ToArray();
    }

    //
    public static object Deserialize(byte[] data)
    {

        MemoryStream memoryStream = new MemoryStream(data);
        object obj = SaveGame.Serializer.Deserialize<CharacterProgress>(memoryStream,Encoding.Default);

        return obj;
    }
}