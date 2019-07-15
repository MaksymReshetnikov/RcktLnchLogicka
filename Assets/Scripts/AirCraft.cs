using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Это скрипт самолета. Скрипт приводит самолет в движение.
/// </summary>

public class AirCraft : MonoBehaviour
{
    #region Параметры самолета

    private AnimationCurve _rotateCurve;
    public float Speed;
    private bool _death;
    
    #endregion

    #region Приватные переменные

    private float _animTimer;

    #endregion

    public void SetRotateCurve(AnimationCurve setParam)
    {
        _rotateCurve = setParam;
        var keysList = _rotateCurve.keys;
        _animTimer = keysList[keysList.Length - 1].time;
    }

    public void SetSpeed(float speed)
    {
        Speed = speed;
        StartCoroutine(Fly());
    }

    public void Kill()
    {
        _death = true;
       
    }
    
    IEnumerator Fly()
    {
        float localRotateTimer = _animTimer;
        transform.Rotate(Random.Range(0f,360f)*transform.forward);
        while (!_death)
        {
            var viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
            {
                _death = true;
            }
            
            transform.localPosition += transform.up*Speed*Time.deltaTime;
            transform.Rotate(_rotateCurve.Evaluate(_animTimer-localRotateTimer)*Vector3.forward*Time.deltaTime*360);

            localRotateTimer -= Time.deltaTime * 1;
            if (localRotateTimer < 0)
            {
                localRotateTimer = _animTimer;
            }
            
            yield return new WaitForEndOfFrame();
        }
        
        
        transform.parent.GetComponent<AirCraftSpawner>().Regen();
        
        Destroy(gameObject);
    }
}
