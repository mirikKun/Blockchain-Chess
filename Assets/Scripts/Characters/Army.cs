using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army 
{
    public figureMover pawn1;
    public figureMover pawn2;
    public figureMover pawn3;
    public figureMover pawn4;
    public figureMover pawn5;
    public figureMover pawn6;
    public figureMover pawn7;
    public figureMover pawn8;
    public figureMover rook1;
    public figureMover knight1;
    public figureMover bishop1;
    public figureMover queen;
    public figureMover king;
    public figureMover bishop2;
    public figureMover knight2;
    public figureMover rook2;

    public Character[] _characters;
    public void SetupArmy(Character[] newCharacters)
    {
        _characters = newCharacters;
        pawn1 = newCharacters[0].figurePrefab;
        pawn2 = newCharacters[1].figurePrefab;
        pawn3 = newCharacters[2].figurePrefab;
        pawn4 = newCharacters[3].figurePrefab;
        pawn5 = newCharacters[4].figurePrefab;
        pawn6 = newCharacters[5].figurePrefab;
        pawn7 = newCharacters[6].figurePrefab;
        pawn8 = newCharacters[7].figurePrefab;
        rook1 = newCharacters[8].figurePrefab;
        knight1 = newCharacters[9].figurePrefab;
        bishop1 = newCharacters[10].figurePrefab;
        queen = newCharacters[11].figurePrefab;
        king = newCharacters[12].figurePrefab;
        bishop2 = newCharacters[13].figurePrefab;
        knight2 = newCharacters[14].figurePrefab;
        rook2 = newCharacters[15].figurePrefab;
    }
    // public static Army GetArmyByIndexes(int[] indexes)
    // {
    //     if (indexes==null)
    //         return null;
    //     List<Character> charactersList = new List<Character>();
    //     foreach (var index in indexes)
    //     {
    //         foreach (var character in PlayerData.Characters)
    //         {
    //             if (character.index == index)
    //             {
    //                 charactersList.Add(character);
    //                 break;
    //             }
    //         }
    //     }
    //
    //     Army newArmy = new Army();
    //     newArmy.SetupArmy(charactersList.ToArray());
    //     return newArmy;
    // }

    public Character[] GetCharacters()
    {
        return _characters;
    }
    

}
