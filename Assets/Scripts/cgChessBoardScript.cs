using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.ComponentModel;
using System.Timers;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;
using UnityEngine.EventSystems;
using BackgroundWorker = System.ComponentModel.BackgroundWorker;
using Photon.Pun;

/// <summary>
/// The script attached to the chessboard prefab, should have gameobjects with squarescripts, this class: verifies if the player can drag and drop pieces, handles whether or not the engine should make move, checks whether or not the game is over(and if so shows the game over prefab), flips the board, resets the board etc.
/// </summary>
[System.Serializable]
[RequireComponent(typeof(PhotonView))]
public class cgChessBoardScript : MonoBehaviour
{
    #region Properties & fields

    [SerializeField] private PhotonView photonView;

    /// <summary>
    /// A sound to play whenever a move is made.
    /// </summary>
    public AudioClip moveSound;

    /// <summary>
    /// A sound to play when any player makes a move that checks the king.
    /// </summary>
    public AudioClip checkSound;

    /// <summary>
    /// A sound to play when the game is won.
    /// </summary>
    public AudioClip winSound;

    /// <summary>
    /// A sound to play when the game is lost or drawn.
    /// </summary>
    public AudioClip loseSound;

    /// <summary>
    /// The number of moves made.
    /// </summary>
    public int movesMade = 0;

    /// <summary>
    /// The textfield to log all moves made.
    /// </summary>
    public UnityEngine.UI.Text moveLog;

    /// <summary>
    /// The provided loadbar to display how far the analysis is.
    /// </summary>
    public GameObject engineProgressBar;

    /// <summary>
    /// The controller of promotion process
    /// </summary>
    public PromotionController promotionController;

    /// <summary>
    /// The controller of promotion process
    /// </summary>
    public MenuController menuController;


    /// <summary>
    /// The target to flip when the user click the Flip board button.
    /// </summary>
    public GameObject flipTarget;

    /// <summary>
    /// The target to flip when the user click the Flip board button.
    /// </summary>
    public GameObject chessPieceHolder;

    /// <summary>
    /// The target to flip when the user click the Flip board button.
    /// </summary>
    public GameObject chessSquareHolder;

    /// <summary>
    /// Game over prefab, instantiated when the game is won/drawn/lost
    /// </summary>
    public GameObject GameOverPrefab;

    /// <summary>
    /// Main menu prefab, instantiated when the user clicks Main menu button.
    /// </summary>
    public GameObject MainMenuPrefab;

    /// <summary>
    /// Main menu prefab, instantiated when the user clicks Main menu button.
    /// </summary>
    public GameObject ChessPiecePrefab;

    /// <summary>
    /// Is it whites turn to move? if false then its blacks turn.
    /// </summary>
    public bool whiteTurnToMove = true;

    /// <summary>
    /// Should the last move be highlighted on the board?
    /// </summary>
    public bool highlightLastMove = true;

    /// <summary>
    /// Was last move succesfull?
    /// </summary>
    public bool moveDone = false;

    /// <summary>
    /// Should the all legal moves be highlighted when the player drags a piece?
    /// </summary>
    public bool highlightLegalMoves = true;

    /// <summary>
    /// Should the pieces be displayed as 3d pieces?
    /// </summary>
    /// <summary>
    /// All possible board modes.
    /// </summary>
    public enum BoardMode
    {
        Undefined,
        PlayerVsPlayer,
        PlayerVsEngine,
        EngineVsPlayer,
        EngineVsEngine,
        Multiplayer
    }

    /// <summary>
    /// The current board mode.
    /// </summary>
    public BoardMode Mode = BoardMode.PlayerVsEngine;

    /// <summary>
    /// What location should dead black pieces be placed at?
    /// </summary>
    public Vector3 blackDeadPiecesLocation = new Vector3(4, -2, 0);

    /// <summary>
    /// What location should dead white pieces be placed at?
    /// </summary>
    public Vector3 whiteDeadPiecesLocation = new Vector3(3, -2, 0);

    /// <summary>
    /// Which notationtype should be used to notate the game?
    /// </summary>
    public cgNotation.NotationType NotationType = cgNotation.NotationType.Algebraic;

    /// <summary>
    /// Should the early moves of the engine be randomized slightly to allow for a wide array of variations?
    /// </summary>
    public bool randomizeEarlyEngineMoves = true;

    /// <summary>
    /// Should the board start on load, or do you want to manually start the board by called Start?
    /// </summary>
    public bool startOnLoad = true;

    /// <summary>
    /// What depth should the engine search seemingly weak moves to? The higher the value the better the AI and the longer the load time.
    /// </summary>
    public byte searchDepthWeak = 4;

    /// <summary>
    /// What depth should the engine search seemingly strong moves to? The higher the value the better the AI and the longer the load time.
    /// </summary>
    public byte searchDepthStrong = 4;

    /// <summary>
    /// Should the engine use multithreading? Please note: Multithreading is not supported on some platforms such as WebGL.
    /// </summary>
    public bool useMultiThreading = true;

    /// <summary>
    /// The instance of the game over screen.
    /// </summary>
    private GameObject _gameOverScreen;

    /// <summary>
    /// This is the underlying board representation, we test and find legality of moves on this.
    /// </summary>
    [SerializeField] protected cgBoard _abstractBoard = null; //new cgBoard();

    [SerializeField] private cgBoard _lastAbstractBoard = null;

    public List<sbyte[]> charactersPositions = new List<sbyte[]>();
    // [SerializeField] public sbyte[] charactersPosition;

    /// <summary>
    /// All currently captured pieces.
    /// </summary>
    private List<cgChessPieceScript> _deadPieces = new List<cgChessPieceScript>();

    /// <summary>
    /// All currently uncaptured pieces on the board.
    /// </summary>
    private List<cgChessPieceScript> _livePieces = new List<cgChessPieceScript>();

    /// <summary>
    /// The AI opponent
    /// </summary>
    private cgEngine _engine;

    /// <summary>
    /// Number of dead white pieces.
    /// </summary>
    private int _deadWhitePieces = 0;

    /// <summary>
    /// Number of dead black pieces.
    /// </summary>
    private int _deadBlackPieces = 0;

    /// <summary>
    /// The current piece being dragged by the mouse.
    /// </summary>
    private cgChessPieceScript _downPiece;

    /// <summary>
    /// The next piece being dragged by the mouse.
    /// </summary>
    /// <summary>
    /// Logged moves, used by coordinate notation.
    /// </summary>
    private int _loggedMoves = 0;

    /// <summary>
    /// Chees was taken.
    /// </summary>
    private bool chessTaken;


    /// <summary>
    /// Was tap.
    /// </summary>
    private bool buttonPressed;

    /// <summary>
    /// Offset from tap position.
    /// </summary>
    private Vector3 tapOffset;

    /// <summary>
    /// Offset frop tap position tap was created.
    /// </summary>
    private bool tapOffsetCreated;

    private cgSquareScript[] _squares;

    private Camera _camera;
    private FigureHolder[] _figureHolders;

    private List<List<cgSimpleMove>> _engineCallbackParams = new List<List<cgSimpleMove>>();
    private List<Action<List<cgSimpleMove>>> _engineCallbackFunctions = new List<Action<List<cgSimpleMove>>>();
    private List<float> _engineProgress = new List<float>();
    private BackgroundWorker _engineThread;
    private Stopwatch stopwatch;
    private SecondScenePositionController _scenePositionController;
    private FigureHolder lastFigureHolder;

    [SerializeField] private CharacterPanel characterPanel;
    [SerializeField] private AchievementManager achievementManager;
    [SerializeField] private Transform figuresHolder;
    [SerializeField] protected CharacterShower characterShower;
    [SerializeField] public NetworkManager networkManager;
    private bool _gameEnded;

