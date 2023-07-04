
using UnityEngine;
[CreateAssetMenu(fileName = "New character stats", menuName = "Character stats")]
public class CharacterStats: ScriptableObject
{
    public int Strength = 20;
    public int Dexterity = 20;
    public int Endurance = 20;
    public int Intelligence = 20;

    public int Armor = 10;
    public int Damage = 0;
}