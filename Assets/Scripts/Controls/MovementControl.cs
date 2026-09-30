using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MovementControl : MonoBehaviour
{
    public PlayerControls Player;

    [Header("UI")]
    [SerializeField] private RectTransform _wheel;
    [SerializeField] private RectTransform _wheelParent;
    [SerializeField] private RectTransform _speedButton;
    [SerializeField] private RectTransform _compass;
    [SerializeField] private RectTransform _anchorChain;

    [Header("Forward")]
    [SerializeField] private float _forwardForce;
    [SerializeField] private bool _isGoingForward;
    [SerializeField] private float breakSpeed;
    [SerializeField] private Touch _touch;
    private bool isOnWheel;
    private bool justRotate;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed;
    private float rotationSpeedAndDirection;
    [SerializeField] private Transform _Rotater;
    [SerializeField] private Transform _TargetRotation;
    [SerializeField] private Transform _RotationDirection;
    [SerializeField] private Transform _ThresholdRotation;
    [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float minRotationSpeed;
    private Vector3 angularVelocityEffect = Vector3.zero;

    private float widthWheel;
    private float heightWheel;
    private float widthSpeed;
    private float heightSpeed;
    private Vector3 originalPosSpeed;
    private Vector3 _mousePos;


    [SerializeField] private float UIAnchorMaxHeight;
    [SerializeField] private float UIAnchorMinHeight;
    [SerializeField] private float UIAnchorMaxPullingSpeed;
    [SerializeField] private float UIAnchorMinPullingSpeed;
    private float UIAnchorPullingSpeed;

    [Header("effects")]
    //[SerializeField] private Transform _modelTransform;
    [SerializeField] private float _rotationEffectMagnitude;
    [SerializeField] private float _rotationEffectMagnitude_x;

    private float CurrentTargetHeight;
    public int wheelFinger;

    // Start is called before the first frame update
    void Start()
    {
        //_wheel = GeneralFunctions.Wheel;
        //_wheelParent = GeneralFunctions.WheelParent;
        //_speedButton = GeneralFunctions.SpeedButton;
        //_compass = GeneralFunctions.Compass;
        
        widthWheel = _wheel.rect.width;
        heightWheel = _wheel.rect.height;
        widthSpeed = _speedButton.rect.width;
        heightSpeed = _speedButton.rect.height;
        originalPosSpeed = _speedButton.transform.position;

        wheelFinger = -1;
        _wheelParent.eulerAngles = new Vector3(0, 0, 90);
        CurrentTargetHeight = UIAnchorMinHeight;
        UIAnchorPullingSpeed = UIAnchorMinPullingSpeed;
    }

    public void NewPlayer()
    {
        Player = GeneralFunctions.Player.GetComponent<PlayerControls>();
        //Player._rb = Player.gameObject.GetComponent<Rigidbody>();
        _RotationDirection = Player.gameObject.transform.GetChild(6); // ---- temp
        _ThresholdRotation = Player.gameObject.transform.GetChild(7); // ---- temp
    }
    
    // Update is called once per frame
    void Update()
    {
        
        if(Player != null)
        {
            //EFFECT - Pull and Drop UI Anchor
            if (_anchorChain != null && CurrentTargetHeight != _anchorChain.rect.height)
            {
                _anchorChain.sizeDelta = new Vector2(_anchorChain.sizeDelta.x, Mathf.MoveTowards(_anchorChain.rect.height, CurrentTargetHeight, UIAnchorPullingSpeed));
            }

            //EFFECT - rotate ship to direction of rotation
            Player._modelTransform.localRotation = Quaternion.Euler(angularVelocityEffect.magnitude * _rotationEffectMagnitude_x, Player._modelTransform.rotation.y, angularVelocityEffect.y * _rotationEffectMagnitude);
            //Debug.Log("start move");
            if (Input.GetKey(KeyCode.J))
            {
                Debug.Log("j");
                _isGoingForward = true;
            }
            //if (Input.GetKeyUp(KeyCode.J))
            //{
            //    _isGoingForward = false;
            //}
            //Debug.Log("update move");
            if (wheelFinger >= 0)
            { // any touch
              //_touch = Input.GetTouch(wheelFinger);
                //Debug.Log("touch");
                if (Input.touchCount - 1 < wheelFinger)
                    _touch = Input.GetTouch(0);
                else
                    _touch = Input.GetTouch(wheelFinger);

                if (!justRotate && (_touch.phase == TouchPhase.Moved || _touch.phase == TouchPhase.Stationary))
                { // touch on speed and move it around
                    if (Vector3.Distance(_touch.position, _wheel.transform.position) < ((widthWheel / 2)))
                    {
                        _speedButton.position = _touch.position;
                    }
                    else
                    { // finger outside the wheel
                        Vector3 temp = CalcNewSpeedPosition();
                        if (temp.x == float.NaN)
                            Debug.Log("?");
                        _speedButton.position = temp;
                    }
                }

                if (!isOnWheel && (justRotate || !GeneralFunctions.CheckMouseOnObject(widthSpeed, heightSpeed, originalPosSpeed, _touch.position)))
                {
                    isOnWheel = true;
                    GetDirection();
                    //GetDirectionOffset();
                }
                if (isOnWheel /*|| (justRotate && Input.GetTouch(wheelFinger).deltaTime > 0f)*/)
                {
                    GetDirection();
                }
            }

            if (isOnWheel && _touch.phase == TouchPhase.Began)
            {
                _Rotater.transform.position = Player.gameObject.transform.position;
                _Rotater.transform.rotation = Quaternion.Euler(0, (GeneralFunctions.WheelParent.rotation.eulerAngles.z - 90) * -1, 0);
                _RotationDirection.transform.position = _TargetRotation.position;

                if (_RotationDirection.localPosition.x >= 0)
                {
                    rotationSpeedAndDirection = rotationSpeed;
                    _ThresholdRotation.localEulerAngles = new Vector3(0, 250, 0);
                }
                else
                {
                    rotationSpeedAndDirection = -1 * rotationSpeed;
                    _ThresholdRotation.localEulerAngles = new Vector3(0, 110, 0);
                }
            }

            _Rotater.transform.position = Player.gameObject.transform.position;
            _Rotater.transform.rotation = Quaternion.Euler(0, (GeneralFunctions.WheelParent.rotation.eulerAngles.z - 90) * -1, 0);
            _RotationDirection.transform.position = _TargetRotation.position;

            Vector2 targetR = new Vector2(_Rotater.forward.x, _Rotater.forward.z);  //_TargetRotation.position;
            Vector2 playerR = new Vector2(Player.gameObject.transform.forward.x, Player.gameObject.transform.forward.z);

            float a = Vector2.Angle(targetR, playerR);

            if (Mathf.Abs(a) > 5f)
            {
                if (_RotationDirection.localPosition.z > 0)
                {
                    if (_RotationDirection.localPosition.x >= 0)
                    {
                        rotationSpeedAndDirection = rotationSpeed;
                    }
                    else
                    {
                        rotationSpeedAndDirection = -1 * rotationSpeed;
                    }
                }
                else
                {
                    float b = Vector2.Angle(targetR, new Vector2(_ThresholdRotation.forward.x, _ThresholdRotation.forward.z));
                    if (b < 5)
                    {
                        _RotationDirection.transform.position = _TargetRotation.position;
                        _ThresholdRotation.localEulerAngles = new Vector3(0, 360 - _ThresholdRotation.localEulerAngles.y, 0);

                    }
                }


                RotateShip(rotationSpeedAndDirection);
            }
            else
            {
                _RotationDirection.transform.position = _TargetRotation.position;
                angularVelocityEffect = Vector3.Lerp(angularVelocityEffect, Vector3.zero, Time.deltaTime);
            }
        }
        
    }

    private void FixedUpdate()
    {
        if(Player != null)
        {
            if (_isGoingForward/* && !isPlayerHold*/)
            {
                //Debug.Log("GoForward");
                GoForward();
            }
            else if (Player._rb.velocity.magnitude > 0)
            {
                Player._rb.velocity = Vector3.MoveTowards(Player._rb.velocity, Vector3.zero, Time.deltaTime * breakSpeed);
            }
        }
        
    }

    public void HoldSpeed()
    {
        _isGoingForward = true;
        foreach (Touch touch in Input.touches)
        {
            if (GeneralFunctions.CheckMouseOnObject(_speedButton, touch.position))
            {
                wheelFinger = touch.fingerId;
            }
        }

        CurrentTargetHeight = UIAnchorMaxHeight;
        UIAnchorPullingSpeed = UIAnchorMaxPullingSpeed;
    }
    public void HoldJustWheel()
    {
        justRotate = true;
        foreach (Touch touch in Input.touches)
        {
            if (GeneralFunctions.CheckMouseNearObject(_compass.rect.width, _compass.rect.height, _compass.transform.position, touch.position, 130))
            {
                wheelFinger = touch.fingerId;
            }
        }
    }
    public void EndWheelTouch()
    {
        wheelFinger = -1;
        isOnWheel = false;
        justRotate = false;
        _speedButton.position = originalPosSpeed;
        _isGoingForward = false;

        //Anchor effect
        CurrentTargetHeight = UIAnchorMinHeight;
        UIAnchorPullingSpeed = UIAnchorMinPullingSpeed;
    }

    public void CheckSpherecast(GameObject container, float radius, float detectableDistance, LayerMask detectableLayer)
    {
        Ray ray = new Ray(container.transform.position, container.transform.forward);
        RaycastHit hitInfo;
        if (Physics.SphereCast(ray, radius, out hitInfo, detectableDistance, detectableLayer))
        {
            Debug.Log(hitInfo);
        }
    }
    private Vector3 CalcNewSpeedPosition()
    {
        Vector2[] array = GeneralFunctions.CalcIntersectionOfCircleAndLine(_wheel.transform.position, _touch.position, widthWheel * 2, _wheel.transform.position);
        float dis0 = Vector2.Distance(array[0], _speedButton.transform.position);
        float dis1 = Vector2.Distance(array[1], _speedButton.transform.position);

        if (dis0 < dis1)
        {
            if (array[0].x == float.NaN || array[0].y == float.NaN)
                Debug.Log("?");
            if (array[0].x == float.NaN || array[0].y == float.NaN)
                Debug.Log("?");
            return array[0];
        }
        if (array[1].x == float.NaN || array[1].y == float.NaN)
            Debug.Log("bug speed");
        return array[1];
    }

    //can hold the wheel from every point of the wheel
    private void GetDirectionOffset()
    {
        _wheel.SetParent(_wheelParent.parent);
        GetDirection();
        _wheel.SetParent(_wheelParent);
    }

    private void GetDirection()
    {
        if (wheelFinger > -1)
        {
            //_mousePos = new Vector3(Input.GetTouch(wheelFinger).position.x, Input.GetTouch(wheelFinger).position.y, 0);
            Touch touch;
            if (Input.touchCount - 1 < wheelFinger)
                touch = Input.GetTouch(0);
            else
                touch = Input.GetTouch(wheelFinger);
            _mousePos = new Vector3(touch.position.x, touch.position.y, 0);
            Vector3 diffrence = _mousePos - _wheel.position;
            float rotationZ = Mathf.Atan2(diffrence.y, diffrence.x) * Mathf.Rad2Deg;
            _wheelParent.rotation = Quaternion.Euler(0, 0, rotationZ);
        }
    }

    private void RotateShip(float force)
    {
        Quaternion a = Quaternion.Euler(transform.rotation.x, (_wheelParent.rotation.eulerAngles.z - 90) * -1, transform.rotation.z);
        float diff = Mathf.Clamp(Mathf.Abs(transform.rotation.eulerAngles.y - a.eulerAngles.y), 0, 180);
        float rotSpeedCalc = Mathf.Clamp(maxRotationSpeed * (diff / 180), minRotationSpeed, maxRotationSpeed);
        //Debug.Log(rotSpeedCalc);
        Player.gameObject.transform.rotation = Quaternion.RotateTowards(Player.gameObject.transform.rotation, a, rotSpeedCalc);
        angularVelocityEffect = new Vector3(0, Mathf.Lerp(angularVelocityEffect.y, force, Time.deltaTime), 0);
    }

    public void GoForward()
    {
        Vector3 pos = Player._rb.position;
        Quaternion rotation = Player._rb.rotation;
        Vector3 force = Player.gameObject.transform.forward * _forwardForce;
        Player._rb.AddForce(force);
    }

    public void OpenOrCloseInventory()
    { // when player hit inventory buttom the inventory open or close
        GeneralFunctions.inventoryControl.OpenOrCloseInventoryUI();
    }
}
