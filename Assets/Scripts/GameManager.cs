using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Dropdown gameMode;
    [SerializeField] private Dropdown firstColor;
    [SerializeField] private Dropdown secondColor;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private MenuController menuController;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TextMeshProUGUI error_showing_text;

    [SerializeField] private TextMeshProUGUI firstPlayerName;
    [SerializeField] private TextMeshProUGUI secondPlayerName;
    [SerializeField] private CharacterProgress[] bharacterProgress;
    public static bool Pause=true;
    public static bool In2dView;    
    public static bool WhiteSide;
    
    private cgChessBoardScript _cgChessBoard;
    private Transform _transform;
    private BoardCreator _boardCreator;

    private void Start()
    {
        Pause = true;
        _transform = transform;
        _cgChessBoard = GetComponent<cgChessBoardScript>();
        _boardCreator = GetComponent<BoardCreator>();
        Character[] characters = PlayerData.Characters;
        foreach (var character in characters)
        {
            character.characterProgress.KillsPerGame = new List<int>();
        }
        
    }

    public void SetPause(bool pause)
    {
        Pause = pause;
    }
    public cgChessBoardScript.BoardMode GetChosenBoardMode()
    {
        return _cgChessBoard.Mode;
    }

    public void ChooseRedTeam()
    {
        _cgChessBoard.Mode = cgChessBoardScript.BoardMode.PlayerVsEngine;
        _cgChessBoard.start();
        _transform.eulerAngles = new Vector3(90, 180, 0);
    }

    public void ChooseBlackTeam()
    {
        _cgChessBoard.Mode = cgChessBoardScript.BoardMode.EngineVsPlayer;
        _cgChessBoard.start();
        _transform.eulerAngles = new Vector3(90, 0, 0);
    }

    public void ChoosePlayerVsPlayer()
    {
        _cgChessBoard.Mode = cgChessBoardScript.BoardMode.PlayerVsPlayer;
        _cgChessBoard.start();
        _transform.eulerAngles = new Vector3(90, 180, 0);
    }



    public void SetupGame()
    {
        error_showing_text.gameObject.SetActive(false);
        Pause = false;
        if (_cgChessBoard.Mode == cgChessBoardScript.BoardMode.Multiplayer && !PhotonNetwork.IsConnected)
        {
            return;
        }

        if ((firstColor.value == 3 || secondColor.value == 3) && PlayerData.Army == null)
        {
            return;
        }

        if (gameMode.value == 0)
        {
            _cgChessBoard.Mode = cgChessBoardScript.BoardMode.PlayerVsEngine;
            WhiteSide = true;
            _transform.localEulerAngles = new Vector3(90, 180, 0);
        }
        else if (gameMode.value == 1)
        {
            _cgChessBoard.Mode = cgChessBoardScript.BoardMode.EngineVsPlayer;
            WhiteSide = false;

            _transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        else if (gameMode.value == 3)
        {
            _cgChessBoard.Mode = cgChessBoardScript.BoardMode.Multiplayer;
            _transform.localEulerAngles = new Vector3(90, 180, 0);
        }
        else
        {
            _cgChessBoard.Mode = cgChessBoardScript.BoardMode.PlayerVsPlayer;
            WhiteSide = true;

            _transform.localEulerAngles = new Vector3(90, 180, 0);
        }

        if (_cgChessBoard.Mode != cgChessBoardScript.BoardMode.Multiplayer)
        {
            _boardCreator.CreateBoard(firstColor.value, secondColor.value);
            menuController.CloseChooseTeamArea();
            _cgChessBoard.start();
        }

        if (_cgChessBoard.Mode == cgChessBoardScript.BoardMode.Multiplayer)
        {

            networkManager.SetRoomsSearchingOptions(SceneManagerHelper.ActiveSceneBuildIndex);

            if (PhotonNetwork.CurrentRoom != null)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    ///Same colors can`t play in same game
                    PhotonNetwork.CurrentRoom.GetPlayer(1).CustomProperties.TryGetValue(NetworkManager.COLOR_ARMY, out var color1);
                    PhotonNetwork.CurrentRoom.GetPlayer(2).CustomProperties.TryGetValue(NetworkManager.COLOR_ARMY, out var color2);
                    NetworkManager.EnemyPlayer.CustomProperties.TryGetValue(NetworkManager.CharactersIDs, out var enemyIndexes);
                    NetworkManager.EnemyPlayer.CustomProperties.TryGetValue(NetworkManager.CharactersXPs, out var enemyExperiences);

                    NetworkManager.EnemyPlayer.CustomProperties.TryGetValue(NetworkManager.PlayerName, out var playerName);
                    Army newArmy = new Army();
                    if((int[]) enemyIndexes!=null)
                    {
                        newArmy.SetupArmy(Character.GetCharactersByIndexes((int[]) enemyIndexes).ToArray());
                    }

                    if ((CharacterProgress[]) enemyExperiences != null)
                    {
                        bharacterProgress = ((CharacterProgress[]) enemyExperiences);
                        int i = 0;
                        foreach (var character in newArmy._characters)
                        {
                            character.enemyCharacterProgress = ((CharacterProgress[]) enemyExperiences)[i];
                            character.SetupLevel(true);
                            i++;
                        }
                    }

                    EnemyData.Army = newArmy;
                    EnemyData.PlayerName = (string)playerName;
                    //NetworkManager.EnemyPlayer.
                    if(NetworkManager.EnemyPlayer.ActorNumber==1)
                    {
                        WhiteSide = false;

                        EnemyData.ColorIndex = (int)color1;
                    }
                    else
                    {
                        WhiteSide = true;

                        EnemyData.ColorIndex = (int)color2;

                    }
                    _boardCreator.CreateBoard(firstColor.value, secondColor.value, NetworkManager.EnemyPlayer.ActorNumber);

                    menuController.CloseChooseTeamArea();
                    if (!NetworkManager.isReconnected)
                    {
                        photonView.RPC("start", RpcTarget.AllBuffered, new object[] { null, _cgChessBoard.Mode });
                    }
                    else
                    {
                        _cgChessBoard.start(null, _cgChessBoard.Mode);
                    }

                    if (NetworkManager.EnemyPlayer.ActorNumber == 1)
                    {
                        firstPlayerName.text=EnemyData.PlayerName;
                        secondPlayerName.text = PlayerData.PlayerName;
                    }
                    else
                    {
                        firstPlayerName.text=PlayerData.PlayerName;
                        secondPlayerName.text = EnemyData.PlayerName; 
                    }
                }
            }
        }
        networkManager.Connect();
    }
}