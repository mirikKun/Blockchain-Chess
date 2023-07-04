using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class figureMover : MonoBehaviour
{
    public enum FigureType
    {
        Pawn = 1,
        Rook = 2,
        Knight = 3,
        Bishop = 4,
        Queen = 5,
        King = 6,
        Ninja=7,
        Samurai=8
    }

    public Character thisCharacter;
    public cgChessPieceScript.Type role;
    public int figureIndex;
    public PlayerType playerType=PlayerType.Computer;

    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] public FigureType figureType = FigureType.Pawn;
    [SerializeField] private float runSpeed = 2.9f;
    [SerializeField] private float timeToSink = 5;

    [SerializeField] private float minDistance = 0.95f;
    [SerializeField] private float depthDie = 3;
    [SerializeField] private float sinkingSpeed = 0.35f;
    private Transform _transform;
    private Animator _anim;
    private GameObject _flag;
    private static readonly int ForwardWalk = Animator.StringToHash("forward_walk");
    private static readonly int Katana001 = Animator.StringToHash("Katana001");
    private static readonly int SpinSword = Animator.StringToHash("SpinSword");
    private static readonly int ForwardRun = Animator.StringToHash("forward_run");

    private static readonly int ToIdle = Animator.StringToHash("ToIdle");


    private FigureHolder _currentTargetHolder;
    private figureMover _targetFigure;
    private bool _dying;
    private bool _haveEnemy;
    private static readonly int Takeoutkatana = Animator.StringToHash("takeoutkatana");
    private Coroutine _animSequence;
    private FatalityController _fatalityController;
    private Vector3 startRotation;
    private int _randomAnimIndex;
    private FigureFatality _lastFatality;
    private bool _inFatality;

    public FigureFatality[] _fatalitys;
    private List<FigureFatality> _curFigureFatalityAnims = new List<FigureFatality>();

    private void Awake()
    {
        _transform = transform;
        startRotation = _transform.localEulerAngles;
        _anim = GetComponent<Animator>();
        _fatalitys = GetComponents<FigureFatality>();
        Transform flagHolder=transform.Find("Root_M");

        if (flagHolder == null)
        {
            foreach (var animatorHolder in GetComponentsInChildren<Animator>())
            {
                if (animatorHolder.transform.Find("Root_M"))
                {
                    flagHolder=animatorHolder.transform.Find("Root_M");
                }
            }
        }

        if (flagHolder&&flagHolder.Find("Flag"))
        {
            _flag=flagHolder.Find("Flag").gameObject;
        }

        float localScale = _transform.lossyScale.x;
        moveSpeed = moveSpeed * localScale ;
        runSpeed = runSpeed * localScale ;
        minDistance = minDistance * localScale ;
        // foreach (var newRigidbody in GetComponentsInChildren<Rigidbody>())
        // {
        //     newRigidbody.detectCollisions=false;
        // }
        foreach (var newCollider in GetComponentsInChildren<Collider>())
        {
            newCollider.enabled=false;
        }
    }

    [PunRPC]
    public void SetFlag(GameObject flag)
    {
        _flag = flag;
        _flag.SetActive(false);
    }

    [PunRPC]
    public void ShowFlag()
    {
        _flag.SetActive(true);
    }
    [PunRPC]
    public void HideFlag()
    {
        if(_flag)
        {
            _flag.SetActive(false);
        }    
    }
    [PunRPC]
    public void RemoveFlag()
    {
        Transform flagHolder=transform.Find("Root_M");

        if (flagHolder == null)
        {
            foreach (var animatorHolder in GetComponentsInChildren<Animator>())
            {
                if (animatorHolder.transform.Find("Root_M"))
                {
                    flagHolder=animatorHolder.transform.Find("Root_M");
                }
            }
        }

        if (flagHolder&&flagHolder.Find("Flag"))
        {
            Destroy(flagHolder.Find("Flag").gameObject);
        }
    }
    [PunRPC]
    public void StartMove(FigureHolder holder, cgSquareScript enPassantSquare, bool run)
    {
        if (_lastFatality)
            _lastFatality.TurnOffAnimation();
        StopAllCoroutines();
        _animSequence = StartCoroutine(FigureMoving(holder, enPassantSquare, run));
    }
    [PunRPC]
    public void SkipAnimation()
    {
        if (_currentTargetHolder)
        {
            StopCoroutine(_animSequence);
            _transform.position = _currentTargetHolder.transform.position;
            _anim.SetBool(SpinSword, false);
            _anim.SetBool(ForwardWalk, false);
            _anim.SetBool(ForwardRun, false);
            _anim.SetBool(Katana001, false);
            _anim.SetTrigger(ToIdle);
            if (_targetFigure)
            {
                //Destroy(_targetFigure.gameObject);
                _targetFigure.gameObject.SetActive(false);

            }

            ShowCharacter();
            _currentTargetHolder.SetNewFigure(this);
            _currentTargetHolder.EndMoving();
            _currentTargetHolder = null;
            _targetFigure = null;
            _transform.localEulerAngles = startRotation;

            foreach (var fatality in _fatalitys)
            {
                fatality.TurnOffAnimation();
            }
        }

        if (_dying)
        {           

            Destroy(gameObject);
        }
    }
    [PunRPC]
    public void SetupRotation(Vector3 newRotation)
    {
        _transform.localEulerAngles = newRotation;
        startRotation = _transform.localEulerAngles;

    }
    [PunRPC]
    public void SetInFatality()
    {
        _inFatality = true;
        RemoveFlag();
        
    }

    private IEnumerator FigureMoving(FigureHolder holder, cgSquareScript enPassantSquare, bool run)
    {
        _currentTargetHolder = holder;
        _anim.SetBool(Takeoutkatana, false);


        Vector3 newPos;
        if (enPassantSquare != null)
        {
            _targetFigure = enPassantSquare.GetComponent<FigureHolder>().HaveTargetFigure();
            _transform.LookAt(_targetFigure.transform);
            newPos = _targetFigure.transform.position;
        }
        else
        {
            _targetFigure = _currentTargetHolder.HaveTargetFigure();
            _transform.LookAt(_currentTargetHolder.transform);
            newPos = _currentTargetHolder.transform.position;
        }

        _currentTargetHolder.SetNewFigure(this);

        if (run)
        {
            _anim.SetBool(ForwardRun, true);
            yield return FigureMovingToPoint(newPos, minDistance, runSpeed);
        }
        else
        {
            _anim.SetBool(ForwardWalk, true);
            yield return FigureMovingToPoint(newPos, minDistance, moveSpeed);
        }

        if (_targetFigure)
        {
            if (!_fatalityController)
                _fatalityController = FindObjectOfType<FatalityController>();

        
            if (_curFigureFatalityAnims.Count == 0)
            {
                foreach (var fatality in _fatalitys)
                {
                    if (!fatality.FatalityName.Contains(figureType.ToString())||fatality.FatalityName=="KnightDeath"||fatality.FatalityName=="KnightDeathFromKnightFatality") continue;
                    _curFigureFatalityAnims.Add(fatality);
                }
            }

            _randomAnimIndex = Random.Range(0, _curFigureFatalityAnims.Count);
            _anim.SetBool(ForwardWalk, false);
            _anim.SetBool(ForwardRun, false);
            if (_targetFigure.figureType == FigureType.Knight)
            {
                if (this.figureType == FigureType.Knight)
                {
                    
                    _fatalityController.StartPutFigure(this, _targetFigure,
                        "KnightDeathFromKnightFatality");

                }
                else
                {
                    _fatalityController.StartPutFigure(this, _targetFigure,
                        "KnightDeath");

                }
            }
            else
            {
                _fatalityController.StartPutFigure(this, _targetFigure,
                    _curFigureFatalityAnims[_randomAnimIndex].FatalityName);

            }

            foreach (Transform curChild in _transform)
            {
                curChild.gameObject.SetActive(false);
            }
            _anim.SetTrigger(ToIdle);
           // Destroy(_targetFigure.gameObject);
           _targetFigure.gameObject.SetActive(false);
            while (_fatalityController.InFatality)
            {
                yield return null;
            }

            ShowCharacter();
            _anim.SetTrigger(ToIdle);
            run = true;
        }

        if (enPassantSquare)
        {
            _transform.LookAt(_currentTargetHolder.transform);
            newPos = _currentTargetHolder.transform.position;
        }

        if (run)
        {
            _anim.SetBool(ForwardRun, true);
            yield return FigureMovingToPoint(newPos, 0.01f, runSpeed);
        }
        else
        {
            _anim.SetBool(ForwardWalk, true);
            yield return FigureMovingToPoint(newPos, 0.01f, moveSpeed);
        }

        _transform.position = newPos;
        _transform.localEulerAngles = startRotation;


        // if (_anim.GetBool(ForwardWalk))
        // {
        //     yield return StartCoroutine(TakeSwordOut(spinSwordOutTime));
        // }

        _anim.SetBool(ForwardRun, false);
        _anim.SetBool(ForwardWalk, false);
        //yield return new WaitForSeconds(0.2f);
        _currentTargetHolder.EndMoving();
        _currentTargetHolder = null;
        _targetFigure = null;
    }

    private void ShowCharacter()
    {
        if(!GameManager.In2dView)
        {
            foreach (Transform curChild in _transform)
            {
                curChild.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator FigureMovingToPoint(Vector3 newPos, float distance, float speed)
    {
        // if (GameManager.In2dView)
        // {
        //     yield return new WaitForSeconds(0.6f);
        //     yield break;
        // }
        while (Vector3.Distance(newPos, _transform.position) > distance)
        {
            yield return null;
            float step = speed * Time.deltaTime;
            _transform.position = Vector3.MoveTowards(_transform.position, newPos, step);
        }
    }


    public IEnumerator FatalityAnimation(figureMover target, string fatalityName)
    {

        bool opponentHasSameFatality=false;
   
        foreach (var fatality in _fatalitys)
        {
            if (fatality.FatalityName == fatalityName)
            {
                opponentHasSameFatality = true;
                _lastFatality = fatality;
                target.StartFatalityDie(false, fatalityName,_lastFatality);

                yield return (fatality.FatalityAnimation(target));
            }
        }
        if(!opponentHasSameFatality)
        {
            target.StartFatalityDie(false, fatalityName, _lastFatality);
        }
        
    }


    private IEnumerator TakeSwordOut(float time)
    {
        _anim.SetBool(Takeoutkatana, true);
        yield return new WaitForSeconds(time);
        _anim.SetBool(Takeoutkatana, false);
        _anim.SetBool(ForwardWalk, false);
    }

    private void StartFatalityDie(bool skip, string fatalityName,FigureFatality opponent)
    {
        // foreach (var newRigidbody in GetComponentsInChildren<Rigidbody>())
        // {
        //     newRigidbody.detectCollisions=true;
        // }        
         foreach (var newCollider in GetComponentsInChildren<Collider>())
         {
             newCollider.enabled=true;
        }
        if (!skip)
        {
            StartCoroutine(FigureFatalityDying(fatalityName,opponent));
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private IEnumerator FigureFatalityDying(string fatalityName,FigureFatality opponent)
    {
        _dying = true;
        foreach (var fatality in _fatalitys)
        {


            if (fatality.FatalityName == fatalityName)
            {
                yield return fatality.FigureFatalityDying(_inFatality,opponent);
            }
        }

        yield return Sinking();
    }


    private IEnumerator Sinking()
    {
        yield return new WaitForSeconds(timeToSink);
        Vector3 newPos = _transform.position + Vector3.down * depthDie;
        while (Vector3.Distance(newPos, _transform.position) > 0.01f)
        {
            yield return null;
            float step = sinkingSpeed * Time.deltaTime;
            _transform.position = Vector3.MoveTowards(_transform.position, newPos, step);
        }

        Destroy(gameObject);
    }

    private bool _physicDisabled;
    public void DisablePhysic()
    {
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.isTrigger = true;
        }

        foreach (var rigidbody in GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = true;
        }
    }
    [PunRPC]
    public void SwitchToPhysics()
    {
        if(_physicDisabled)
            return;
        Destroy(_anim);
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.isTrigger = false;
        }

        foreach (var rigidbody in GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = false;
        }
    }
}