    /// <summary>
    /// Can the player drag and move pieces? Yes if a human controls the current color whoms turn it is to move.
    /// </summary>
    public bool playerCanMove
    {
        //get
        //{
        //    return ((_humanPlayerIsBlack && !whiteTurnToMove) || (_humanPlayerIsWhite && whiteTurnToMove))
        //        ? true
        //        : false;
        //}
        get
        {
            if (_gameEnded)
                return false;
            if (Mode == BoardMode.Multiplayer)
            {
                //Debug.Log("Actor num = " + PhotonNetwork.LocalPlayer.ActorNumber + "    white turn to move = " + whiteTurnToMove);
                return (PhotonNetwork.LocalPlayer.ActorNumber == 1 && whiteTurnToMove) ||
                       (PhotonNetwork.LocalPlayer.ActorNumber == 2 && !whiteTurnToMove);
            }
            else
            {
                return (_humanPlayerIsBlack && !whiteTurnToMove) || (_humanPlayerIsWhite && whiteTurnToMove);
            }
        }
    }

    /// <summary>
    /// Get the engine.
    /// </summary>
    public cgEngine getEngine
    {
        get { return _engine; }
    }

    /// <summary>
    /// Is it a human playing the black pieces? Determined by the current boardmode.
    /// </summary>
    private bool _humanPlayerIsBlack
    {
        get { return ((Mode == BoardMode.EngineVsPlayer) || (Mode == BoardMode.PlayerVsPlayer)); }
    }

    /// <summary>
    /// Is it a human playing white? Determined by the current boardmode.
    /// </summary>
    private bool _humanPlayerIsWhite
    {
        get { return ((Mode == BoardMode.PlayerVsEngine) || (Mode == BoardMode.PlayerVsPlayer)); }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Start the game, with provided board.
    /// </summary>
    /// <param name="customBoard">The abstract board that we should match, if none specified we use existing one, if none exists we generate an 8x8 board</param>
    /// <param name="mode">Which mode the game is in, player vs player, player vs engine, engine vs engine etc.</param>
    [PunRPC]
    public void start(cgBoard customBoard = null, BoardMode mode = BoardMode.Undefined)
    {
        _abstractBoard.WhiteTurnToMove = true;
        whiteTurnToMove = true;
        promotionController.SetBoard(_abstractBoard);
        _scenePositionController = FindObjectOfType<SecondScenePositionController>();

        if (customBoard == null)
        {
            if (this._abstractBoard != null)
            {
                customBoard = this._abstractBoard;
            }
            else
            {
                customBoard = new global::cgBoard();
            }
        }


#if UNITY_WEBGL
            this.useMultiThreading = false;
#endif

        _squares = getSquares();
        _figureHolders = getFigureHolders();
        Mode = (mode != BoardMode.Undefined ? mode : Mode);
        _engine = new cgEngine(searchDepthWeak, searchDepthStrong);
        //UnityEngine.Debug.Log(customBoard.boardToNeatString());
        setBoardTo(customBoard);
        //photonView.RPC("setBoardTo", RpcTarget.All, customBoard);

        //Determine if engine should make a starting move based on Mode.
        if (Mode == BoardMode.PlayerVsEngine && !whiteTurnToMove)
            MakeEngineMove(_abstractBoard.duplicate(), false, _engineCallback);
        else if (Mode == BoardMode.EngineVsPlayer && whiteTurnToMove)
            MakeEngineMove(_abstractBoard.duplicate(), true, _engineCallback);
        else if (Mode == BoardMode.EngineVsEngine) MakeEngineMove(_abstractBoard.duplicate(), true, _engineCallback);

        if(Mode==BoardMode.Multiplayer)
        {
            FindObjectOfType<FatalityController>().ONSkip = null;
            FindObjectOfType<FatalityController>().ONSkip += SendRPCSkip;
        } 
        //Flip camera view for black turn player
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            _camera.GetComponent<CameraController>().SetFirstCameraPosition(true);
        }

        ClearCharacterPositions();
        RememberCharactersPotions();
    }

    /// <summary>
    /// Get all cgSquareScripts.
    /// </summary>
    /// <returns>All cgSquareScripts</returns>
    public cgSquareScript[] getSquares()
    {
        return chessSquareHolder.GetComponentsInChildren<cgSquareScript>();
    }

    public FigureHolder[] getFigureHolders()
    {
        FigureHolder[] newChessSquareHolder = chessSquareHolder.GetComponentsInChildren<FigureHolder>();
        return newChessSquareHolder;
    }

    /// <summary>
    /// Place pieces according to the current state of the underlying abstractboard.
    /// </summary>
    [PunRPC]
    public void placePieces()
    {
        //Collect existing pieces, we instantiate new ones if we run out of existing pieces below.
        List<cgChessPieceScript> pieces = new List<cgChessPieceScript>();
        foreach (cgChessPieceScript piece in chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>(true))
            pieces.Add(piece);

        // UnityEngine.Debug.Log("Place pieces called. " + pieces.Count + " " + chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>().Length+" "+_abstractBoard.squares.Count);
        for (byte i = 0; i < _abstractBoard.squares.Count; i++)
        {
            if (_abstractBoard.squares[i] != 0)
            {
                if (pieces.Count == 0)
                {
                    //We instantiate more pieces if we run out of pieces to place.
                    cgChessPieceScript newPiece =
                        GameObject.Instantiate(ChessPiecePrefab).GetComponent<cgChessPieceScript>();
                    newPiece.gameObject.transform.parent = chessPieceHolder.transform;
                    newPiece.gameObject.SetActive(true);
                    pieces.Add(newPiece);
                    //UnityEngine.Debug.Log("Instantiating piece");
                }

                cgChessPieceScript piece = pieces[pieces.Count - 1];
                //UnityEngine.Debug.Log("i: "+i+" squarenames: "+_abstractBoard.SquareNames.Length);
                piece.StartAtSquare(_getSquare(_abstractBoard.SquareNames[i]));
                _livePieces.Add(piece);
                piece.dead = false;
                piece.SetType((int) _abstractBoard.squares[i]);
                pieces.Remove(piece);
                //UnityEngine.Debug.Log("Finding piece for " + _abstractBoard.squares[i]);
            }
        }

        //Unused pieces
        foreach (cgChessPieceScript piece in pieces)
        {
            DestroyImmediate(piece.gameObject);
            //piece.gameObject.SetActive(false);
        }

        cgChessPieceScript[] allPieces = chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>();
        foreach (cgChessPieceScript piece in allPieces)
        {
            if (piece.square == null)
                Destroy(piece.gameObject);
        }

        //Give all active pieces callback functions for mouse events.
        foreach (cgChessPieceScript piece in _livePieces)
        {
            piece.SetCallbacks(_pieceDown, _pieceUp);
            // piece.FlagSetup();
        }
    }

    public void ClearCharacterPositions()
    {
        charactersPositions = new List<sbyte[]>();
    }
    public void RememberCharactersPotions()
    {
        List<cgChessPieceScript> pieces = new List<cgChessPieceScript>();
        foreach (cgChessPieceScript piece in chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>(true))
            pieces.Add(piece);
        sbyte[] newCharactersPosition = new sbyte[_abstractBoard.squares.Count];
        // UnityEngine.Debug.Log("Place pieces called. " + pieces.Count + " " + chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>().Length+" "+_abstractBoard.squares.Count);
        for (byte i = 0; i < _abstractBoard.squares.Count; i++)
        {
            if (_abstractBoard.squares[i] != 0)
            {
                newCharactersPosition[i] = (sbyte) (_getPieceOn((_abstractBoard.SquareNames[i])).figureArmyIndex);
            }
            else
            {
                newCharactersPosition[i] = -1;
            }
        }

        //charactersPosition = newCharactersPosition;
        charactersPositions.Add(newCharactersPosition);
 
    }

