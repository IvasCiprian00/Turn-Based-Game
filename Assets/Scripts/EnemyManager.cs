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
    [SerializeField] private int _enemyCount;

    [SerializeField] private EnemyScript _enmScript;
    [SerializeField] private GameManager _gameManager;

    public void Awake()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void SpawnEnemies()
    {
        _enemyCount = enemyList.Length;

        for (int i = 0; i < _enemyCount; i++)
        {
            int linePos = enemyList[i].startingXPos;
            int colPos = enemyList[i].startingYPos;

            enemyList[i].enemy = Instantiate(enemyList[i].enemy);
            enemyList[i].enemy.transform.position = _gameManager.GetTile(linePos, colPos).transform.position;
            enemyList[i].enemy.transform.position -= new Vector3(0, 0, 1);
            _gameManager.gameBoard[linePos, colPos] = enemyList[i].enemy;
            enemyList[i].enemy.GetComponent<EnemyScript>().SetCoords(linePos, colPos);
        }
    }

    public int GetEnemyCount() { return _enemyCount; }
    public void SetEnemyCount(int count) { _enemyCount = count; }
}
