using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CharacterModelPreview : MonoBehaviour
{
    [SerializeField] private Transform spawnPlace;
    private Vector3 _startCharacterRotation=new Vector3(0,0,0);
    [SerializeField] private Transform model;
    
    public void SetupCharacterModel(figureMover characterModel)
    {
        DeleteLastCharacter();
        model=Instantiate(characterModel.transform, spawnPlace.position, quaternion.Euler(_startCharacterRotation), spawnPlace);
        model.localEulerAngles = _startCharacterRotation;
    }

    public void DeleteLastCharacter()
    {
        if (model)
        {
            Destroy(model.gameObject);
        } 
    }
    


}
