using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Utilities;
using UnityEngine;

/// <summary>
/// Это скрипт ракетной установки. Он ищет упреждение и управляет установкой, а так же считывает указание на пуск ракет.
/// </summary>

public class RocketLauncher : MonoBehaviour
{
    
    //Сингтон
    static RocketLauncher MainS;
    
    #region Инспектор

    [SerializeField] private AnimationCurve _soundBlink;
    
    
    [SerializeField] private GameObject _rocketPrefab;
    
    [SerializeField] private AudioSource _beepLong;
    [SerializeField] private AudioSource _beepShort;
    
    [SerializeField] private Transform _targetParent;
    [SerializeField] private AirCraft _targetAirCraft;
    
    [SerializeField] private Transform _leadPoint;
    [Header("Параметры установки")]
    [SerializeField] private float _rotateSpeed;
    [Header("Параметры ракеты")]
    [SerializeField] private float _rocketRotateSpeed;
    [SerializeField] private float _rocketSpeed;
    /// <summary>
    /// Минимальная дистанция для уничтожения цели
    /// </summary>
    [SerializeField] private float _rocketMinDist;
    public enum RocketFollowMode {Off, LocalLeadPoint};
    /// <summary>
    /// Тип слижения за целью у ракеты
    /// </summary>
    [SerializeField] private RocketFollowMode _rocketTargetFlowMode;
    /// <summary>
    /// Отслеживать скорость через перемещение между кадрами (Более натуральная погрешность)
    /// </summary>
    [Tooltip("Отслеживать скорость через перемещение между кадрами (Более натуральная погрешность)")]
    [SerializeField] private bool _transformCheckSpeed;
    
    #endregion

    #region Локальные переменные

    private Transform _target;
    private bool _rocketMove;
    private Vector3 _targetSpeed = Vector3.zero;

    #endregion

    private void Start()
    {
        MainS = this;
    }

    public void SetAirCraft(Transform _acTrnsf, AirCraft _acPar)
    {
        _target = _acTrnsf;
        _targetAirCraft = _acPar;

        InitTargetFollow();
    }

    public static Vector3 GetTargetVelocity()
    {
        return MainS._targetSpeed;
    }

    private void InitTargetFollow()
    {
        //_target = _targetParent.GetChild(0);
        _leadPoint.gameObject.SetActive(true);

        _beepShort.pitch = 1;
        _beepShort.PlayOneShot(_beepShort.clip);
        
        StartCoroutine(TargetFollow());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null && (Input.GetKeyDown(KeyCode.Mouse0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            _rocketMove = true;
            LaunchRocket();
        }
    }

    public void LaunchRocket()
    {
        GameObject newRocket = Instantiate(_rocketPrefab, _targetParent);
        newRocket.transform.position = transform.position;
        newRocket.transform.rotation = transform.rotation;
        newRocket.GetComponent<Rocket>().SetParams(_rocketSpeed,_rocketMinDist,_rocketRotateSpeed,_rocketTargetFlowMode,_target);
    }

    /// <summary>
    /// Наводит пусковую установку на упреждение
    /// </summary>
    /// <returns></returns>
    IEnumerator TargetFollow()
    {
        _beepLong.volume = 1;
        Vector3 oldPos = MainS._target.position;
        
        while (_target != null)
        {
//            if (_transformCheckSpeed)
//            {
//                
//            }
            if (_transformCheckSpeed)
            {
                _targetSpeed = (MainS._target.position - oldPos)/Time.deltaTime;
                oldPos = MainS._target.position;
            }
            else
            {
                _targetSpeed = _target.up * _targetAirCraft.Speed;
            }

            _leadPoint.localPosition = FirstOrderIntercept(transform.position,Vector3.zero,_rocketSpeed,_target.position,_targetSpeed);
            
            float rotateZ = transform.rotation.eulerAngles.z;
            transform.LookAt(_leadPoint.position);
            if (_leadPoint.position.x < transform.position.x)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, MathUtilities.AngleToAngle(
                    rotateZ,
                    90 + transform.rotation.eulerAngles.x,
                    _rotateSpeed * Time.deltaTime)));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, MathUtilities.AngleToAngle(
                    rotateZ,
                    -transform.rotation.eulerAngles.x - 90,
                    _rotateSpeed * Time.deltaTime)));

            }
            
            if (!_rocketMove)
            {
                _beepLong.pitch = 0.6f;
                if (Mathf.Abs( rotateZ - transform.rotation.eulerAngles.z) < 0.85f*_rotateSpeed * Time.deltaTime)
                {
                    _beepLong.volume = 0.3f;
                }
                else
                {
                    _beepLong.volume = _soundBlink.Evaluate(MathUtilities.GetNormalizeTime())*0.3f;
                }
            }
            else
            {
                _beepLong.volume = _soundBlink.Evaluate(MathUtilities.GetNormalizeTime(1.5f))*0.3f;
                _beepLong.pitch = 0.8f;
            }

            
                
            
                
            yield return new WaitForEndOfFrame();
        }
        
        _beepShort.pitch = 1.2f;
        _beepShort.PlayOneShot(_beepShort.clip);

        _beepLong.volume = 0;
        _rocketMove = false;
        
        _leadPoint.gameObject.SetActive(false);
        
    }
    
    //first-order intercept using absolute target position
    public static Vector3 FirstOrderIntercept(Vector3 shooterPosition,Vector3 shooterVelocity,float shotSpeed,Vector3 targetPosition,Vector3 targetVelocity)
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        
        return targetPosition + t*(targetRelativeVelocity);
    }
    
//first-order intercept using relative target position
    public static float FirstOrderInterceptTime
    (float shotSpeed,Vector3 targetRelativePosition,Vector3 targetRelativeVelocity) {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if(velocitySquared < 0.001f)
            return 0f;
 
        float a = velocitySquared - shotSpeed*shotSpeed;
 
        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude/
                      (
                          2f*Vector3.Dot
                          (
                              targetRelativeVelocity,
                              targetRelativePosition
                          )
                      );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }
 
        float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b*b - 4f*a*c;
 
        if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
            float	t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
                t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
            if (t1 > 0f) {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            } else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        } else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
    }
    
}
