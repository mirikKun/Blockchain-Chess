using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionController : MonoBehaviour
{
    private cgChessPieceScript _newPiece;
    private cgBoard _newBoard;
    [SerializeField]private BoardCreator _boardCreator;

    [SerializeField] private GameObject promotionMenu;
    [SerializeField] private Transform allFigureParent;
    [SerializeField] private cgChessBoardScript chessBoard;
    [SerializeField] private PhotonView photonView;
    private cgSimpleMove _newMove;
    public bool figureChoosing; 
    public sbyte newFigureIndex;
    public int curFigureIndex;
    public bool IsOpen;

    public void SetBoard(cgBoard newBoard)
    {
        _newBoard = newBoard;
    }

    public void SetNewMove(cgSimpleMove newMove)
    {
        _newMove = newMove;
    }

    public void RememberPromotion(cgBoard board,cgChessPieceScript cgChessPiece,byte moveTo)
    {
        figureChoosing = true;
        _newPiece = cgChessPiece;
    }

    public void OpenPromotionArea()
    {
        promotionMenu.SetActive(true);
        IsOpen = true;
    }

    public void ChooseNewFigure(int figureIndex)
    {
        curFigureIndex = figureIndex;
        newFigureIndex= (sbyte)figureIndex;
        _newMove.promotionedType = newFigureIndex;

        _newBoard.curPromotion = newFigureIndex;
        figureChoosing = false;

        if (chessBoard.Mode == cgChessBoardScript.BoardMode.Multiplayer)
        {
            photonView.RPC("SetPromotionIndex", RpcTarget.AllBuffered, new object[] { figureIndex });

            promotionMenu.SetActive(false);
        }
        else
        {
            promotionMenu.SetActive(false);
        }
        IsOpen = false;
    }

    public void ChangeCharacter()
    {
        if(curFigureIndex==0)
            return;
        figureMover newFigure;
        if ((int) _newPiece.type > 0)
        {
            _newPiece.type = (cgChessPieceScript.Type) curFigureIndex;

            if (curFigureIndex == 2)
            {
                newFigure = Instantiate(_boardCreator.GetCurFigures()[0].rook, _newPiece.transform.position, Quaternion.Euler(0, 180, 0),
                    allFigureParent);
                newFigure.role = cgChessPieceScript.Type.WhiteRook;
            }
            else if(curFigureIndex == 3)           
            {
                newFigure = Instantiate(_boardCreator.GetCurFigures()[0].knight, _newPiece.transform.position, Quaternion.Euler(0, 180, 0),
                    allFigureParent); 
                newFigure.role = cgChessPieceScript.Type.WhiteKnight;

            }
            else if(curFigureIndex == 4)            
            {
                newFigure = Instantiate(_boardCreator.GetCurFigures()[0].bishop, _newPiece.transform.position, Quaternion.Euler(0, 180, 0),
                    allFigureParent); 
                newFigure.role = cgChessPieceScript.Type.WhiteBishop;

            }
            else
            {
                newFigure = Instantiate(_boardCreator.GetCurFigures()[0].queen, _newPiece.transform.position, Quaternion.Euler(0, 180, 0),
                    allFigureParent); 
                newFigure.role = cgChessPieceScript.Type.WhiteQueen;

            }
          
            newFigure.SetupRotation(new Vector3(0,180,0));
        }
        else
        {
            
            
            _newPiece.type = (cgChessPieceScript.Type) (-1*curFigureIndex);
            
            if (curFigureIndex == 2)
            {
                newFigure = Instantiate(_boardCreator.GetCurFigures()[1].rook, _newPiece.transform.position, Quaternion.Euler(0, 0, 0),
                    allFigureParent);
                newFigure.role = cgChessPieceScript.Type.BlackRook;

            }
            else if(curFigureIndex == 3)           
            {
                newFigure = Instantiate(_boardCreator.GetCurFigures()[1].knight, _newPiece.transform.position, Quaternion.Euler(0, 0, 0),
                    allFigureParent); 
                newFigure.role = cgChessPieceScript.Type.BlackKnight;

            }
            else if(curFigureIndex == 4)            
            {
                newFigure = Instantiate(_boardCreator.GetCurFigures()[1].bishop, _newPiece.transform.position, Quaternion.Euler(0, 0, 0),
                    allFigureParent); 
                newFigure.role = cgChessPieceScript.Type.BlackBishop;

            }
            else
            {
                newFigure = Instantiate(_boardCreator.GetCurFigures()[1].queen, _newPiece.transform.position, Quaternion.Euler(0, 0, 0),
                    allFigureParent);
                newFigure.role = cgChessPieceScript.Type.BlackQueen;

            }

            newFigure.SetupRotation(new Vector3(0,0,0));

        }
        
        FigureHolder figureHolder = _newPiece.square.GetComponent<FigureHolder>();
        figureHolder.figure.gameObject.SetActive(false);
        newFigure.figureIndex = _newPiece.figureArmyIndex;

        figureHolder.figure = newFigure;
        //_newPiece.figure = newFigure;

        curFigureIndex = 0;
    }
    
}
