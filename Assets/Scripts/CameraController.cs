using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float scaler;
    private Transform _currentTarget;
    [SerializeField] private float upperLimitY = 85;
    [SerializeField] private float buttomLimitY = 25;
    [SerializeField] private float height = 6;
    [SerializeField] private float maxDistance = 45;


    [SerializeField] private float minDistance = 10;
    [SerializeField] private float zoomSpeed = 0.8f;
    private Vector3 _offset;
    private Vector3 _direction;


    [SerializeField] private float turnSpeedY = 100;
    [SerializeField] private float turnSpeedX = 80;
    [SerializeField] private float invert = 1;
    [SerializeField] private bool clipping;
    [SerializeField] private float minClipping;
    [SerializeField] private float maxClipping = 8;

    [SerializeField] private Joystick joystickMovement;
    [SerializeField] private Joystick joystickZoom;

    [SerializeField] private float defaultCameraKeyDistance=30;
    [SerializeField] private float defaultCameraUpperKeyDistance=40;
    private int _lastCameraPos = -1;
    private float _yrot;
    private float _xrot;
    private float _currentDistance;

    private float _lastYrot;
    private float _lastXrot;
    private float _lastCurrentDistance;

    private float _xInput;
    private float _yInput;
    private float _zoomInput;
    [SerializeField] private float cameraTransitionTime = 0.3f;

    private Quaternion _rot;
    private Transform _transform;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        //Application.targetFrameRate = 60;
        _transform = transform;
        _transform.LookAt(_currentTarget);
        ChangeTarget(target);
        SetFirstCameraPosition();
    }

    public void ChangeTarget(Transform newTarget)
    {
        _currentTarget = newTarget;
        _xrot = _transform.eulerAngles.y;
        _yrot = _transform.eulerAngles.x;
        _rot = Quaternion.Euler(_yrot, _xrot, 0);
        _currentDistance = Vector3.Distance(_transform.position, _currentTarget.position);
    }

    private void LateUpdate()
    {
        if (!_transform)
        {
            _transform = transform;
        }

        InputsCollection();
        CameraRotation();
        CameraZoom();
        UpdateCamera();
    }

    private void InputsCollection()
    {
        _xInput = joystickMovement.Horizontal;
        _yInput = joystickMovement.Vertical;
        _zoomInput = joystickZoom.Vertical;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.3f)
        {
            _xInput = Input.GetAxis("Horizontal");
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.3f)
        {
            _yInput = Input.GetAxis("Vertical");
        }

        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.05f)
        {
            _zoomInput = Input.GetAxis("Mouse ScrollWheel") * 8;
        }

        if (Mathf.Abs(_xInput) > 0.1f || Mathf.Abs(_yInput) > 0.1f)
        {
            CheckDistance();
        }
    }

    private void UpdateCamera()
    {
        _direction = new Vector3(0, 0, _currentDistance) * scaler;
        transform.position = _currentTarget.position - _rot * _direction;
        _transform.LookAt(_currentTarget);
    }

    private void CameraZoom()
    {
        float scroll = _zoomInput;
        if (scroll != 0.0f)
        {
            if ((scroll < 0 && _currentDistance < maxDistance * scaler) ||
                (scroll > 0 && _currentDistance > minDistance * scaler))
            {
                _currentDistance -= scroll * zoomSpeed;
            }

            CheckDistance();
        }
    }

    public void CheckDistance()
    {
        if (_currentDistance > maxDistance * scaler)
        {
            _currentDistance = maxDistance * scaler;
        }

        CheckClipping();
    }

    private void CheckClipping()
    {
       
        if (clipping)
        {
            float semiDistance = (maxDistance - minDistance) / 2;
            if (_currentDistance > minDistance + semiDistance)
            {
                _camera.nearClipPlane = Mathf.Lerp(minClipping, maxClipping,
                    (_currentDistance - (minDistance + semiDistance)) / semiDistance);
            }
            else
            {
                _camera.nearClipPlane = minClipping;
            }
        } 
    }

    private void CameraRotation()
    {
        _xrot -= _xInput * turnSpeedX * Time.deltaTime;
        _yrot += invert * _yInput * turnSpeedY * Time.deltaTime;

        _yrot = Mathf.Clamp(_yrot, buttomLimitY, upperLimitY);
        _rot = Quaternion.Euler(_yrot, _xrot, 0);
    }

    public void SetFirstCameraPosition()
    {
        // yrot = 30;
        // xrot = 180;
        // currentDistance = 30;
        StartCoroutine(CameraTransition(_transform.parent.eulerAngles.y + 0, 30, defaultCameraKeyDistance));
        UpdateCamera();
    }

    public void SetFirstCameraPosition(bool is_black_player)
    {
        _transform = transform;

        // yrot = 30;
        // xrot = 180;
        // currentDistance = 30;
        if (is_black_player)
        {
            StartCoroutine(CameraTransition(_transform.parent.eulerAngles.y + 180, 30, defaultCameraKeyDistance));
        }
        else
        {
            StartCoroutine(CameraTransition(_transform.parent.eulerAngles.y + 0, 30, defaultCameraKeyDistance));
        }

        UpdateCamera();
    }

    public void SetSecondCameraPosition()
    {
        // yrot = 30;
        // xrot = 270;
        // currentDistance = 30;
        StartCoroutine(CameraTransition(_transform.parent.eulerAngles.y + 90, 30, defaultCameraKeyDistance));
        UpdateCamera();
    }

    public void SetThirdCameraPosition()
    {
        // yrot = 30;
        // xrot = 0;
        // currentDistance = 30;
        StartCoroutine(CameraTransition(_transform.parent.eulerAngles.y + 180, 30, defaultCameraKeyDistance));
        UpdateCamera();
    }

    public void SetFourthCameraPosition()
    {
        // yrot = 30;
        // xrot = 90;
        // currentDistance = 30;
        StartCoroutine(CameraTransition(_transform.parent.eulerAngles.y + 270, 30, defaultCameraKeyDistance));
        UpdateCamera();
    }

    public void SetNextCamera()
    {
        _lastCameraPos++;
        if (_lastCameraPos > 4)
        {
            _lastCameraPos = 0;
        }

        switch (_lastCameraPos)
        {
            case 0:
                SetFirstCameraPosition();
                break;
            case 1:
                SetSecondCameraPosition();
                break;
            case 2:
                SetThirdCameraPosition();
                break;
            case 3:
                SetFourthCameraPosition();
                break;
            case 4:
                SetUpperCameraPosition();
                break;
        }

        CheckClipping();
    }

    public void SaveCameraPosition()
    {
        _lastYrot = _yrot;
        _lastXrot = _xrot;
        _lastCurrentDistance = _currentDistance / scaler;
    }

    public void LoadLastCameraPosition()
    {
        StartCoroutine(CameraTransition(_lastXrot, _lastYrot, _lastCurrentDistance));
        UpdateCamera();
    }

    public void SetUpperCameraPosition()
    {
        if (GameManager.WhiteSide)
        {
            StartCoroutine(CameraTransition(_transform.parent.eulerAngles.y + 0, 85, defaultCameraUpperKeyDistance));
        }
        else
        {
            StartCoroutine(CameraTransition(_transform.parent.eulerAngles.y + 180, 85, defaultCameraUpperKeyDistance));
        }

        UpdateCamera();
    }

    private IEnumerator CameraTransition(float newXRot, float newYRot, float newDistance)
    {
        newDistance = newDistance * scaler;
        _xrot = _xrot % 360;
        var difXRot = newXRot - _xrot;
        if (difXRot > 180)
        {
            difXRot = -360 + difXRot;
        }
        else if (difXRot < -180)
        {
            difXRot = 360 + difXRot;
        }

        var difYRot = newYRot - _yrot;
        var difDistance = newDistance - _currentDistance;
        var curTransitionTime = cameraTransitionTime;
        while (curTransitionTime > 0)
        {
            _xrot = newXRot - difXRot * curTransitionTime / cameraTransitionTime;
            _yrot = newYRot - difYRot * curTransitionTime / cameraTransitionTime;
            _currentDistance = (newDistance - difDistance * curTransitionTime / cameraTransitionTime);
            curTransitionTime -= Time.deltaTime;
            CheckClipping();

            yield return null;
        }

        _xrot = newXRot;
        _yrot = newYRot;
        _currentDistance = newDistance;

    }
}