using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    [Serializable]
    public struct HeroInfo
    {
        public GameObject hero;
        public int startingXPos;
        public int startingYPos;
    }

    public HeroInfo[] heroList;
    public GameObject[] heroesAlive;
    [SerializeField] private int _heroCount;

    [SerializeField] private GameManager _gameManager;

    public void Awake()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void SpawnHeroes()
    {
        _heroCount = heroList.Length;
        heroesAlive = new GameObject[_heroCount];

        for (int i = 0; i < _heroCount; i++)
        {
            int linePos = heroList[i].startingXPos;
            int colPos = heroList[i].startingYPos;

            heroesAlive[i] = Instantiate(heroList[i].hero);
            heroesAlive[i].transform.position = _gameManager.GetTile(linePos, colPos).transform.position;
            heroesAlive[i].transform.position -= new Vector3(0, 0, 1);
            _gameManager.gameBoard[linePos, colPos] = heroesAlive[i];
            heroesAlive[i].GetComponent<HeroScript>().SetCoords(linePos, colPos);
            /*heroList[i].hero = Instantiate(heroList[i].hero);
            heroList[i].hero.transform.position = _gameManager.GetTile(linePos, colPos).transform.position;
            heroList[i].hero.transform.position -= new Vector3(0, 0, 1);
            _gameManager.gameBoard[linePos, colPos] = heroList[i].hero;
            heroList[i].hero.GetComponent<HeroScript>().SetCoords(linePos, colPos);*/
        }
    }

    public int GetHeroCount() { return _heroCount; }
    public void SetHeroCount(int count) { _heroCount = count; }
}
