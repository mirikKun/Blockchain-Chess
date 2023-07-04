using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SecondScenePositionController : MonoBehaviour
{
    [SerializeField] private FatalityPositionData[] secondSceneFatalitiesData;

    [Serializable]
    public class FatalityPositionData
    {
        public Transform positionTransform;
        public int defaultYRotation;
        public bool cantBeRotated;
    }

    private List<FatalityData> _fatalityHistory = new List<FatalityData>();

    [System.Serializable]
    public struct FatalityData
    {
        public int fatalityPlaceIndex;
        public int fatalityRotation;

        public FatalityData(int fatalityPlaceIndex, int fatalityRotation)
        {
            this.fatalityPlaceIndex = fatalityPlaceIndex;
            this.fatalityRotation = fatalityRotation;
        }
    }

    public FatalityData GenerateFatalityData()
    {
        int randomPositionNumber = Random.Range(0, secondSceneFatalitiesData.Length);
        int randomRotation;
        if (secondSceneFatalitiesData[randomPositionNumber].cantBeRotated)
        {
            randomRotation = secondSceneFatalitiesData[randomPositionNumber].defaultYRotation;
        }
        else
        {
            randomRotation = Random.Range(0, 360);
        }

        FatalityData newFatalityData = new FatalityData(randomPositionNumber, randomRotation);
        _fatalityHistory.Add(newFatalityData);
        return newFatalityData;
    }

    public void SetDefaultRotation()
    {
        int randomPositionNumber = _fatalityHistory[_fatalityHistory.Count - 1].fatalityPlaceIndex;
        int randomRotation = secondSceneFatalitiesData[randomPositionNumber].defaultYRotation;
        FatalityData newFatalityData = new FatalityData(randomPositionNumber, randomRotation);
        _fatalityHistory[_fatalityHistory.Count - 1] = newFatalityData;
    }

    public void SetFatalityData(int fatalityPlaceIndex, int fatalityRotation)
    {
        int randomPositionNumber = fatalityPlaceIndex;
        int randomRotation = fatalityRotation;

        FatalityData newFatalityData = new FatalityData(randomPositionNumber, randomRotation);
        _fatalityHistory.Add(newFatalityData);
    }

    public void SetFatalityDataByIndex(int fatalityIndex)
    {
        int randomPositionNumber = _fatalityHistory[fatalityIndex].fatalityPlaceIndex;
        int randomRotation = _fatalityHistory[fatalityIndex].fatalityRotation;
        FatalityData newFatalityData = new FatalityData(randomPositionNumber, randomRotation);
        _fatalityHistory.Add(newFatalityData);
    }

    public void ChangePosition()
    {
        int fatalityIndex = _fatalityHistory[GetFatalityIndex()].fatalityPlaceIndex;
        transform.position = secondSceneFatalitiesData[fatalityIndex].positionTransform.position;
        transform.eulerAngles = new Vector3(0, _fatalityHistory[GetFatalityIndex()].fatalityRotation, 0);
    }    
    public void ChangePosition(int positionIndex)
    {
        transform.position = secondSceneFatalitiesData[positionIndex].positionTransform.position;
        int randomRotation = secondSceneFatalitiesData[positionIndex].defaultYRotation;
        FatalityData newFatalityData = new FatalityData(positionIndex, randomRotation);
        _fatalityHistory[_fatalityHistory.Count - 1] = newFatalityData;
    }

    public int GetFatalityIndex()
    {
        return _fatalityHistory.Count - 1;
    }
}