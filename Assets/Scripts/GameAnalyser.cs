using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameAnalyser : MonoBehaviour
{
    [SerializeField] private RectTransform movesList;
    [SerializeField] private GameObject moveListPanel;
    [SerializeField] private AnaliseMove analiseMovePrefab;
    private FlagSwitch _flagSwitch;
    private List<AnaliseMove> _analiseMoves = new List<AnaliseMove>();
    private List<cgSimpleMove> _moves;
    private AnaliseMove _lastAnaliseMove;
    private FatalityController _fatalityController;
    private cgChessBoardScript _cgChessBoardScript;
    private BoardCreator _boardCreator;
    private SecondScenePositionController _scenePositionController;
    private int _curMoveNumber;
    private bool _moveListPanelOpened;

    private bool _analiseStarted;

    private void Start()
    {
        _scenePositionController = FindObjectOfType<SecondScenePositionController>();
    }

    public void SetupAnalyser()
    {
        _fatalityController = FindObjectOfType<FatalityController>();
        _cgChessBoardScript = FindObjectOfType<cgChessBoardScript>();
        _boardCreator = _cgChessBoardScript.GetComponent<BoardCreator>();

        _flagSwitch = FindObjectOfType<FlagSwitch>();
        _flagSwitch.SwitchToOff();
        SetupMoves(_cgChessBoardScript.GetMoves());
    }

    private void SetupMoves(List<cgSimpleMove> newMoves)
    {
        if(!_analiseStarted)
        {
         _cgChessBoardScript.SetGameEnded();

            _analiseStarted = true;
            _moves = newMoves;
            _cgChessBoardScript.UnhighlightMoves();
            movesList.sizeDelta = new Vector2(movesList.sizeDelta.x, 113 * (newMoves.Count + 1) + 55);
            List<int> promotionIndexes=new List<int>();
            for (int i = 0; i < newMoves.Count + 1; i++)
            {
                {
                    //movesNames.Add("Move "+(i));
                    var analiseMove = Instantiate(analiseMovePrefab, movesList);
                    if (i == 0)
                    {
                        analiseMove.SetupMove(null, "", "", i, GoToMove, SeeExecution);
                    }
                    else
                    {
                        if (Math.Abs(newMoves[i - 1].promotionedType) > 1)
                        {           
                            promotionIndexes.Add(_cgChessBoardScript.charactersPositions[i][newMoves[i-1].to]);
                        }
                        analiseMove.SetupMove(_cgChessBoardScript.GetCharacterByMove(i,newMoves[i-1].to,promotionIndexes), _cgChessBoardScript.GetMoveName(newMoves[i - 1])
                            , newMoves[i - 1].time, i, GoToMove, SeeExecution);
                    }

                    if (!_lastAnaliseMove)
                    {
                        _lastAnaliseMove = analiseMove;
                    }

                    _analiseMoves.Add(analiseMove);
                }
            }
            Debug.Log("_______-");
            foreach (var promotionType in promotionIndexes)
            {
                Debug.Log(promotionType);
            }
            _cgChessBoardScript.SaveBoard();

        }
        //movesDropdown.AddOptions(movesNames);}
        ReCreateBoard();
    }

    public void NextMove()
    {

        _cgChessBoardScript.NextMove();
        if (_curMoveNumber < _moves.Count)
        {
            _curMoveNumber++;
            SelectCurrentMove(_curMoveNumber);
        }
    }

    public void PreviousMove()
    {
        if (_curMoveNumber > 0)
        {
            _cgChessBoardScript.PreviousMove();
            _curMoveNumber--;
            SelectCurrentMove(_curMoveNumber);
        }
    }
    

    public void GoToMove(int moveNumber)
    {
        _cgChessBoardScript.GoToTheMove(moveNumber);
        _curMoveNumber = moveNumber;
        SelectCurrentMove(moveNumber);
    }

    private void ReCreateBoard()
    {
        _cgChessBoardScript.GoToTheMove(0,true);
        _curMoveNumber = 0;
        SelectCurrentMove(0);
    }

    public void SeeExecution(int moveNumber)
    {
        int attackerRole=_cgChessBoardScript.GetLastMoveFigureIndex();
        int defenderRole=_moves[moveNumber - 1].capturedType;        
        
        int attackerIndex=_cgChessBoardScript.charactersPositions[moveNumber -1][_moves[moveNumber - 1].from];
        int defenderIndex=_cgChessBoardScript.charactersPositions[moveNumber -1][_moves[moveNumber - 1].to];
        
        Debug.Log("000");
        Debug.Log(attackerIndex);
        Debug.Log(defenderIndex);
        
        // figureMover _attackerFigure=_boardCreator.GetFigureByIndex((cgChessPieceScript.Type)attackerRole);
        // figureMover _targetFigure=_boardCreator.GetFigureByIndex((cgChessPieceScript.Type)defenderRole);        
        figureMover attackerFigure=_boardCreator.GetFigureByRoleAndIndex(attackerIndex,(cgChessPieceScript.Type)attackerRole);
        figureMover targetFigure=_boardCreator.GetFigureByRoleAndIndex(defenderIndex,(cgChessPieceScript.Type)defenderRole);
        FigureFatality[] _fatalitys;
        List<FigureFatality> _curFigureFatalityAnims = new List<FigureFatality>();
        _fatalitys = attackerFigure.GetComponents<FigureFatality>();
        int _randomAnimIndex;
        
        if (_curFigureFatalityAnims.Count == 0)
        {
            foreach (var fatality in _fatalitys)
            {
                if (!fatality.FatalityName.Contains(attackerFigure.figureType.ToString())||fatality.FatalityName=="KnightDeath"||fatality.FatalityName=="KnightDeathFromKnightFatality") continue;
                _curFigureFatalityAnims.Add(fatality);
            }
        }
        _randomAnimIndex = Random.Range(0, _curFigureFatalityAnims.Count);

        _scenePositionController.SetFatalityDataByIndex(_moves[moveNumber - 1].fatalityIndex);
        
        if (targetFigure.figureType == figureMover.FigureType.Knight)
        {
            if (attackerFigure.figureType == figureMover.FigureType.Knight)
            {
                    
                _fatalityController.StartPutFigure(attackerFigure, targetFigure,
                    "KnightDeathFromKnightFatality");

            }
            else
            {
                _fatalityController.StartPutFigure(attackerFigure, targetFigure,
                    "KnightDeath");

            }
        }
        else
        {
            _fatalityController.StartPutFigure(attackerFigure, targetFigure,
                _curFigureFatalityAnims[_randomAnimIndex].FatalityName);

        } 
    }

    public void OpenMovesListPanel()
    {
        _moveListPanelOpened = true;
        moveListPanel.SetActive(true);
        SelectCurrentMove(_curMoveNumber);
    }
    public void CloseMovesListPanel()
    {
        _moveListPanelOpened = false;
        moveListPanel.SetActive(false);
    }
    private void SelectCurrentMove(int moveNumber)
    {
        if(_moveListPanelOpened)
        {
            if (_lastAnaliseMove)
            {
                _lastAnaliseMove.UnselectMove();
            }

            if (moveNumber > 0)
            {
                _analiseMoves[moveNumber].SelectMove(_moves[moveNumber - 1].capturedType != 0);
            }
            else
            {
                _analiseMoves[moveNumber].SelectMove(false);
            }
        }

        _lastAnaliseMove = _analiseMoves[moveNumber];
    }
}