    public figureMover GetCharacterByMove(int moveNumber,int moveTo,List<int> promotionTypes)
    {
        // List<figureMover> figureMovers = figuresHolder.GetComponentsInChildren<figureMover>(true).ToList();
        // figureMovers.Reverse();

   

        bool promotionSkipped = false;
        foreach (figureMover character in figuresHolder.GetComponentsInChildren<figureMover>(true))
        {

            if (character.figureIndex == charactersPositions[moveNumber][moveTo])
            {
                Debug.Log("22222");
                Debug.Log(charactersPositions[moveNumber][moveTo]);
                bool promotioned=false;
                
                foreach (var promotionType in promotionTypes)
                {
                    if (promotionType == character.figureIndex)
                    {
                        promotioned = true;
                        break;
                    }
                }

              
                Debug.Log(character.figureIndex);
                if (moveNumber % 2 == 0 && (int) character.role < 0)
                {  if (!promotionSkipped&&promotioned)
                    {
                        promotionSkipped = true;
                        continue;
                    }
                    return character;
                }
                if (moveNumber % 2 == 1 && (int) character.role > 0)
                {  if (!promotionSkipped&&promotioned)
                    {
                        promotionSkipped = true;
                        continue;
                    }
                    return character;
                }
                    
            }
        }

        return null;
    }
    public void SetCharactersIndexes(int moveNumber)
    {
        for (byte i = 0; i < _abstractBoard.squares.Count; i++)
        {
            if (_abstractBoard.squares[i] != 0)
            {
                cgChessPieceScript piece = _getPieceOn((_abstractBoard.SquareNames[i]));
                piece.figureArmyIndex = charactersPositions[moveNumber][i]; 
            }
        }
    }

    public void PlaceFigures()
    {
        foreach (cgChessPieceScript piece in _livePieces)
        {
            figureMover figureMover = piece.square.GetComponent<FigureHolder>().figure;
            if (figureMover)
                figureMover.transform.position = piece.transform.position;
        }
    }

    public List<cgSimpleMove> GetMoves()
    {
        return _abstractBoard.moves;
    }

    #endregion

    #region Unity messages

    // Use this for initialization
    void Awake()
    {
        _camera = Camera.main;

        if (startOnLoad)
        {
            // UnityEngine.Debug.Log("Abstract board: " + this._abstractBoard);
            // UnityEngine.Debug.Log("board prop: " + this._abstractBoard.boardHeight);
            // foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(_abstractBoard))
            // {
            //     string name = descriptor.Name;
            //     object value = descriptor.GetValue(_abstractBoard);
            //     Console.WriteLine("{0}={1}", name, value);
            // }

            // List<sbyte> _placements =
            // new List<sbyte>{
            //                 -2,  0,  0, -2,  0, -3, -6,  0,
            //                 0, -1,  0,  0,  0, -1, -1,  0,
            //                 0,  0, -1, -4,  0,  0,  0,  0,
            //                 1,  0,  0, -3,  0,  0,  1,  0,
            //                 0,  0,  0, -5,  0,  0,  0,  0,
            //                 1,  0,  3,  0,  0,  5,  0,  1,
            //                 4,  0,  0,  3,  0,  2,  0,  0,
            //                 2,  0,  0,  0,  0,  0,  6,  0
            //                 };
            // this._abstractBoard = new cgBoard(_placements);
            this._abstractBoard.generateHypotheticalMoves();
            start(this._abstractBoard);
        }
    }

