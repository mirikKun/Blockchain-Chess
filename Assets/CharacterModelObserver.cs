using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterModelObserver : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler,
    IPointerClickHandler
{
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject[] objectsToShow;
    [SerializeField] private Transform spawnPlace;
    [SerializeField] private Transform characterCamera;

    [SerializeField] private float maxZoom = 18;
    [SerializeField] private float minZoom = 11;
    [SerializeField] private float zoomSensitivity = 3f;
    [SerializeField] private float phoneZoomSensitivity = 3f;
    [SerializeField] private float maxHeight = 1.75f;
    [SerializeField] private float minHeight = 0.5f;
    [SerializeField] private float moveSensitivity = 0.001f;
    [SerializeField] private float rotationSensitivity = 0.5f;

    private Vector3 _startCharacterRotation = new Vector3(0, 0, 0);
    private Vector3 _curCharacterRotation;

    private Vector3 _previousMousePosition;
    private Vector3 _curMouseOffset;
    [SerializeField] private Transform model;
    private bool _objectWasClicked;

    private bool _mouseOverObject;
    private bool _rotating;
    private bool _moving;

    // private void Start()
    // {
    //     float offset = canvas.sizeDelta.y / canvas.sizeDelta.x;
    //     Debug.Log(offset);
    //     characterCamera.position = new Vector3((0.5f - offset),characterCamera.position.y,characterCamera.position.z);
    //     Debug.Log(characterCamera.position);
    //
    // }

    private void Update()
    {
        // Debug.Log("_");

        if (_objectWasClicked)
        {
#if UNITY_ANDROID
            if (_mouseOverObject &&Input.touchCount == 2)
            {
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                Vector2 firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
                Vector2 secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;
                
                float prevMagnitude = (firstTouchPrevPos-secondTouchPrevPos).magnitude;
                float currentMagnitude = (firstTouch.position-secondTouch.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;
                Vector3 cameraDist = characterCamera.position;

                cameraDist += difference * phoneZoomSensitivity * Vector3.forward;
                cameraDist = new Vector3(cameraDist.x, cameraDist.y, Mathf.Clamp(cameraDist.z, -maxZoom, -minZoom));
                characterCamera.position = cameraDist;
            }
            else
#endif
            {
                _curMouseOffset = Input.mousePosition - _previousMousePosition;
                if (Mathf.Abs(_curMouseOffset.y) > 0.1f || _moving || _rotating)
                {
                    if (!_moving && !_rotating)
                    {
                        if (Mathf.Abs(_curMouseOffset.x) > Mathf.Abs(_curMouseOffset.y))
                        {
                            _rotating = true;
                        }
                        else
                        {
                            _moving = true;
                        }

                        _previousMousePosition = Input.mousePosition;
                        _curMouseOffset = Vector3.zero;
                    }


                    if (_rotating)
                    {
                        model.Rotate(model.up, Vector3.Dot(_curMouseOffset, spawnPlace.right) * rotationSensitivity);
                        _previousMousePosition = Input.mousePosition;
                    }
                    else if (_moving)
                    {
                        Vector3 cameraDist = characterCamera.localPosition;
                        cameraDist -= _curMouseOffset.y * moveSensitivity * Vector3.up;
                        cameraDist = new Vector3(cameraDist.x, Mathf.Clamp(cameraDist.y, minHeight, maxHeight),
                            cameraDist.z);
                        characterCamera.localPosition = cameraDist;
                        _previousMousePosition = Input.mousePosition;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _objectWasClicked = false;
            _moving = false;
            _rotating = false;
        }
        // Debug.Log(1)
# if !UNITY_ANDROID
        if (_mouseOverObject && Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f)
        {
            Vector3 cameraDist = characterCamera.position;
            cameraDist += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity * Vector3.forward;
            cameraDist = new Vector3(cameraDist.x, cameraDist.y, Mathf.Clamp(cameraDist.z, -maxZoom, -minZoom));
            characterCamera.position = cameraDist;
        }
#endif
    }

    public void SetupCharacterModel(figureMover characterModel)
    {
        DeleteLastCharacter();
        model = Instantiate(characterModel.transform, spawnPlace.position, quaternion.Euler(_startCharacterRotation),
            spawnPlace);
        model.localEulerAngles = _startCharacterRotation;
        foreach (SkinnedMeshRenderer mesh in model.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mesh.updateWhenOffscreen = true;
        }

        foreach (var panel in objectsToHide)
        {
            panel.SetActive(false);
        }

        foreach (var panel in objectsToShow)
        {
            panel.SetActive(true);
        }
    }

    public void DeleteLastCharacter()
    {
        if (model)
        {
            Destroy(model.gameObject);
        }
    }

    public void ExitCharacterScreen()
    {
        foreach (var panel in objectsToHide)
        {
            panel.SetActive(true);
        }

        foreach (var panel in objectsToShow)
        {
            panel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        DeleteLastCharacter();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseOverObject = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(3333);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mouseOverObject = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _previousMousePosition = Input.mousePosition;
        _objectWasClicked = true;
    }
}