using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using Photon.Pun;

/// <summary>
/// This script controls the piece on the board, it alters graphics according to promotions/reverts and registers mouse down and mouse up events for dragging purposes
/// </summary>
public class cgChessPieceScript : MonoBehaviour
{
    /// <summary>
    /// Is this piece white?
    /// </summary>
    public bool white
    {
        get { return (type > 0); }
    }

    /// <summary>
    /// The current square being occupied by this instance.
    /// </summary>
    public cgSquareScript square;

    /// <summary>
    /// The flag of current piece.
    /// </summary>
    public FLag2d flag;

    /// <summary>
    /// All possible chess types.
    /// </summary>
    public enum Type
    {
        WhitePawn = 1,
        WhiteRook = 2,
        WhiteKnight = 3,
        WhiteBishop = 4,
        WhiteQueen = 5,
        WhiteKing = 6,

        BlackPawn = -1,
        BlackRook = -2,
        BlackKnight = -3,
        BlackBishop = -4,
        BlackQueen = -5,
        BlackKing = -6
    }
    public figureMover figure;

    public int figureArmyIndex;
    private Vector3 startPos;
    private Vector3 startRot;
    private Vector3 startIconRot;
    private float startHeight;
    private float takenHeight;
    private float curHeight;

    private void Start()
    {
        _size = square.GetComponent<RectTransform>().sizeDelta.x;


        // FlagSetup();
        // startHeight = flag.transform.position.y;
        // takenHeight = startHeight + 2.8f;
        // curHeight = startHeight;
        // startRot = flag.transform.localEulerAngles;
        
        
        //Debug.Log("000");
        //Debug.Log(flag.transform.GetChild(0).localEulerAngles);
    }
    