    // Update is called once per frame
    void Update()
    {
        lock (_engineCallbackParams)
        {
            if (_engineCallbackParams.Count > 0)
            {
                //
                //UnityEngine.Debug.Log(": " + _engineCallbackParams.Count + " is it more than 0 ? " +(_engineCallbackParams.Count > 0));
                _engineCallbackFunctions[0](_engineCallbackParams[0]);
                _engineCallbackParams.RemoveAt(0);
                _engineCallbackFunctions.RemoveAt(0);
                //threadListener.Stop();
            }
        }

        lock (_engineProgress)
        {
            if (_engineProgress.Count > 0 && engineProgressBar != null)
            {
                // Debug.Log("bbbbbb");
                float progress = _engineProgress[0];
                _engineProgress.RemoveAt(0);
                Vector3 nscale = engineProgressBar.transform.localScale;
                nscale.x = progress;
            }
        }

        foreach (cgChessPieceScript cp in _livePieces)
        {
            if (cp.dead && !_deadPieces.Contains(cp)) _setDeadPiece(cp);
        }

        for (int i = _deadPieces.Count; i > 0; i--)
        {
            if (_livePieces.Contains(_deadPieces[i - 1])) _livePieces.Remove(_deadPieces[i - 1]);
        }

        if (_downPiece != null && buttonPressed)
        {
            //Debug.Log("A1");
            if (lastFigureHolder && lastFigureHolder.GetMovingBool())
            {
                return;
            }

            Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            Ray cursorRay = _camera.ScreenPointToRay(cursorPoint);
            RaycastHit hit;
            if (Physics.Raycast(cursorRay, out hit, 100.0f))
            {
                //Debug.Log("A2");
                _downPiece.transform.position =
                    new Vector3(hit.point.x, _downPiece.transform.position.y, hit.point.z);
            }

            //
            // if (chessTaken)
            // {
            _pieceUp(_downPiece);
            if (moveDone)
            {
                //Debug.Log("A3");
                chessTaken = false;
                // if (_newPiece && _newPiece.square)
                //     _pieceDown(_newPiece);
            }

            // }
        }

        if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.LeftControl)) _copyGameToClipboard();
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C)) _copyGameToClipboard();


        if (Input.GetKey(KeyCode.V) && Input.GetKeyDown(KeyCode.LeftControl)) _pasteGameFromClipboard();
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V)) _pasteGameFromClipboard();

        if (Input.GetMouseButtonDown(0)&&!GameManager.Pause)
        {
            buttonPressed = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            buttonPressed = false;
        }

        if (Input.GetKey(KeyCode.F1)) UnityEngine.Debug.Break();
        // if (Input.GetKey(KeyCode.A))
        // {
        //     //_abstractBoard.boardToString()
        //     UnityEngine.Debug.Log(_abstractBoard.boardToString());
        //     //UnityEngine.Debug.Log(_abstractBoard._blackKingStart);
        // }

        if (Input.GetKey(KeyCode.U))
        {
            _abstractBoard.revert();
            UnityEngine.Debug.Log(_abstractBoard.boardToString());
        }

        if (Input.GetKey(KeyCode.V))
        {
            //Perform long castling move.
            List<cgSimpleMove> moves = _abstractBoard.findStrictLegalMoves(_abstractBoard.WhiteTurnToMove);
            cgSimpleMove move = null;
            foreach (cgSimpleMove mv in moves)
            {
                if (mv is cgCastlingMove)
                {
                    if (move == null)
                    {
                        move = mv;
                    }
                    else if (Math.Abs(move.to - move.from) < Math.Abs(mv.to - mv.to))
                    {
                        move = mv;
                    }
                }
            }

            _makeMove(move);
        }
    }

    #endregion

    #region UI Button functions

    /// <summary>
    /// Resets the board, called by the menu button called restart.
    /// </summary>
    public void ResetBoard()
    {
        //UnityEngine.Debug.Log("reset");
        cgBoard newboard = _abstractBoard.duplicate().revertToStart();

        if (_gameOverScreen != null)
        {
            //UnityEngine.Debug.Log("destroying:" + _gameOverScreen);
            GameObject.Destroy(_gameOverScreen);
            _gameOverScreen = null;
        }

        start(newboard, Mode);
    }

    /// <summary>
    /// Reverts the last move, called by the menu button called revert last move.
    /// </summary>
    public void RevertLastMove()
    {
        //UnityEngine.Debug.Log("reverting");
        _abstractBoard.revert();
        setBoardTo(_abstractBoard);
        //photonView.RPC("setBoardTo", RpcTarget.All, _abstractBoard);
    }

    public void SaveBoard()
    {
        cgChessPieceScript[] allPieces = chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>(true);
        foreach (var cgPieceScript in allPieces)
        {
            cgPieceScript.gameObject.SetActive(true);
        }

        _lastAbstractBoard = _abstractBoard.duplicate();
    }

    public void PreviousMove()
    {
        _abstractBoard.revert();
        setBoardTo(_abstractBoard);
        SetCharactersIndexes(_abstractBoard.moves.Count);

        //photonView.RPC("setBoardTo", RpcTarget.All, _abstractBoard);
        //GetComponent<BoardCreator>().CreateBoard();
        GetComponent<BoardCreator>().SetupBoard();

        ShowLastMove();
    }

    public void NextMove()
    {
        if (_lastAbstractBoard.moves.Count <= _abstractBoard.moves.Count)
            return;
        _abstractBoard.revertToNext(_lastAbstractBoard.moves);
        setBoardTo(_abstractBoard);
        SetCharactersIndexes(_abstractBoard.moves.Count);

        //photonView.RPC("setBoardTo", RpcTarget.All, _abstractBoard);
        //GetComponent<BoardCreator>().CreateBoard();
        GetComponent<BoardCreator>().SetupBoard();
        Debug.Log("__");
        Debug.Log(_abstractBoard.moves.Count);
        //charactersPosition = charactersPositions[_abstractBoard.moves.Count];
        ShowLastMove();
    }

    public void GoToTheMove(int move, bool createBoard = false)
    {
        int changeMoveCount = _abstractBoard.moves.Count - move;
        if (changeMoveCount > 0)
        {
            for (int i = 0; i < changeMoveCount; i++)
            {
                _abstractBoard.revert();
            }
        }
        else
        {
            for (int i = 0; i < -changeMoveCount; i++)
            {
                _abstractBoard.revertToNext(_lastAbstractBoard.moves);
            }
        }

        setBoardTo(_abstractBoard);
        SetCharactersIndexes(move);
       
        GetComponent<BoardCreator>().SetupBoard();
       

        ShowLastMove();
    }

    private void PlaceAllFlags()
    {
        foreach (var chessPiece in chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>())
        {
            chessPiece.AnalysisFlagPosSetup((int) chessPiece.type);
            // chessPiece.SetFlagDown();
            // chessPiece.square.GetComponent<FigureHolder>().SetupFlagColorAndRotation((int)chessPiece.type);
        }
    }
    private void ShowLastMove()
    {
        PlaceAllFlags();
        UnhighlightMoves();
        if (_abstractBoard.moves.Count == 0)
        {
            return;
        }

        cgSimpleMove lastMove = _abstractBoard.moves[_abstractBoard.moves.Count - 1];
        cgSquareScript square = _getSquare(_abstractBoard.SquareNames[lastMove.to]);
        square.changeColor(square.recentMoveColor);

        square = _getSquare(_abstractBoard.SquareNames[lastMove.from]);
        square.changeColor(square.recentSelectedColor);
    }

    /// <summary>
    /// Instantiates and shows an instance of MainMenuPrefab. Called by menu button called Main Menu
    /// </summary>
    public void MainMenu()
    {
        if (_gameOverScreen != null)
        {
            GameObject.Destroy(_gameOverScreen);
            _gameOverScreen = null;
        }

        GameObject.Instantiate(MainMenuPrefab);
        GameObject.DestroyImmediate(gameObject);
    }

    /// <summary>
    /// Flips the board so if white is currently on top, it will be at the bottom. Preferable if playing as black.
    /// Called by the menu button called Flip Board.
    /// </summary>
    public void FlipBoard()
    {
        if (flipTarget != null)
        {
            int increment = 180;

            //Vector3 rotatePoint = new Vector3(0.000725314f, 0.001148298f, 0);
            //Vector3 rotatePoint = new Vector3(flipTarget.transform.position.x,flipTarget.transform.position.y, 0);
            flipTarget.transform.RotateAround(Vector3.zero, Vector3.forward, increment);
            //flipTarget.transform.rot
            //flipTarget.transform.Rotate(Vector3.forward, increment);
            foreach (cgChessPieceScript piece in _livePieces) piece.transform.Rotate(Vector3.forward, increment);
            foreach (cgChessPieceScript piece in _deadPieces) piece.transform.Rotate(Vector3.forward, increment);
        }
    }

    /// <summary>
    /// Use the engine to find a suggested move for the current position. Called by the menu button called Suggest Move.
    /// </summary>
    public void SuggestMove()
    {
        if (playerCanMove)
            MakeEngineMove(_abstractBoard.duplicate(), _abstractBoard.WhiteTurnToMove, _engineSuggestion);
    }

    private void _engineSuggestion(List<cgSimpleMove> moves)
    {
        if (playerCanMove)
        {
            if (_abstractBoard.verifyLegality(moves[0])) _suggestMove(moves[0]);
            else
            {
                moves.RemoveAt(0);
                if (moves.Count > 0) _engineSuggestion(moves);
            }
        }
    }

    private void _suggestMove(cgSimpleMove move)
    {
        cgSquareScript departingSquare = _getSquare(this._abstractBoard.SquareNames[move.from]);
        cgSquareScript arrivalSquare = _getSquare(this._abstractBoard.SquareNames[move.to]);

        departingSquare.highlightTemporarily(new Color(0, .5f, 0));
        arrivalSquare.highlightTemporarily(new Color(0, .5f, 0));
    }

    #endregion

    /// <summary>
    /// Play provided sound, adds an audiosource if one does not exist on this gameobject.
    /// </summary>
    /// <param name="clip"></param>
    public void playSound(AudioClip clip)
    {
        if (clip == null) return;
        if (GetComponent<AudioSource>() == null) gameObject.AddComponent<AudioSource>();
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Skip figure animations
    /// </summary>
    [PunRPC]
    public void SkipAnimations()
    {
        foreach (var holder in _figureHolders)
        {
            holder.SkipFigureAnimations();
        }
    }

    /// <summary>
    /// A piece has callbacked that the user has pressed it.
    /// </summary>
    /// <param name="piece"></param>
    private void _pieceDown(cgChessPieceScript piece)
    {
        if (characterShower)
        {
            figureMover curFigure = piece.square.GetComponent<FigureHolder>().figure;
            characterShower.ShowCurFigure(curFigure.thisCharacter );
            if(curFigure&&curFigure.thisCharacter)
            {
                characterPanel.ChooseCharacter(curFigure.thisCharacter,curFigure.playerType);
            }        
        }

        if (!playerCanMove)
        {
            //Debug.Log("B2");
            return;
        }

        if (lastFigureHolder && lastFigureHolder.moving)
        {
            if (Mode == BoardMode.Multiplayer)
            {
                photonView.RPC("SkipAnimations", RpcTarget.Others, new object[] { });
            }

            SkipAnimations();
            return;
        }

        if (chessTaken)
        {
            //Debug.Log("B4");
            return;
        }

        if (!piece)
        {
            //Debug.Log("B5");
            moveDone = true;
            _downPiece = null;
            buttonPressed = false;
            return;
        }

        if (highlightLastMove)
        {
            //revert colors if highlighting is active
            //Debug.Log("B6");

            foreach (cgSquareScript square in _squares)
            {
                //Debug.Log("B7");
                square.changeColor(square.startColor);
            }

            SetLastFlagDown();
        }

        _downPiece = null;


        // Added () to (_abstractBoard.whiteTurnToMove && piece.type > 0
        if (highlightLegalMoves && playerCanMove && _abstractBoard.WhiteTurnToMove && piece.type > 0 ||
            !_abstractBoard.WhiteTurnToMove && piece.type < 0)

        {
            List<cgSimpleMove> moves = _abstractBoard.findStrictLegalMoves(_abstractBoard.WhiteTurnToMove);
            foreach (cgSimpleMove move in moves)
            {
                if (_abstractBoard.SquareNames[move.from] == piece.square.uniqueName)
                {
                    if (move is cgCastlingMove)
                    {
                        //Highlighting rook instead of king destination when castling.
                        _getSquare(_abstractBoard.SquareNames[(move as cgCastlingMove).secondFrom]).changeColor(
                            _getSquare(_abstractBoard.SquareNames[(move as cgCastlingMove).secondFrom])
                                .legalMoveToColor);
                    }
                    else
                    {
                        //Debug.Log("A15");
                        _getSquare(_abstractBoard.SquareNames[move.to])
                            .changeColor(_getSquare(_abstractBoard.SquareNames[move.from]).legalMoveToColor);
                    }
                }
            }

            
            piece.square.changeColor(piece.square.recentSelectedColor);
            piece.SetFlagUpward();
        }

        if (chessTaken)
            return;
        _downPiece = piece;
    }

    /// <summary>
    /// The user has released a dragged piece. Verify that its a legal move, if so perform the move and perform the next move if appropriate mode.
    /// </summary>
    /// <param name="piece"></param>
    [PunRPC]
    private void _pieceUp(cgChessPieceScript piece)
    {
        moveDone = false;
        bool rightMove = false;
        if (_downPiece != null && !_gameEnded)
        {
            if (playerCanMove || Mode == BoardMode.PlayerVsPlayer)
            {
                cgSimpleMove legalMove = null;
                cgSquareScript closestSquare = _findSquareAt(_downPiece.transform.position);
                List<cgSimpleMove> legalMoves = _abstractBoard.findStrictLegalMoves(whiteTurnToMove);
                foreach (cgSimpleMove move in legalMoves)
                {
                    if (move is cgCastlingMove)
                    {
                        if (_abstractBoard.SquareNames[move.from] == _downPiece.square.uniqueName &&
                            _abstractBoard.SquareNames[(move as cgCastlingMove).secondFrom] == closestSquare.uniqueName)
                        {
                            legalMove = move;
                        }
                    }
                    else
                    {
                        if (_abstractBoard.SquareNames[move.from] == _downPiece.square.uniqueName &&
                            _abstractBoard.SquareNames[move.to] == closestSquare.uniqueName)
                        {
                            legalMove = move;
                        }
                    }
                }
                //test legality of move here.

                if (legalMove != null && _abstractBoard.verifyLegality(legalMove))
                {
                    rightMove = true;
                    _abstractBoard.curPromotion = 5;

                    StartCoroutine(_pieceUpping(legalMove));
                }
                else
                {
                    PieceMoveToSquare(piece, piece.square);

                    //piece.moveToSquare(piece.square);
                }
            }
            else
            {                    
                PieceMoveToSquare(piece, piece.square);

                //piece.moveToSquare(piece.square);
            }

            if (chessTaken && piece != _downPiece && !rightMove)
            {
                PieceMoveToSquare(piece, piece.square);

               // piece.moveToSquare(piece.square);
                if (_downPiece)
                {
                    _downPiece.SetFlagDown();
                }

                _downPiece = null;
                chessTaken = false;
                _pieceDown(piece);
            }
        }
        else
        {
            PieceMoveToSquare(piece, piece.square);

            //piece.moveToSquare(piece.square);
        }

        buttonPressed = false;
        if (_downPiece)
        {
            chessTaken = true;
        }
    }

    [PunRPC]
    public void CreateRPCMove(cgSimpleMove legalMove)
    {
        if (legalMove is cgCastlingMove)
        {
            photonView.RPC("RPC_makeCaslingMove", RpcTarget.AllBuffered, new object[]
            {
                (byte) legalMove.capturedType, legalMove.from, (byte) legalMove.positionalVal,
                ((cgCastlingMove) legalMove).secondFrom, ((cgCastlingMove) legalMove).secondTo, legalMove.promotioned,
                (byte) legalMove.promotionedType, legalMove.to, legalMove.val,legalMove.fatalityIndex
            });
        }
        else if (legalMove is cgEnPassantMove)
        {
            photonView.RPC("RPC_makeEnPassantMove", RpcTarget.AllBuffered, new object[]
            {
                (byte) legalMove.capturedType, legalMove.from, (byte) legalMove.positionalVal,
                ((cgEnPassantMove) legalMove).attackingSquare, legalMove.promotioned, (byte) legalMove.promotionedType,
                legalMove.to, legalMove.val,legalMove.fatalityIndex
            });
        }
        else
        {
            photonView.RPC("RPC_makeSimpleMove", RpcTarget.AllBuffered, new object[]
            {
                (byte) legalMove.capturedType, legalMove.from, (byte) legalMove.positionalVal,
                legalMove.promotioned, (byte) legalMove.promotionedType, legalMove.to, legalMove.val,legalMove.fatalityIndex
            });
        }
    }

    [PunRPC]
    public IEnumerator _pieceUpping(cgSimpleMove legalMove)
    {
        if (Mode == BoardMode.Multiplayer)
        {
            if (legalMove.capturedType != 0 && _scenePositionController)
            {
                SecondScenePositionController.FatalityData fatalityData =
                    _scenePositionController.GenerateFatalityData();
                legalMove.fatalityIndex = _scenePositionController.GetFatalityIndex();

                photonView.RPC("SetFatalityData", RpcTarget.Others,
                    new object[] {fatalityData.fatalityPlaceIndex, fatalityData.fatalityRotation});
            }

            if (!(legalMove.to < _abstractBoard.GetWhitePromotionBelow() &&
                  _abstractBoard.squares[legalMove.from] == 1 ||
                  legalMove.to > _abstractBoard.GetBlackPromotionAbove() &&
                  _abstractBoard.squares[legalMove.from] == -1))
            {
                CreateRPCMove(legalMove);
            }
            else
            {
                photonView.RPC("SetPromotionChoosing", RpcTarget.AllBuffered,
                    new object[] {legalMove.from, legalMove.to});
                CreateRPCMove(legalMove);
                promotionController.OpenPromotionArea();
                yield return null;
            }
        }
        else
        {
            _makeMove(legalMove);
        }

        moveDone = true;
        if (highlightLastMove)
        {
            //revert colors if highlighting is active
            foreach (cgSquareScript square in _squares) square.changeColor(square.startColor);
        }

        _downPiece.SetFlagDown();
        _downPiece = null;
        while (promotionController.figureChoosing)
        {
            yield return null;
        }
        if (Mode == BoardMode.PlayerVsEngine)
            MakeEngineMove(_abstractBoard.duplicate(), false, _engineCallback);
        else if (Mode == BoardMode.EngineVsPlayer)
            MakeEngineMove(_abstractBoard.duplicate(), true, _engineCallback);
    }

    public void SendRPCSkip()
    {
        photonView.RPC("SkipAnimationsMultiplayer", RpcTarget.Others, new object[] { });
    }

    [PunRPC]
    public void SkipAnimationsMultiplayer()
    {
        FindObjectOfType<FatalityController>().SkipAnimations();
    }

    /// <summary>
    /// Find the square location at the provided position, used to find the square where the user is dragging and dropping a piece.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private cgSquareScript _findSquareAt(Vector3 position)
    {
        float best = Vector3.Distance(position, _squares[0].transform.position);
        cgSquareScript square = _squares[0];

        foreach (cgSquareScript candsquare in _squares)
        {
            if (Vector3.Distance(position, candsquare.transform.position) < best)
            {
                best = Vector3.Distance(position, candsquare.transform.position);
                square = candsquare;
            }
        }

        return square;
    }

    /// <summary>
    /// The engine returns with its prefered move.
    /// </summary>
    /// <param name="move">The prefered move.</param>
    private void _engineCallback(List<cgSimpleMove> moves)
    {
        if (!playerCanMove)
        {
            cgSimpleMove move = moves[0];
            if (this._abstractBoard.moves.Count < 10 && randomizeEarlyEngineMoves)
            {
                //We are in the very early game and we should randomize the engine moves early, so we will make a random-ish choice here instad of the prefered choice.
                //random-ish in the sense, that we will take a random move of one of the moves that are in the better half of all possible moves.

                List<cgSimpleMove> candidates = _findAllMovesNear(moves, 200);
                int choice = UnityEngine.Random.Range(0, candidates.Count);

                move = moves[choice];
            }

            if (_abstractBoard.verifyLegality(move)) _makeMove(move);
            else
            {
                moves.Remove(move);
                if (_engine.moves.Count > 0) _engineCallback(moves);
                else
                {
                    _checkGameOver();
                }
            }
        }
    }

    private List<cgSimpleMove> _findAllMovesNear(List<cgSimpleMove> moves, int spill)
    {
        List<cgSimpleMove> candidates = new List<cgSimpleMove>();
        int best = moves[0].val;
        foreach (cgSimpleMove mv in moves)
        {
            if (Math.Abs(mv.val - best) < spill)
                candidates.Add(mv);
        }

        return candidates;
    }

    /// <summary>
    /// Game over, instantiate the game over screen and let it display the provided message.
    /// </summary>
    /// <param name="display"></param>
    private void _gameOver(bool whiteWins, bool blackWins)
    {
        SetGameEnded();

        Debug.Log(("_____"));
        Debug.Log((movesMade-1));
        Debug.Log(GetMoves()[movesMade-1].to);
        Debug.Log((_getSquare(_abstractBoard.SquareNames[GetMoves()[movesMade-1].to])));
        Debug.Log(_getSquare(_abstractBoard.SquareNames[GetMoves()[movesMade-1].to]).GetComponent<FigureHolder>().figure);
        var kingKiller=_getSquare(_abstractBoard.SquareNames[GetMoves()[movesMade-1].to]).GetComponent<FigureHolder>().figure;
        ExperienceAccounter.GainExperience(kingKiller,5);

        string gameOverString = "win";
        if (Mode == BoardMode.PlayerVsEngine)
        {
            if (whiteWins)
                gameOverString = "win";
            else if (blackWins)
                gameOverString = "defeat";
            else
                gameOverString = "draw";
        }
        else if (Mode == BoardMode.EngineVsPlayer)
        {
            if (whiteWins)
                gameOverString = "defeat";
            else if (blackWins)
                gameOverString = "win";
            else
                gameOverString = "draw";
        }
        else if (Mode == BoardMode.Multiplayer)
        {
            if (NetworkManager.EnemyPlayer.ActorNumber == 1)
            {
                if (whiteWins)
                    gameOverString = "defeat";
                else if (blackWins)
                    gameOverString = "win";
                else
                    gameOverString = "draw";
            }
            else
            {
                if (whiteWins)
                    gameOverString = "win";
                else if (blackWins)
                    gameOverString = "defeat";
                else
                    gameOverString = "draw";
            }
        }

        if (gameOverString == "win")
        {
            
            menuController.OpenWinScreen();
        }
        else if (gameOverString == "defeat")
        {
            menuController.OpenDefeatScreen();
        }
        else if (gameOverString == "draw")
        {
            menuController.OpenDrawScreen();
        }
        networkManager.LeaveRoom();

    }

    /// <summary>
    /// Check if the game is over, should be called after every move.
    /// </summary>
    private void _checkGameOver()
    {
        bool isChecked = _abstractBoard.isChecked(!whiteTurnToMove);
        List<cgSimpleMove> responses = _abstractBoard.findLegalMoves(_abstractBoard.WhiteTurnToMove);
        for (int i = responses.Count; i > 0; i--)
        {
            if (!_abstractBoard.verifyLegality(responses[i - 1]))
            {
                responses.RemoveAt(i - 1);
            }
        }

        if (responses.Count == 0 && isChecked)
        {
       
            //Checkmate.
            _gameOver(whiteTurnToMove, !whiteTurnToMove);
            if ((_humanPlayerIsBlack && whiteTurnToMove) || (_humanPlayerIsWhite && !whiteTurnToMove))
            {
                playSound(winSound);
            }
            else playSound(loseSound);
        }
        else if (isChecked)
        {           

            //Checked but not checkmate.
            playSound(checkSound);
        }
        
        else if (responses.Count == 0 && !isChecked)
        {          

            //Draw by stalemate - no legal moves available to player whose turn it is to move.
            // _gameOver(whiteTurnToMove, !whiteTurnToMove);
            _gameOver(false, false);
            playSound(loseSound);
        }
        else
        { 
            //Test Draw by material insuffience
            //Collect living non-king pieces, to test draw by material insuffience
            bool blackBishopsEvenSquares = false;
            bool blackBishopsUnevenSquares = false;
            bool blackKnights = false;
            bool whiteBishopsEvenSquares = false;
            bool whiteBishopsUnevenSquares = false;
            bool whiteKnights = false;

            bool drawByMaterial = false;
            bool hasEnoughOtherMaterial = false;
            for (byte i = 0; i < _abstractBoard.squares.Count; i++)
            {
                sbyte sb = _abstractBoard.squares[i];
                if (sb != 0 && Math.Abs(sb) != 6) //Count everything but empty squares and king pieces.
                {
                    if (sb == 3)
                        whiteKnights = true;
                    else if (sb == 4)
                    {
                        if (i / 2 == 0)
                            whiteBishopsEvenSquares = true;
                        else
                            whiteBishopsUnevenSquares = true;
                    }
                    else if (sb == -3)
                        blackKnights = true;
                    else if (sb == 4)
                    {
                        if (i / 2 == 0)
                            blackBishopsEvenSquares = true;
                        else
                            blackBishopsUnevenSquares = true;
                    }
                    else
                    {
                        //pawn, queen or rook is alive, no need to test further material insuffience
                        hasEnoughOtherMaterial = true;
                        break;
                    }
                }
            }

            if (!hasEnoughOtherMaterial)
            {
                //White bishop on even square only piece left and possibly other black bishops on even square(which doesnt matter.)
                if (whiteBishopsEvenSquares && !whiteBishopsUnevenSquares && !blackBishopsUnevenSquares &&
                    !blackKnights && !whiteKnights)
                    drawByMaterial = true;
                //White bishop on uneven square left and possibly other black bishops on uneven square(which doesnt matter.)
                else if (whiteBishopsUnevenSquares && !whiteBishopsEvenSquares && !blackBishopsEvenSquares &&
                         !blackKnights && !whiteKnights)
                    drawByMaterial = true;
                //black bishop on even square only piece left 
                else if (blackBishopsEvenSquares && !whiteBishopsEvenSquares && !whiteBishopsUnevenSquares &&
                         !blackBishopsUnevenSquares && !blackKnights && !whiteKnights)
                    drawByMaterial = true;
                //black bishop on uneven square only piece left.
                else if (blackBishopsUnevenSquares && !whiteBishopsEvenSquares && !whiteBishopsUnevenSquares &&
                         !blackBishopsEvenSquares && !blackKnights && !whiteKnights)
                    drawByMaterial = true;
                //black knight only piece left.
                else if (blackKnights && !whiteBishopsEvenSquares && !whiteBishopsUnevenSquares &&
                         !blackBishopsEvenSquares && !blackBishopsUnevenSquares && !whiteKnights)
                    drawByMaterial = true;
                //white knight only piece left.
                else if (whiteKnights && !blackKnights && !whiteBishopsUnevenSquares && !blackBishopsEvenSquares &&
                         !blackBishopsUnevenSquares && !blackKnights)
                    drawByMaterial = true;
                //no one piece left.
                //
                else if (!whiteKnights && !blackKnights && !whiteBishopsUnevenSquares && !whiteBishopsEvenSquares &&
                         !blackBishopsEvenSquares)
                    drawByMaterial = true;
            }

            if (drawByMaterial)
            {
                _gameOver(false, false);
                playSound(loseSound);
            }
        }
    }

    public void SetLastFlagDown()
    {
        if (_downPiece)
        {
            //Debug.Log("B8");
            _downPiece.SetFlagDown();
        }
    }
    /// <summary>
    /// Peform the provided move on the visual board and the abstract board, with no legality checks - thus should be performed prior to calling this.
    /// </summary>
    /// <param name="move"></param>
    public void _makeMove(cgSimpleMove move)
    {
        if (Mode != BoardMode.Multiplayer && move.capturedType != 0 && _scenePositionController)
        {
            _scenePositionController.GenerateFatalityData();
            move.fatalityIndex = _scenePositionController.GetFatalityIndex();
        }

        movesMade++;
        playSound(moveSound);
        move.time = DateTime.Now.ToString("HH:mm");
        StartCoroutine(TurnSwitch(move));
    }

    [PunRPC]
    private void RPC_makeSimpleMove(byte capturedType, byte from, byte positionalVal, Boolean promotioned,
        byte promotionedType, byte to, Int32 val,int fatalityIndex)
    {
        cgSimpleMove move = new cgSimpleMove(from, to, (sbyte) positionalVal);
        move.capturedType = (sbyte) capturedType;
        move.promotioned = promotioned;
        move.promotionedType = (sbyte) promotionedType;
        move.val = val;
        move.fatalityIndex = fatalityIndex;
        _makeMove(move);
    }

    [PunRPC]
    private void RPC_makeCaslingMove(byte capturedType, byte from, byte positionalVal, byte s_from, byte s_to,
        Boolean promotioned, byte promotionedType, byte to, Int32 val,int fatalityIndex)
    {
        cgCastlingMove move = new cgCastlingMove(from, to, (sbyte) positionalVal, s_from, s_to);
        move.capturedType = (sbyte) capturedType;
        move.promotioned = promotioned;
        move.promotionedType = (sbyte) promotionedType;
        move.val = val;
        move.fatalityIndex = fatalityIndex;

        _makeMove(move);
    }

    [PunRPC]
    private void RPC_makeEnPassantMove(byte capturedType, byte from, byte positionalVal, byte attackSquare,
        Boolean promotioned, byte promotionedType, byte to, Int32 val,int fatalityIndex)
    {
        cgEnPassantMove move = new cgEnPassantMove(from, to, (sbyte) positionalVal, attackSquare);
        move.capturedType = (sbyte) capturedType;
        move.promotioned = promotioned;
        move.promotionedType = (sbyte) promotionedType;
        move.val = val;
        move.fatalityIndex = fatalityIndex;

        _makeMove(move);
    }


    [PunRPC]
    public void SetPromotionIndex(int figure_index)
    {
        promotionController.curFigureIndex = figure_index;
        promotionController.figureChoosing = false;
    }

    [PunRPC]
    public void SetFatalityData(int fatalityPlaceIndex, int fatalityRotation)
    {
        _scenePositionController.SetFatalityData(fatalityPlaceIndex, fatalityRotation);
    }

    [PunRPC]
    public void SetPromotionChoosing(byte from, byte to)
    {
        cgChessPieceScript piece = _getPieceOn(_abstractBoard.SquareNames[from]);

        promotionController.RememberPromotion(_abstractBoard, piece, to);
    }


    private IEnumerator TurnSwitch(cgSimpleMove move)
    {
        cgChessPieceScript piece = _getPieceOn(_abstractBoard.SquareNames[move.from]);

        if (Mode == BoardMode.Multiplayer)
        {
            if (promotionController.figureChoosing)
            {
                promotionController.SetNewMove(move);

                while (promotionController.figureChoosing)
                {
                    yield return null;
                }

                move.promotionedType = (sbyte) promotionController.curFigureIndex;
                chessTaken = false;
            }
        }
        else
        {
            if (move.to < _abstractBoard.GetWhitePromotionBelow() && _abstractBoard.squares[move.from] == 1 ||
                move.to > _abstractBoard.GetBlackPromotionAbove() && _abstractBoard.squares[move.from] == -1)
            {
                promotionController.RememberPromotion(_abstractBoard, piece, move.to);
            }

            if (promotionController.figureChoosing)
            {
                promotionController.SetNewMove(move);
                promotionController.OpenPromotionArea();


                while (promotionController.figureChoosing)
                {
                    yield return null;
                    if (!playerCanMove)
                        promotionController.ChooseNewFigure(5);
                }

                move.promotionedType = _abstractBoard.curPromotion;
            }
        }

        _abstractBoard.move(move);

        move.promotioned = false;

        lastFigureHolder = _getSquare(_abstractBoard.SquareNames[move.to]).GetComponent<FigureHolder>();

        _writeLog(move);
        //_abstractBoard.debugReadBoard();
        if (_getPieceOn(_abstractBoard.SquareNames[move.to]) != null && !(move is cgCastlingMove))
        {
            _setDeadPiece(_getPieceOn(_abstractBoard.SquareNames[move.to]));
        }

        bool enPassant = false;
        piece = _getPieceOn(_abstractBoard.SquareNames[move.from]);


        if (move is cgCastlingMove)
        {
            cgChessPieceScript piece2 = _getPieceOn(_abstractBoard.SquareNames[(move as cgCastlingMove).secondFrom]);
            if (piece2)
            {
                PieceMoveToSquare(piece2, _getSquare(_abstractBoard.SquareNames[(move as cgCastlingMove).secondTo]));

                //piece2.moveToSquare(_getSquare(_abstractBoard.SquareNames[(move as cgCastlingMove).secondTo]));
            }
        }
        else if (move is cgEnPassantMove)
        {
            enPassant = true;
            cgChessPieceScript piece2 =
                _getPieceOn(_abstractBoard.SquareNames[(move as cgEnPassantMove).attackingSquare]);
            PieceMoveToSquare(piece, _getSquare(_abstractBoard.SquareNames[move.to]), _getSquare(_abstractBoard.SquareNames[(move as cgEnPassantMove).attackingSquare]));

           // piece.moveToSquare(_getSquare(_abstractBoard.SquareNames[move.to]), _getSquare(_abstractBoard.SquareNames[(move as cgEnPassantMove).attackingSquare]));
            piece2.dead = true;
        }

        if (!enPassant && piece != null)
        {
            PieceMoveToSquare(piece, _getSquare(_abstractBoard.SquareNames[move.to]));
           // piece.moveToSquare(_getSquare(_abstractBoard.SquareNames[move.to]));
        }

        if (lastFigureHolder)
        {
            while (lastFigureHolder.GetMovingBool())
            {
                yield return null;
            }
        }

        promotionController.ChangeCharacter();
        RememberCharactersPotions();

        _checkGameOver();
        if (highlightLastMove)
        {
            //Color color = _getSquare(_abstractBoard.SquareNames[move.to]).recentMoveColor;
            ShowLastMove();
           // _getSquare(_abstractBoard.SquareNames[move.to]).highlightTemporarily(color);
        }

        //Debug.Log("C2 before whiteturn = " + whiteTurnToMove);
        whiteTurnToMove = _abstractBoard.WhiteTurnToMove; // +++
        //Debug.Log("C3 after whiteturn = " + whiteTurnToMove);
        if (Mode == BoardMode.EngineVsEngine)
            MakeEngineMove(_abstractBoard.duplicate(), _abstractBoard.WhiteTurnToMove, _engineCallback);
    }

    private void PieceMoveToSquare(cgChessPieceScript piece,cgSquareScript newSquare, cgSquareScript enPassantSquare = null)
    {
        FigureHolder figureHolder = piece.square.GetComponent<FigureHolder>();
        FigureHolder newFigureHolder = newSquare.GetComponent<FigureHolder>();


        if(piece.square!=newSquare&&figureHolder.HaveTargetFigure()&&newFigureHolder.HaveTargetFigure())
        {
            ExperienceAccounter.GainKillExperience(figureHolder.figure, newFigureHolder.figure.thisCharacter);
            achievementManager.CheckKillAchievement(figureHolder.figure, newFigureHolder.figure.thisCharacter);
            if (figureHolder.figure.playerType == PlayerType.Player)
            {
                CharacterProgress.saveCharacterProgress(figureHolder.figure.thisCharacter.characterProgress,"Character" + PlayerData.Account.AccountIndex + figureHolder.figure.thisCharacter.index,PlayerData.Account.AccountIndex);

            }
        }  
        piece.moveToSquare(newSquare,enPassantSquare);
    }
    /// <summary>
    /// Called when the engine should generate a new move.
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="moveAsWhiteP">Move as white(true) or black(false).</param>
    /// <param name="callback">Where the prefered move will be returned.</param>
    public void MakeEngineMove(cgBoard board, bool moveAsWhiteP, Action<List<cgSimpleMove>> callback)
    {
        StartCoroutine(WaitMakeEngineMove(board, moveAsWhiteP, callback));
    }

    private IEnumerator WaitMakeEngineMove(cgBoard board, bool moveAsWhiteP, Action<List<cgSimpleMove>> callback)
    {
        if (lastFigureHolder)
        {
            while (lastFigureHolder.GetMovingBool())
            {
                yield return null;
            }
        }

        if (promotionController.figureChoosing)
        {
            while (promotionController.figureChoosing)
            {
                yield return null;
            }
        }

        _engineCallbackFunctions.Add(callback);
        if (useMultiThreading)
        {
            this._makeEngineMoveMulti(board, moveAsWhiteP, callback);
        }
        else
        {
            this._makeEngineMoveMono(board, moveAsWhiteP, callback);
        }
    }

    /// <summary>
    /// Called when the engine should generate a new move using a new thread(multi threaded)..
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="moveAsWhiteP">Move as white(true) or black(false).</param>
    /// <param name="callback">Where the prefered move will be returned.</param>
    private void _makeEngineMoveMulti(cgBoard board, bool moveAsWhiteP, Action<List<cgSimpleMove>> callback)
    {
        if (_engineThread != null)
        {
            _engineThread.CancelAsync();
        }

        _engineThread = new BackgroundWorker();
        //The thread for the engine to make its computations to decide on a move using a new thread(multi threaded).
        Action<object, DoWorkEventArgs> _threadMakeMove = (object sender,
            DoWorkEventArgs e) =>
        {
            Action<List<cgSimpleMove>> completeCallback = (List<cgSimpleMove> moves) =>
            {
                lock (_engineCallbackParams)
                {
                    _engineCallbackParams.Add(moves);
                }
            };
            Action<float> progessCallback = (float progress) =>
            {
                lock (_engineProgress)
                {
                    _engineProgress.Add(progress);
                }
            };

            _engine.makeMove(board, moveAsWhiteP, completeCallback, progessCallback);
        };
        _engineThread.DoWork += new DoWorkEventHandler(_threadMakeMove);
        _engineThread.RunWorkerCompleted += _engineThread_RunWorkerCompleted;
        _engineThread.ProgressChanged += _engineThread_ProgressChanged;
        _engineThread.WorkerSupportsCancellation = true;

        _engineThread.RunWorkerAsync();
    }

    /// <summary>
    /// Called when the engine should generate a new move using corutine on a singlethread(mono threaded).
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="moveAsWhiteP">Move as white(true) or black(false).</param>
    /// <param name="callback">Where the prefered move will be returned.</param>
    public void _makeEngineMoveMono(cgBoard board, bool moveAsWhiteP, Action<List<cgSimpleMove>> callback)
    {
        Action<List<cgSimpleMove>> completeCallback = (List<cgSimpleMove> moves) =>
        {
            lock (_engineCallbackParams)
            {
                UnityEngine.Debug.Log("Callback and added moves: " + moves.Count);
                _engineCallbackParams.Add(moves);
            }
        };
        Action<float> progessCallback = (float progress) =>
        {
            lock (_engineProgress)
            {
                _engineProgress.Add(progress);
            }
        };
        _engine.makeMoveMono(this._startCoroutine, board, moveAsWhiteP, completeCallback, progessCallback);
    }

    private void _startCoroutine(IEnumerator ienum)
    {
        StartCoroutine(ienum);
    }

    private void OnApplicationQuit()
    {
        //_engineThread.CancelAsync();
    }

    private void _engineThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        Vector3 nscale = this.engineProgressBar.transform.localScale;
        nscale.x = e.ProgressPercentage;
    }

    private void _engineThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
