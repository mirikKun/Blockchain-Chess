using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using System.Linq;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameManager game_manager;
    [SerializeField] private GameObject looking_random_player_panel;
    [SerializeField] private GameObject disconnect_player_panel;
    public static bool isReconnected = false;
    
    private const int MAX_PLAYERS_IN_ROOM = 2;
    private int environment_scene_id;

    private bool rejoinCalled;

    private int minTimeRequiredToRejoin = 0; // TODO: set dynamically based on PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime

    private DisconnectCause lastDisconnectCause;
    private bool wasInRoom;

    private bool connectPressed;
    private bool reconnectCalled;

    [SerializeField]
    private bool skipRejoinChecks;
    [Tooltip("Whether or not realtime webhooks are configured with persistence enabled")]
    [SerializeField]
    private bool persistenceEnabled;
    private TypedLobby sqlLobby = new TypedLobby("customSqlLobby", LobbyType.SqlLobby);
    private const string ENVIRONMENT = "C0";
    public const string COLOR_ARMY = "C1";
    public const string CharactersIDs = "C2";
    public const string CharactersXPs = "C3";
    public const string PlayerName = "C4";

    public static Player EnemyPlayer => PhotonNetwork.CurrentRoom.Players.Values
            .First(x =>!PhotonNetwork.LocalPlayer.Equals(x));
    
    private string SqlLobbyFilter => $"C0 = { environment_scene_id } ";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        disconnect_player_panel.SetActive(false);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
        PhotonPeer.RegisterType(typeof(CharacterProgress), 255, CharacterProgress.Serialize, CharacterProgress.Deserialize);
    }

    public void Connect()
    {
        connectPressed = true;
        looking_random_player_panel.SetActive(false);

        if (game_manager.GetChosenBoardMode() == cgChessBoardScript.BoardMode.Multiplayer)
        {
            if (PhotonNetwork.IsConnected)
            {
                JoiningRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            looking_random_player_panel.SetActive(true);
        }
    }

    private void JoiningRandomRoom()
    {
        ExitGames.Client.Photon.Hashtable playerData = new ExitGames.Client.Photon.Hashtable(1);
        playerData.Add(COLOR_ARMY, PlayerData.ColorIndex);
            
        if(PlayerData.Army!=null)
        {
              var indexesIds = PlayerData.Army._characters.Select(x => x.index).ToArray();
              //Debug.Log(String.Join(",", indexes));
              playerData.Add(CharactersIDs, indexesIds);
              var indexesXps = PlayerData.Army._characters.Select(x => x.characterProgress).ToArray();
              playerData.Add(CharactersXPs, indexesXps);
              playerData.Add(PlayerName, PlayerData.PlayerName);
        }
        ///
       // game_manager.GetArmyByIndexes(indexes);
        /////
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerData);
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, sqlLobby, SqlLobbyFilter);
    }

    public void LeaveRoom()
    {
        looking_random_player_panel.SetActive(false);
        connectPressed = false;

        wasInRoom = false;
        if (PhotonNetwork.InRoom)
        {
            //photonView.RPC("RPC_ShowOpponentLeftGameMessage", RpcTarget.OthersBuffered);

            PhotonNetwork.LeaveRoom();
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }
        //cgBoard._whiteTurnToMove = true;
        isReconnected = false;
    }

    [PunRPC]
    private void RPC_ShowOpponentLeftGameMessage()
    {
        //disconnect_player_panel.SetActive(true);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.StateChanged += this.OnStateChanged;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.StateChanged -= this.OnStateChanged;
    }

    private void OnStateChanged(ClientState fromState, ClientState toState)
    {
        if (toState == ClientState.Disconnected)
        {
            Debug.LogFormat("OnStateChanged from {0} to {1}, PeerState={2}", fromState, toState,
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);
            this.HandleDisconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogFormat("OnDisconnected(cause={0}) ClientState={1} PeerState={2}",
            cause,
            PhotonNetwork.NetworkingClient.State,
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);
        if (rejoinCalled)
        {
            Debug.Log("PhotonConnection Rejoin failed, client disconnected");
            rejoinCalled = false;
            return;
        }
        
        if (reconnectCalled)
        {
            Debug.Log("PhotonConnection Reconnect failed, client disconnected");
            reconnectCalled = false;
            return;
        }
        lastDisconnectCause = cause;
        if (PhotonNetwork.NetworkingClient.State == ClientState.Disconnected)
        {
            this.HandleDisconnect();
        }
    }

    private void HandleDisconnect()
    {
        lastDisconnectCause = DisconnectCause.DisconnectByServerReasonUnknown; // test feature, remove later
        switch (lastDisconnectCause)
        {
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.AuthenticationTicketExpired:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                if (wasInRoom)
                {
                    //Debug.Log("18");
                    isReconnected = true;
                    this.CheckAndRejoin();
                }
                else
                {
                    //Debug.Log("19");
                    //Debug.Log("PhotonNetwork.Reconnect called");
                    reconnectCalled = PhotonNetwork.Reconnect();
                }
                break;
            case DisconnectCause.OperationNotAllowedInCurrentState:
                //Debug.Log("20.1");
                break;
            case DisconnectCause.CustomAuthenticationFailed:
                //Debug.Log("20.2");
                break;
            case DisconnectCause.DisconnectByClientLogic:
                //Debug.Log("20.3");
                break;
            case DisconnectCause.InvalidAuthentication:
                //Debug.Log("20.4");
                break;
            case DisconnectCause.ExceptionOnConnect:
                //Debug.Log("20.5");
                break;
            case DisconnectCause.MaxCcuReached:
                //Debug.Log("20.6");
                break;
            case DisconnectCause.InvalidRegion:
                //Debug.Log("20.7");
                break;
            case DisconnectCause.None:
                //Debug.Log("20");
                break;
            default:
                //Debug.Log("21");
                throw new ArgumentOutOfRangeException("cause", lastDisconnectCause, null);
        }
    }

    private void CheckAndRejoin()
    {
        Debug.Log("rejoining");
        if (skipRejoinChecks)
        {
            //Debug.Log("PhotonNetwork.ReconnectAndRejoin called");
            rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
        }
        else
        {
            bool wasLastActivePlayer = true;
            if (!persistenceEnabled)
            {
                for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
                {
                    if (!PhotonNetwork.PlayerListOthers[i].IsInactive)
                    {
                        wasLastActivePlayer = false;
                        break;
                    }
                }
            }
            if ((PhotonNetwork.CurrentRoom.PlayerTtl < 0 || PhotonNetwork.CurrentRoom.PlayerTtl > minTimeRequiredToRejoin) // PlayerTTL checks
              && (!wasLastActivePlayer || PhotonNetwork.CurrentRoom.EmptyRoomTtl > minTimeRequiredToRejoin || persistenceEnabled)) // EmptyRoomTTL checks
            {
                //Debug.Log("PhotonNetwork.ReconnectAndRejoin called");
                rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
            }
            else
            {
                //Debug.Log("PhotonNetwork.ReconnectAndRejoin not called, PhotonNetwork.Reconnect is called instead.");
                reconnectCalled = PhotonNetwork.Reconnect();
            }
        }
        Debug.Log("4 environment_scene_id = " + environment_scene_id + " PlayerData.ColorIndex = " + PlayerData.ColorIndex);
    }

    public void SetRoomsSearchingOptions(int scene_index_id)
    {
        environment_scene_id = scene_index_id;
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        if (reconnectCalled)
        {
            //Debug.Log("Reconnect successful");
            reconnectCalled = false;
        }
        else
        {
            Debug.Log("PhotonConnection Connecting to server. Looking for random room.");

            JoiningRandomRoom();
        }
    }
    

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("aaaaa" + connectPressed);
        if (!connectPressed)
        {
            return;
        }
        Debug.Log($"PhotonConnection Joining random room failed because of { message }. Creating a new one");
        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            CustomRoomPropertiesForLobby = new string[]
            {
                ENVIRONMENT,
                COLOR_ARMY
            },
            MaxPlayers = MAX_PLAYERS_IN_ROOM,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                { ENVIRONMENT, environment_scene_id },
                { COLOR_ARMY, PlayerData.ColorIndex }
            },
            PlayerTtl = -1,
            EmptyRoomTtl = -1,
            IsOpen = true,
            IsVisible = true
        }, sqlLobby);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonConnection Player { PhotonNetwork.LocalPlayer.ActorNumber } joined the room");
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            if (!wasInRoom)
            {
                wasInRoom = true;
                game_manager.SetupGame();
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"PhotonConnection Player { newPlayer.ActorNumber } entered the room");
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {

            game_manager.SetupGame();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"PhotonConnection Player { otherPlayer.ActorNumber } left the room");
    }
    #endregion
}
