using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCraftSpawner : MonoBehaviour
{

    /// <summary>
    /// Создает самолеты
    /// </summary>
    
    #region Инспектор
    [SerializeField] private RocketLauncher _mainLauncher;
    [SerializeField] private GameObject _airCraftPrefab;
    /// <summary>
    /// Список с кривой поворотов (где 1\-1 это 360 и -360, где T = время)
    /// </summary>
    [SerializeField] private AnimationCurve[] _airCraftMoveCurve;
/// <summary>
/// Минимальная и Максимальна скорость для самолета (d км\с)
/// </summary>
    [SerializeField] private Vector2 _minMaxACSpeed = new Vector2(0.2f,0.4f);
    /// <summary>
    /// Минимальная и Максимальна задержка перед спавно самолета
    /// </summary>
    [SerializeField] private Vector2 _minMaxACSpawn = new Vector2(0.4f, 2f);

    #endregion

    #region Локальные переменные

    

    #endregion
    
    

    public void Regen()
    {
        StartCoroutine(RegenWait());
    }

    IEnumerator RegenWait()
    {
        float localWaitTimer = Random.Range(_minMaxACSpawn.x,_minMaxACSpawn.y);
        
        while (localWaitTimer > 0)
        {
            localWaitTimer -= Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }

        GameObject newAirCraft = Instantiate(_airCraftPrefab,transform);
        newAirCraft.transform.localPosition = ((Vector3) Random.insideUnitCircle) * 7.5f;
        newAirCraft.GetComponent<AirCraft>().SetRotateCurve(_airCraftMoveCurve[Random.Range(0,_airCraftMoveCurve.Length)]);
        newAirCraft.GetComponent<AirCraft>().SetSpeed(Random.Range(_minMaxACSpeed.x,_minMaxACSpeed.y));

        _mainLauncher.SetAirCraft(newAirCraft.transform,newAirCraft.GetComponent<AirCraft>());
    }

    // Start is called before the first frame update
    void Start()
    {
        Regen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
