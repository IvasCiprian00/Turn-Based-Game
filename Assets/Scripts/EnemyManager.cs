using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Serializable]
    public struct EnemyInfo
    {
        public GameObject enemy;
        public int startingXPos;
        public int startingYPos;
    }

    public EnemyInfo[] enemyList;
    public GameObject[] enemiesAlive;
    [SerializeField] private int _enemyCount;

    [SerializeField] private EnemyScript _enmScript;
    [SerializeField] private GameManager _gameManager;

    public void Awake()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void Start()
    {
        _gameManager.LevelLoaded();
    }

    public void SpawnEnemies()
    {
        _enemyCount = enemyList.Length;
        enemiesAlive = new GameObject[_enemyCount];

        for (int i = 0; i < _enemyCount; i++)
        {
            int linePos = enemyList[i].startingXPos;
            int colPos = enemyList[i].startingYPos;

            enemiesAlive[i] = Instantiate(enemyList[i].enemy);
            enemiesAlive[i].transform.position = _gameManager.GetTile(linePos, colPos).transform.position;
            enemiesAlive[i].transform.position -= new Vector3(0, 0, 1);
            _gameManager.gameBoard[linePos, colPos] = enemiesAlive[i];
            enemiesAlive[i].GetComponent<EnemyScript>().SetCoords(linePos, colPos);
            /*
            enemyList[i].enemy = Instantiate(enemyList[i].enemy);
            enemyList[i].enemy.transform.position = _gameManager.GetTile(linePos, colPos).transform.position;
            enemyList[i].enemy.transform.position -= new Vector3(0, 0, 1);
            _gameManager.gameBoard[linePos, colPos] = enemyList[i].enemy;
            enemyList[i].enemy.GetComponent<EnemyScript>().SetCoords(linePos, colPos);*/
        }
    }

    public int GetEnemyCount() { return _enemyCount; }
    public void SetEnemyCount(int count) { _enemyCount = count; }
}
