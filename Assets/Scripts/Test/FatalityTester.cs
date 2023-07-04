using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FatalityTester : MonoBehaviour
{
    [SerializeField] private FatalityController fatalityController;
    [SerializeField] private figureMover[] allFigurePrefabs;
    [SerializeField] private Dropdown attackerPrefabsDropDown;
    [SerializeField] private Dropdown defenderPrefabsDropDown;
    private SecondScenePositionController _scenePositionController;
    private void Start()
    {
        _scenePositionController = FindObjectOfType<SecondScenePositionController>();
        List<string> figurePrefabNames=new List<string>();
        foreach (var figurePrefab in allFigurePrefabs)
        {
            figurePrefabNames.Add(figurePrefab.gameObject.name);
                
        }        
        attackerPrefabsDropDown.AddOptions(figurePrefabNames);
        defenderPrefabsDropDown.AddOptions(figurePrefabNames);
    }

    public void TestCurAnimations()
    {
        if(_scenePositionController)
         _scenePositionController.GenerateFatalityData();
        figureMover _attackerFigure=allFigurePrefabs[attackerPrefabsDropDown.value];
        figureMover _targetFigure=allFigurePrefabs[defenderPrefabsDropDown.value];
        FigureFatality[] _fatalitys;
        List<FigureFatality> _curFigureFatalityAnims = new List<FigureFatality>();
        _fatalitys = _attackerFigure.GetComponents<FigureFatality>();
        int _randomAnimIndex;
        
        if (_curFigureFatalityAnims.Count == 0)
        {
            foreach (var fatality in _fatalitys)
            {
                if (!fatality.FatalityName.Contains(_attackerFigure.figureType.ToString())||fatality.FatalityName=="KnightDeath"||fatality.FatalityName=="KnightDeathFromKnightFatality") continue;
                _curFigureFatalityAnims.Add(fatality);
            }
        }
        _randomAnimIndex = Random.Range(0, _curFigureFatalityAnims.Count);

        if (_targetFigure.figureType == figureMover.FigureType.Knight)
        {
            if (_attackerFigure.figureType == figureMover.FigureType.Knight)
            {
                    
                fatalityController.StartPutFigure(_attackerFigure, _targetFigure,
                    "KnightDeathFromKnightFatality");

            }
            else
            {
                fatalityController.StartPutFigure(_attackerFigure, _targetFigure,
                    "KnightDeath");

            }
        }
        else
        {
            fatalityController.StartPutFigure(_attackerFigure, _targetFigure,
                _curFigureFatalityAnims[_randomAnimIndex].FatalityName);

        }

    }
}