//        UnityEngine.Debug.Log("Worker thread done and done. " + e.Error.Message + " " + e.Error.StackTrace);
    }

    private cgSquareScript _getSquare(string p)
    {
        foreach (cgSquareScript square in getSquares())
            if (square != null && square.uniqueName == p)
                return square;
        return null;
    }

    private cgChessPieceScript _getPieceOn(string p)
    {
        foreach (cgChessPieceScript cp in _livePieces)
            if (cp.square != null && cp.square.uniqueName == p)
                return cp;
        return null;
    }

    private void _setDeadPiece(cgChessPieceScript cp)
    {
        cp.dead = true;
        cp.square = null;
        if (!_deadPieces.Contains(cp))
        {
            if (cp.white)
            {
                cp.gameObject.SetActive(false);
            }
            else if (!cp.white)
            {
                cp.gameObject.SetActive(false);
            }

            cp.flag.gameObject.SetActive(false);
            _deadPieces.Add(cp);
            if (cp.white) _deadWhitePieces++;
            else _deadBlackPieces++;
        }
    }

    /// <summary>
    /// Paste the game notation from clipboard onto the board.
    /// </summary>
    private void _pasteGameFromClipboard()
    {
        string curgame = GUIUtility.systemCopyBuffer;
        _abstractBoard = new cgBoard();
        UnityEngine.Debug.Log("Pasted game from clipboard: " + curgame);
        cgNotation notation = new cgNotation(_abstractBoard);
        notation.Read(curgame);

        _abstractBoard.LoadGame(notation);
        setBoardTo(_abstractBoard);
        //photonView.RPC("setBoardTo", RpcTarget.All, _abstractBoard);
    }

    /// <summary>
    /// Copy game notation to clipboard, if for instance the user wants to save his current game.
    /// </summary>
    private void _copyGameToClipboard()
    {
        string curgame;
        cgNotation notation = new cgNotation(_abstractBoard);
        curgame = notation.writeFullNotation(cgNotation.NotationType.Algebraic, cgNotation.FormatType.None);
        GUIUtility.systemCopyBuffer = curgame;
    }

    /// <summary>
    /// Set the board to the provided abstract board, write any moves provided in said abstract board to the log, etc.
    /// </summary>
    /// <param name="board"></param>
    [PunRPC]
    public void setBoardTo(cgBoard board)
    {
        _abstractBoard = board;
        _livePieces = new List<cgChessPieceScript>();
        _deadPieces = new List<cgChessPieceScript>();
        _deadWhitePieces = 0;
        _deadBlackPieces = 0;
        movesMade = _abstractBoard.moves.Count;
        if (moveLog != null)
            moveLog.text = "Moves: \n";
        _loggedMoves = 0;
        foreach (cgSimpleMove move in board.moves) _writeLog(move);
        //Debug.Log("C4 before whiteturn = " + whiteTurnToMove);
        whiteTurnToMove = _abstractBoard.WhiteTurnToMove;
        //Debug.Log("C5 before whiteturn = " + whiteTurnToMove);
        placePieces();
        
    }


    /// <summary>
    /// Write move to log.
    /// </summary>
    /// <param name="move"></param>
    private void _writeLog(cgSimpleMove move)
    {
        if (moveLog != null)
        {
            if (NotationType == cgNotation.NotationType.Coordinate)
            {
                if (_loggedMoves % 2 == 0) moveLog.text += "\n";
                else moveLog.text += " | ";
                moveLog.text += _abstractBoard.SquareNames[move.from] + "-" + _abstractBoard.SquareNames[move.to];
            }
            else if (NotationType == cgNotation.NotationType.Algebraic)
            {
                moveLog.text = "Moves:\n";
                cgNotation note = new cgNotation(_abstractBoard.duplicate());
                moveLog.text = note.getLogFriendlyNotation();
            }

            _loggedMoves++;
        }
    }

    public void UnhighlightMoves()
    {
        if (highlightLastMove)
        {
            //revert colors if highlighting is active
            foreach (cgSquareScript square in _squares) square.changeColor(square.startColor);
        }
    }

    public cgChessPieceScript GetMoveFigureIndex(int moveTo)
    {
        cgChessPieceScript piece = _getPieceOn(_abstractBoard.SquareNames[moveTo]);
        return piece;
    }

    public string GetMoveName(cgSimpleMove curCgSimpleMove)
    {
        return _abstractBoard.GetMoveName(curCgSimpleMove);
    }

    public int GetLastMoveFigureIndex()
    {
        return _abstractBoard.GetLastMoveFigureIndex();
    }

    public void SetGameEnded()
    {
        _gameEnded = true;
        chessTaken = true;
        SkipAnimations();
    }

    public void Multiplayer_OpenVictoryScreen()
    {
        photonView.RPC("RPC_OpenVictoryScreen", RpcTarget.OthersBuffered);
    }

    public void Multiplayer_OfferDraw()
    {
        photonView.RPC("RPC_OfferDraw", RpcTarget.OthersBuffered);
    }

    public void Multiplayer_OfferDrawShowScreen()
    {
        photonView.RPC("RPC_OpenOfferDrawScreen", RpcTarget.AllBuffered);
    }

    public void Multiplayer_OfferDrawRefused()
    {
        photonView.RPC("RPC_OpenOfferDrawRefused", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPC_OpenOfferDrawRefused()
    {
        menuController.OpenMultiplayerDrawRefusedScreen();
    }

    [PunRPC]
    private void RPC_OpenOfferDrawScreen()
    {
        
        menuController.MultiplayerOpenDrawScreen();
        networkManager.LeaveRoom();

    }

    [PunRPC]
    private void RPC_OfferDraw()
    {
        menuController.OpenMultiplayerOfferDrawConcede();
    }

    [PunRPC]
    private void RPC_OpenVictoryScreen()
    {
        networkManager.LeaveRoom();
        menuController.OpenWinScreen();
    }
}