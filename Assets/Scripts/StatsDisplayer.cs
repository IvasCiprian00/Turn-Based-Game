using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsDisplayer : MonoBehaviour
{
    private HeroManager _heroManager;
    private EnemyManager _enemyManager;

    [SerializeField] private GameObject _stats;
    [SerializeField] private GameObject[] _activeChars;
    [SerializeField] private TextMeshProUGUI[] _hpLeft;

    public void GetActiveChars()
    {
        if(_heroManager == null || _enemyManager == null)
        {
            return;
        }

        int heroCount = _heroManager.GetHeroCount();
        int enemyCount = _enemyManager.GetEnemyCount();
        int total = heroCount + enemyCount;

        _activeChars = new GameObject[total];

        for(int i = 0; i < heroCount; i++)
        {
            _activeChars[i] = _heroManager.heroesAlive[i];

            AsignStats(_activeChars[i]);
        }
        for(int i = heroCount; i < total; i++)
        {
            _activeChars[i] = _enemyManager.enemiesAlive[i - heroCount];

            AsignStats(_activeChars[i]);
        }
    }

    public void AsignStats(GameObject character)
    {
        GameObject reference = Instantiate(_stats, character.transform.position, Quaternion.identity, gameObject.transform);
        reference.GetComponent<StatsScript>().SetTarget(character);
    }

    public void SetHeroManager()
    {
        _heroManager = GameObject.Find("Hero Manager").GetComponent<HeroManager>();
    }

    public void SetEnemyManager()
    {
        _enemyManager = GameObject.Find("Enemy Manager").GetComponent<EnemyManager>();
    }
}
