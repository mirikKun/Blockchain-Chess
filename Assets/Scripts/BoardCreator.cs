using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BoardCreator : MonoBehaviour
{
    [SerializeField] private Transform piecesParent;
    [SerializeField] private Transform figuresParent;

    [SerializeField] private Transform flagsParent;
    // [SerializeField] private figureMover WhitePawn;
    // [SerializeField] private figureMover WhiteRook;
    // [SerializeField] private figureMover WhiteKnight;
    // [SerializeField] private figureMover WhiteBishop;
    // [SerializeField] private figureMover WhiteQueen;
    // [SerializeField] private figureMover WhiteKing;
    //
    // [SerializeField] private GameObject RedFlag;
    //
    // [SerializeField] private figureMover BlackPawn;
    // [SerializeField] private figureMover BlackRook;
    // [SerializeField] private figureMover BlackKnight;
    // [SerializeField] private figureMover BlackBishop;
    // [SerializeField] private figureMover BlackQueen;
    // [SerializeField] private figureMover BlackKing;
    //
    // [SerializeField] private GameObject BlackFlag;

    [SerializeField] private FlagSwitch flagSwitch;
    private List<figureMover> _newFigureMovers=new List<figureMover>();
    private cgChessPieceScript[] _cgPieceScripts;
    private int _curFirstColor;
    private int _curSecondColor;
    private int _enemyTurn;
    [SerializeField]private FigureTeam[] figureTeams;


    [System.Serializable]
    public struct FigureTeam
    {
        public figureMover pawn;
        public figureMover rook;
        public figureMover knight;
        public figureMover bishop;
        public figureMover queen;
        public figureMover king;
        
        public GameObject flag;
    }

    public FigureTeam[] GetCurFigures()
    {
        FigureTeam[] curFigureTeams = new FigureTeam[2];
        if (_curFirstColor > 2)
        {
            if (_enemyTurn == 1 && EnemyData.Army != null)
            {
                curFigureTeams[0] = figureTeams[EnemyData.ColorIndex];
            }
            else
            {
                curFigureTeams[0] = figureTeams[PlayerData.ColorIndex];

            }
        }
        else
        {
            curFigureTeams[0] = figureTeams[_curFirstColor];
        }
        if (_curSecondColor > 2)
        {
            if (_enemyTurn == 1 || EnemyData.Army == null)
            {
                curFigureTeams[1] = figureTeams[PlayerData.ColorIndex];
            }
            else
            {
                curFigureTeams[1] = figureTeams[EnemyData.ColorIndex];

            }        
        }
        else
        {
            curFigureTeams[1] = figureTeams[_curSecondColor];

        }
        return curFigureTeams;
    }

    public figureMover GetFigureByRoleAndIndex(int index,cgChessPieceScript.Type role)
    {
        Debug.Log("Figure are not found");

        figureMover[] curFigureMovers= figuresParent.GetComponentsInChildren<figureMover>(true);
        foreach (var curFigureMover in curFigureMovers)
        {
            if (curFigureMover.figureIndex == index && curFigureMover.role == role)
            {
                return curFigureMover;
            }
        }
        Debug.Log("Figure are not found");
        return null;
    }
    public void SetupBoard()
    {
        figureMover[] curFigureMovers= figuresParent.GetComponentsInChildren<figureMover>(true);
        _cgPieceScripts = piecesParent.GetComponentsInChildren<cgChessPieceScript>();

        foreach (var curFigureMover in curFigureMovers)
        {
            bool figureWasFound=false;
            foreach (var cgPieceScript in _cgPieceScripts)
            {
                //if()
                if (curFigureMover.figureIndex == cgPieceScript.figureArmyIndex&&curFigureMover.role==cgPieceScript.type)
                {
                    curFigureMover.gameObject.SetActive(true);
                    curFigureMover.transform.position = cgPieceScript.transform.position;
                    cgPieceScript.square.GetComponent<FigureHolder>().figure = curFigureMover;
                    figureWasFound = true;
                }
                
            }

            if (!figureWasFound)
            {
                curFigureMover.gameObject.SetActive(false);
            }
        }
    }
    public void CreateBoard()
    {

        CreateBoard(_curFirstColor, _curSecondColor);
        if (_curFirstColor == 3)
        {
            if (_enemyTurn == 1&&EnemyData.Army!=null)
            {
                _curFirstColor = EnemyData.ColorIndex;
            }
            else
            {
                _curFirstColor = PlayerData.ColorIndex;
            }
        }        if (_curSecondColor == 3)
        {
            if (_enemyTurn == 1||EnemyData.Army==null)
            {
                _curSecondColor = PlayerData.ColorIndex;
            }
            else
            {
                _curSecondColor = EnemyData.ColorIndex;
            }
        }
    }
    

    public figureMover GetFigureByIndex(cgChessPieceScript.Type type)
    {
            figureMover newFigureMover;
            switch (type)
            {
                case cgChessPieceScript.Type.WhitePawn:
                    newFigureMover = figureTeams[_curFirstColor].pawn;
                    break;
                case cgChessPieceScript.Type.WhiteRook:
                    newFigureMover = figureTeams[_curFirstColor].rook;
                    break;                
                case cgChessPieceScript.Type.WhiteKnight:
                    newFigureMover = figureTeams[_curFirstColor].knight;
                    break;                
                case cgChessPieceScript.Type.WhiteBishop:
                    newFigureMover = figureTeams[_curFirstColor].bishop;
                    break;                
                case cgChessPieceScript.Type.WhiteQueen:
                    newFigureMover = figureTeams[_curFirstColor].queen;
                    break;                
                case cgChessPieceScript.Type.WhiteKing:
                    newFigureMover = figureTeams[_curFirstColor].king;
                    break;         
                
                case cgChessPieceScript.Type.BlackPawn:
                    newFigureMover = figureTeams[_curSecondColor].pawn;
                    break;                
                case cgChessPieceScript.Type.BlackRook:
                    newFigureMover = figureTeams[_curSecondColor].rook;
                    break;
                case cgChessPieceScript.Type.BlackKnight:
                    newFigureMover = figureTeams[_curSecondColor].knight;
                    break;                
                case cgChessPieceScript.Type.BlackBishop:
                    newFigureMover = figureTeams[_curSecondColor].bishop;
                    break;                
                case cgChessPieceScript.Type.BlackQueen:
                    newFigureMover = figureTeams[_curSecondColor].queen;
                    break;                
                default:
                    newFigureMover = figureTeams[_curSecondColor].king;
                    break;
            }

            return newFigureMover;
    }
    public figureMover GetFigureByIndexInArmy(Army army,int figureArmyIndex)
    {
        var newFigureMover = army.GetCharacters()[figureArmyIndex].figurePrefab;
        return newFigureMover;
    }
    
    public void CreateBoard(int firstColorIndex, int secondColorIndex,int enemyTurn=1)
    {
        _enemyTurn = enemyTurn;
        _newFigureMovers=new List<figureMover>();
        
        _curFirstColor = firstColorIndex;
        _curSecondColor = secondColorIndex;
        foreach (Transform figure in figuresParent)
        {
            Destroy(figure.gameObject);
        }

        _cgPieceScripts = piecesParent.GetComponentsInChildren<cgChessPieceScript>();
       // FigureHolder[] figureHolders = new FigureHolder[cgSquareScripts.Length];
       figureMover[] curFigureMovers= figuresParent.GetComponentsInChildren<figureMover>();

       foreach (var curFigureMover in curFigureMovers)
       {
           Destroy(curFigureMover.gameObject);
       }       

       foreach (Transform curFlag in flagsParent)
       {
           Destroy(curFlag.gameObject);
       }


        for (int i = 0; i < _cgPieceScripts.Length; i++)
        {

            bool enemyArmy=false;
            bool playerArmy=false;
           

            var figureHolder = _cgPieceScripts[i].square.GetComponent<FigureHolder>();

            figureMover newFigureMover;
            float rotation = 180+transform.eulerAngles.y;
            figureMover newFigurePrefab;
                
            if (_cgPieceScripts[i].type > 0)
            {    
                
                if(_curFirstColor<3)
                {
                    newFigurePrefab = GetFigureByIndex(_cgPieceScripts[i].type);
                    
                }   
                else
                {
                    //newFigurePrefab = GetFigureByIndexInArmy(PlayerData.Army, _cgPieceScripts[i].figureArmyIndex);
                    
                        if (enemyTurn == 1&&EnemyData.Army!=null)
                        {
                            newFigurePrefab =
                                GetFigureByIndexInArmy(EnemyData.Army, _cgPieceScripts[i].figureArmyIndex);
                            enemyArmy = true;
                        }
                        else
                        {
                            newFigurePrefab =
                                GetFigureByIndexInArmy(PlayerData.Army, _cgPieceScripts[i].figureArmyIndex);
                            playerArmy = true;

                        }
                    
                }
                newFigureMover = Instantiate(newFigurePrefab,
                                         figureHolder.transform.position, Quaternion.Euler(0, rotation + 180, 0), figuresParent);
                // if(_curFirstColor==3)
                // {
                //     newFigureMover.thisCharacter = army.GetCharacters()[figureArmyIndex];
                //     
                // }   

            }
            else
            {  
                if(_curSecondColor<3)
                {
                    newFigurePrefab = GetFigureByIndex(_cgPieceScripts[i].type);
                }   
                else
                {
                    
                    if (enemyTurn == 1||EnemyData.Army==null)
                    {
                        playerArmy = true;

                        newFigurePrefab = GetFigureByIndexInArmy(PlayerData.Army, _cgPieceScripts[i].figureArmyIndex);
                    }
                    else
                    {
                        enemyArmy = true;

                        newFigurePrefab = GetFigureByIndexInArmy(EnemyData.Army, _cgPieceScripts[i].figureArmyIndex);

                    }
                    //newFigurePrefab = GetFigureByIndexInArmy(PlayerData.Army, _cgPieceScripts[i].figureArmyIndex);
                }
                newFigureMover = Instantiate(newFigurePrefab, figureHolder.transform.position, Quaternion.Euler(0, rotation+0, 0),figuresParent);
            }

            if (enemyArmy)
            {
                newFigureMover.thisCharacter = EnemyData.Army._characters[_cgPieceScripts[i].figureArmyIndex];
                newFigureMover.playerType = PlayerType.Enemy;

            }
            else if (playerArmy)
            {
                newFigureMover.thisCharacter = PlayerData.Army._characters[_cgPieceScripts[i].figureArmyIndex];
                newFigureMover.playerType = PlayerType.Player;

            }
            newFigureMover.figureIndex = _cgPieceScripts[i].figureArmyIndex;

            newFigureMover.role = _cgPieceScripts[i].type;
            //_cgPieceScripts[i].figure = newFigureMover;
            figureHolder.figure = newFigureMover;
            Transform flagHolder=newFigureMover.transform.Find("Root_M");
            
            if (flagHolder == null)
            {
                foreach (var animatorHolder in newFigureMover.GetComponentsInChildren<Animator>())
                {
                    if (animatorHolder.transform.Find("Root_M"))
                    {
                        flagHolder=animatorHolder.transform.Find("Root_M");
                    }
                }
            }
            if(flagHolder)
            {
                GameObject newFlag;
                if (_cgPieceScripts[i].type > 0)
                {
                    if(_curFirstColor<3)
                    {                   
                        newFlag=Instantiate(figureTeams[_curFirstColor].flag, flagHolder);
                    }
                    else
                    {

                        if (enemyTurn == 1&&EnemyData.Army!=null)
                        { 
                            newFlag=Instantiate(figureTeams[EnemyData.ColorIndex].flag, flagHolder);
                        }
                        else
                        {
                            newFlag=Instantiate(figureTeams[PlayerData.ColorIndex].flag, flagHolder);

                        }
                    }
                   
                    newFlag.transform.localPosition = new Vector3(0, -0.12f, 0);
                    newFlag.transform.localEulerAngles = new Vector3(-90, 90, 0);

                }
                else
                {
                    if(secondColorIndex<3)
                    {                   
                        newFlag=Instantiate(figureTeams[secondColorIndex].flag,flagHolder);
                    }
                    else
                    {
                        if (enemyTurn == 1)
                        { 
                            newFlag=Instantiate(figureTeams[PlayerData.ColorIndex].flag, flagHolder);
                        }
                        else
                        {
                            newFlag=Instantiate(figureTeams[EnemyData.ColorIndex].flag, flagHolder);

                        }
                    }
                    newFlag.transform.localPosition = new Vector3(0, -0.12f, 0);
                    newFlag.transform.localEulerAngles = new Vector3(-90, 90, 0);
                }

                if (Mathf.Abs((int)_cgPieceScripts[i].type) ==4)
                {
                    newFlag.transform.localEulerAngles = new Vector3(-103, 90, 0);

                }

                newFlag.name = "Flag";
                newFigureMover.SetFlag(newFlag);
                
            }
            _cgPieceScripts[i].FlagSetup();

            _newFigureMovers.Add(newFigureMover);
            
        }

        flagSwitch.AddFigureMover(_newFigureMovers);
    }
}