    private void OnDisable()
    {
        if (flag)
        {
            flag.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void SetFlagUpward()
    {
        if (GameManager.In2dView)
        {
            return;
        }
        curHeight = takenHeight;
        startPos = flag.transform.position;
        flag.transform.SetAsLastSibling();
        flag.transform.position =new Vector3(startPos.x,takenHeight,startPos.z);
        flag.transform.localEulerAngles = Vector3.zero;
         startIconRot = flag.transform.GetChild(0).localEulerAngles;
         flag.transform.GetChild(0).localEulerAngles=Vector3.zero;

     
    }
    [PunRPC]
    public void SetFlagDown()
    {

        if (GameManager.In2dView)
        {
            return;
        }
        curHeight = startHeight;
        startPos= new Vector3(startPos.x, curHeight, startPos.z);

        flag.transform.position=startPos;
        flag.transform.localEulerAngles = startRot;
        flag.SetDefaultIconRotation();

    }

    public void AnalysisFlagPosSetup(int figureIndex)
    {
        if(!flag)
            return;

        flag.gameObject.SetActive(true);
        Vector3 newPosition = transform.position;
        startPos= new Vector3(newPosition.x, curHeight, newPosition.z);
        flag.transform.position = startPos;
        
        square.GetComponent<FigureHolder>().SetupFlagColorAndRotation(figureIndex);
        flag.SetupNewIcon(Mathf.Abs(figureIndex) - 1);

    }

    /// <summary>
    /// The type of this piece.
    /// </summary>
    public Type type = Type.WhitePawn;


    /// <summary>
    /// If displaying as 2d, we will use the sprites insde this Holder.
    /// </summary>
    public GameObject twoDPieceHolder;

    /// <summary>
    /// If displaying as 3d, we will use the models insde this Holder.
    /// </summary>
    /// <summary>
    /// the current pieceholder
    /// </summary>
    private GameObject _pieceholder;

    /// <summary>
    /// Is this instance dead?
    /// </summary>
    public bool dead = false;

    /// <summary>
    /// Size of square.
    /// </summary>
    private float _size;

    private Action<cgChessPieceScript> _onDown;
    private Action<cgChessPieceScript> _onUp;

    private void Awake()
    {
    }

    /// <summary>
    /// Set mouse callbacks to allow this instance to be dragged and dropped.
    /// </summary>
    /// <param name="onDown">Callback for mouse down</param>
    /// <param name="onUp">Callback for mouse up</param>
    public void SetCallbacks(Action<cgChessPieceScript> onDown, Action<cgChessPieceScript> onUp)
    {
        _onDown = onDown;
        _onUp = onUp;
    }


    void OnMouseDown()
    {
        if (_onDown != null && !dead && !  GameManager.Pause)
        
        {
            _onDown(this);
        }
    }

    void OnMouseUp()
    {
        if (_onUp != null && !dead&&!  GameManager.Pause)
        {
            _onUp(this);
        }
    }


    /// <summary>
    /// Set the type of this piece, changes its sprite accordingly.
    /// Useful when reverting moves, or when pawns are promoted.
    /// </summary>
    /// <param name="toType">The type to change to.</param>
    public void SetType(Type toType)
    {
        string typeName = toType.ToString();

        //Display as 2d.
        twoDPieceHolder.SetActive(true);

        foreach (Transform child in twoDPieceHolder.transform)
        {
            if (child.gameObject.name == typeName) child.gameObject.SetActive(true);
            else child.gameObject.SetActive(false);
        }


        type = toType;
    }

    /// <summary>
    /// Set the type of this piece, changes its sprite accordingly.
    /// Useful when reverting moves, or when pawns are promoted.
    /// </summary>
    /// <param name="toType">The type to change to.</param>
    public void SetType(int toType)
    {
        Type type = (Type) toType;
        SetType(type);
    }

    /// <summary>
    /// Start at provided square.
    /// </summary>
    /// <param name="startSquare">the starting square.</param>
    [PunRPC]
    public void StartAtSquare(cgSquareScript startSquare)
    {
        square = startSquare;
        dead = false;
        if (startSquare != null)
        {
            //piece.SetStartNode(startnode.node);
            transform.position = new Vector3(startSquare.transform.position.x, startSquare.transform.position.y,
                startSquare.transform.position.z);
      
        }
    }

    [PunRPC]
    public void FlagSetup()
    {
        FigureHolder figureHolder = square.GetComponent<FigureHolder>();

        figureHolder.CreateFlag((int) type,this);

        startHeight = flag.transform.position.y;
        takenHeight = startHeight + 2.8f;
        curHeight = startHeight;
   
        startRot = flag.transform.localEulerAngles;
        // if (!flagTransform)
        // {        
        //     FigureHolder figureHolder = square.GetComponent<FigureHolder>();
        //
        //     figureHolder.CreateFlag((int) type,this);
        // }
        // else
        // {
        //     startPos= new Vector3(transform.position.x, curHeight, transform.position.z);
        //             
        //     flagTransform.position = startPos;
        // }
    }


    /// <summary>
    /// Move to a new square.
    /// </summary>
    /// <param name="newSquare">the new square to move to.</param>
    [PunRPC]
    public void moveToSquare(cgSquareScript newSquare, cgSquareScript enPassantSquare = null)
    {
        bool run=false;
        float scaleMultiplayer=1;
        FigureHolder figureHolder = square.GetComponent<FigureHolder>();
        if (square != newSquare && figureHolder.HaveTargetFigure())
        {   
            var newFigureMover = newSquare.GetComponent<FigureHolder>();

            scaleMultiplayer = figureHolder.figure.transform.lossyScale.x;
         
            if (Vector3.Distance(newSquare.transform.localPosition, square.transform.localPosition) < 1.9f * _size&&type!=Type.BlackRook&&type!=Type.WhiteRook)
            {
                run = false;
            }
            else
            {
                run = true;
            }

            figureHolder.FigureMove(newFigureMover, enPassantSquare, run);
         
        }
        square = newSquare;
        Vector3 newPosition = newSquare.transform.position;
        startPos= new Vector3(newPosition.x, curHeight, newPosition.z);
        if (GameManager.In2dView)
        {
            StartCoroutine(MoveFlagIn2D(run,scaleMultiplayer));
        }
        else
        {
            flag.transform.position = startPos;
        }

        transform.position = new Vector3(newPosition.x, newPosition.y, newPosition.z);
    }

    public IEnumerator MoveFlagIn2D(bool run,float scaleMultiplayer)
    {
        yield return flag.FlagMoving(startPos,run,scaleMultiplayer);
        flag.transform.position = startPos;

    }
}