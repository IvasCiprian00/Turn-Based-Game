using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public struct EnemyInfo
    {
        public GameObject enemy;
        public int startingXPos;
        public int startingYPos;
    }

    public EnemyInfo[] enemyList;
    [SerializeField] private int nrOfEnemies;

    [SerializeField] private EnemyScript _enmScript;
    [SerializeField] private GameManager _gmManager;

    public void TilesLoaded()
    {
        for (int i = 0; i < nrOfEnemies; i++)
        {
            int linePos = enemyList[i].startingXPos;
            int colPos = enemyList[i].startingYPos;

            enemyList[i].enemy = Instantiate(enemyList[i].enemy);
            enemyList[i].enemy.transform.position = _gmManager.GetTile(linePos, colPos).transform.position;
            enemyList[i].enemy.transform.position -= new Vector3(0, 0, 1);
            //_gmManager.gameBoard[linePos, colPos] = enemyList[i].enemy;
            enemyList[i].enemy.GetComponent<EnemyScript>().SetCoords(linePos, colPos);
        }
    }
}
