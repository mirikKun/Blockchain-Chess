using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FatalityController : MonoBehaviour
{
    [SerializeField] private Transform firstPlace;
    [SerializeField] private Transform secondPlace;


    [SerializeField] private Camera firstCamera;
    [SerializeField] private Camera secondCamera;
    [SerializeField] private LayerMask secondCameraLayerMask;

    [SerializeField] private Material lastSkybox;
    [SerializeField] private Material fatalitySkybox;
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject[] objectsToShow;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private PlayableDirector clip;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SecondScenePositionController scenePositionController;
    [SerializeField] private cgChessBoardScript board;
    [SerializeField] private bool hideOthers;
    public int knightPositionIndex = -1;
    private ViewSwitcher viewSwitcher;
    private List<Transform> figureHolders = new List<Transform>();
    private CinemachineBrain _cameraCinemachineBrain;
    private figureMover _curFigure;
    private IEnumerator _coroutine;
    public Action ONSkip;
    public bool InFatality;
    public bool physicDisabled;

    [System.Serializable]
    public struct FatalityParameters
    {
        public string FatalityName;
        public PlayableAsset cameraTimeline;
        public AudioClip fatalitySound;
        public Vector3 FirstPosition;
        public Vector3 FirstRotation;

        public Vector3 SecondPosition;
        public Vector3 SecondRotation;
        public float SoundDelay;
        public bool disableCinemachine;
    }


    [SerializeField] private FatalityParameters[] fatalityParameters;

    private void Start()
    {
        ClearPlaces();
        figureHolders.Add(firstPlace);
        figureHolders.Add(secondPlace);
        TransitionObjectsHolder transitionObjects = FindObjectOfType<TransitionObjectsHolder>();
        viewSwitcher = FindObjectOfType<ViewSwitcher>();
        if (transitionObjects)
        {
            firstCamera = transitionObjects.firstCamera;
            objectsToHide = objectsToHide.Concat(transitionObjects.objectsToHide).ToArray();
            board = transitionObjects.GetComponent<cgChessBoardScript>();
        }

        audioSource = FindObjectOfType<AudioSource>();
        _cameraCinemachineBrain = secondCamera.GetComponent<CinemachineBrain>();
    }

    public void StartPutFigure(figureMover firstFigure, figureMover secondFigure, string fatalityName)
    {
        _coroutine = PutFigures(firstFigure, secondFigure, fatalityName);
        InFatality = true;
        StartCoroutine(_coroutine);
    }

    public IEnumerator PutFigures(figureMover firstFigure, figureMover secondFigure, string fatalityName)
    {
        foreach (var objectToHide in objectsToHide)
        {
            objectToHide.SetActive(false);
        }

        foreach (var objectToShow in objectsToShow)
        {
            objectToShow.SetActive(true);
        }

        if (viewSwitcher && hideOthers && !viewSwitcher.Get2DView())
        {
            viewSwitcher.SetFiguresActive(false);
        }

        lastSkybox = RenderSettings.skybox;
        RenderSettings.skybox = fatalitySkybox;
        if (board && board.playerCanMove)
        {
            skipButton.SetActive(true);
        }

        FatalityParameters fatalityParameter = new FatalityParameters();
        foreach (var newFatalityParameter in fatalityParameters)
        {
            if (newFatalityParameter.FatalityName == fatalityName)
            {
                fatalityParameter = newFatalityParameter;
            }
        }

        firstCamera.enabled = false;
        secondCamera.gameObject.SetActive(true);
        clip.playableAsset = fatalityParameter.cameraTimeline;
        clip.Play();

        firstPlace.localPosition = fatalityParameter.FirstPosition;
        firstPlace.localEulerAngles = fatalityParameter.FirstRotation;

        figureMover first = Instantiate(firstFigure, firstPlace.position, Quaternion.identity, firstPlace);
        first.gameObject.SetActive(true);
        first.transform.localEulerAngles = Vector3.zero;
        first.SetInFatality();
        //first.gameObject.layer = secondCameraLayerMask;

        foreach (Transform trans in first.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = secondCameraLayerMask;
        }

        foreach (Transform trans in first.transform)
        {
            trans.gameObject.SetActive(true);
        }

        foreach (SkinnedMeshRenderer mesh in first.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mesh.updateWhenOffscreen = true;
        }

        secondPlace.localPosition = fatalityParameter.SecondPosition;
        secondPlace.localEulerAngles = fatalityParameter.SecondRotation;
        figureMover second = Instantiate(secondFigure, secondPlace.position, Quaternion.identity, secondPlace);
        second.gameObject.SetActive(true);
        second.transform.localEulerAngles = Vector3.zero;
        second.SetInFatality();
        //second.gameObject.layer = secondCameraLayerMask;
        foreach (Transform trans in second.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = secondCameraLayerMask;
        }

        foreach (Transform trans in second.transform)
        {
            trans.gameObject.SetActive(true);
        }

        foreach (SkinnedMeshRenderer mesh in second.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mesh.updateWhenOffscreen = true;
        }

        //yield return new WaitForSeconds(1);
        if (fatalityParameter.fatalitySound && audioSource)
        {
            audioSource.clip = fatalityParameter.fatalitySound;
            StartCoroutine(SoundDelay(fatalityParameter.SoundDelay));
        }

        if (scenePositionController)
        {
            //scenePositionController.GenerateFatalityData();
            if (fatalityName == "Knight1" || fatalityName == "KnightDeathFromKnightFatality")
            {
                if (knightPositionIndex >= 0)
                {
                    scenePositionController.ChangePosition(knightPositionIndex);
                }
                else
                {
                    scenePositionController.SetDefaultRotation();
                }
            }

            scenePositionController.ChangePosition();
        }

        if (fatalityParameter.disableCinemachine)
        {
            secondCamera.fieldOfView = 40;
        }

        if (_cameraCinemachineBrain)
        {
            _cameraCinemachineBrain.enabled = !fatalityParameter.disableCinemachine;
        }

        yield return StartCoroutine(first.FatalityAnimation(second, fatalityName));
        yield return new WaitForSeconds(0.5f);
        BackToGame();
    }

    private void ClearPlaces()
    {
        foreach (Transform child in secondPlace)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in firstPlace)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator SoundDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }

    private void BackToGame()
    {
        clip.Stop();
        InFatality = false;

        secondCamera.gameObject.SetActive(false);
        firstCamera.enabled = true;

        if (viewSwitcher && hideOthers && !viewSwitcher.Get2DView())
        {
            viewSwitcher.SetFiguresActive(true);
        }

        foreach (var objectToShow in objectsToShow)
        {
            objectToShow.SetActive(false);
        }

        foreach (var objectToHide in objectsToHide)
        {
            objectToHide.SetActive(true);
        }

        if (lastSkybox)
            RenderSettings.skybox = lastSkybox;

        //if (skipButton)
        skipButton.SetActive(false);
        ClearPlaces();
    }

    public void Skip()
    {
        SkipAnimations();
        ONSkip?.Invoke();
    }

    public void SkipAnimations()
    {
        BackToGame();
        StopAllCoroutines();
        if (audioSource)
            audioSource.Stop();
        if (board)
            board.SkipAnimations();
    }
}