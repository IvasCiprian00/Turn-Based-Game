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
        public int xPos;
        public int yPos;
    };

    public EnemyInfo[] enemies;

    [SerializeField] private EnemyScript _enmScript;
    [SerializeField] private GameManager _gmManager;

    /*public void Start()
    {
        //_gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        _gmManager.enemies = new GameObject[enemies.Length];

        for (int i = 0; i < enemies.Length; i++)
        {
            _enmScript = enemies[i].enemy.GetComponent<EnemyScript>();

            _enmScript.SetCoords(enemies[i].xPos, enemies[i].yPos);
            _gmManager.enemies[i] = Instantiate(enemies[i].enemy);

            int enemyXPos = enemies[i].xPos;
            int enemyYPos = enemies[i].yPos;

            //Transform enemyPosition = _gmManager.GetTilePosition(enemyXPos, enemyYPos);
        }
    }*/
}
