using System.Collections;
using System.Collections.Generic;
using Systems.Utilities;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    #region Параметры ракеты

    private float _speed;
    private float _minDistSqrt;
    private RocketLauncher.RocketFollowMode _followMode;
    private Transform _target;
    private float _rotateSpeed;
    
    #endregion
    

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        transform.localPosition += transform.up * _speed * Time.deltaTime;

        if ((_target.position - transform.localPosition).sqrMagnitude < _minDistSqrt)
        {
            _target.GetComponent<AirCraft>().Kill();
            Destroy(gameObject);
        }

        switch (_followMode)
        {
            case RocketLauncher.RocketFollowMode.Off:
                break;
            case RocketLauncher.RocketFollowMode.LocalLeadPoint:

                Vector3 _leadPoint = RocketLauncher.FirstOrderIntercept(transform.position, Vector3.zero, _speed,
                    _target.position, RocketLauncher.GetTargetVelocity());
                
                float rotateZ = transform.rotation.eulerAngles.z;
                transform.LookAt(_leadPoint);
                if (_leadPoint.x < transform.position.x)
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
                
                break;
        }
    }

    public void SetParams(float Speed,float MinDist,float Rotate, RocketLauncher.RocketFollowMode FollowMode, Transform Target)
    {
        _speed = Speed;
        _minDistSqrt = MinDist;
        _followMode = FollowMode;
        _target = Target;
        _rotateSpeed = Rotate;
    }
}
