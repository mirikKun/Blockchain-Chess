using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FigureHolder : MonoBehaviour
{
    public figureMover figure;
    private FLag2d _figureFlag;
    public bool moving;

    [SerializeField] private FLag2d flagPrefab;


    [SerializeField] private Transform flags;
    private void Start()
    {
    
    }

    [PunRPC]
    public void CreateFlag(int figureIndex,cgChessPieceScript pieceScript)
    {
        _figureFlag = Instantiate(flagPrefab, new Vector3(transform.position.x, flags.position.y, transform.position.z),
            Quaternion.Euler(90,0,0), flags);
        SetupFlagColorAndRotation(figureIndex);
        _figureFlag.SetupNewIcon(Mathf.Abs(figureIndex) - 1);
        pieceScript.flag = _figureFlag;
    }

    public void SetupFlagColorAndRotation(int figureIndex)
    {   
        if(!_figureFlag)
            return;
        _figureFlag.SetColor((int)figure.thisCharacter.characterClan);
        if(figureIndex>0)
        {
            //_figureFlag.SetRedBackgroundColor();
            _figureFlag.transform.localEulerAngles=new Vector3(90,0,0);
        }
        else
        {
            //_figureFlag.SetBlackBackgroundColor();

            _figureFlag.transform.localEulerAngles=new Vector3(-90,0,0);

        }      
        _figureFlag.SetDefaultIconRotation();

    }
    
    
    
    [PunRPC]
    public void FigureMove(FigureHolder newPlace,cgSquareScript enPassantSquare,bool run)
    {
        figure.StartMove(newPlace,enPassantSquare,run);
        figure = null;
    }
    [PunRPC]
    public figureMover HaveTargetFigure()
    {
        return figure;
    }

    [PunRPC]
    public void SetNewFigure(figureMover newFigure)
    {
        figure = newFigure; 
        moving = true;
    }

    [PunRPC]
    public void EndMoving()
    {
        moving = false;
    }

    [PunRPC]
    public bool GetMovingBool()
    {
        return moving;
    }
    [PunRPC]
    public void SkipFigureAnimations()
    {
        if (figure)
        {
            figure.SkipAnimation();  
        }
    }
}
