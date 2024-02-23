using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _moveTile;
    [SerializeField] private GameObject _dummy;
    [SerializeField] private int _levelNumber;

    [Header("Hero Section")]
    [SerializeField] private HeroManager _heroManager;
    public int currentHero;
    public int speedLeft;
    public int attacksLeft;
    public bool canMove = true;
    public HeroScript hsScript;
    [SerializeField] private GameObject _selectedEffect;
    private GameObject _effectReference;

    [Header("Enemy Section")]
    [SerializeField] private EnemyManager _enemyManager;
    public int currentEnemy;
    public EnemyScript enemyScript;

    private int numberOfLines = 6;
    private int numberOfColumns = 5;

    private bool _heroTurn = true;
    private bool _attacking = false;
    private bool _gameOver = false;
    private bool _sceneLoaded = false;

    public GameObject[,] tiles;
    public GameObject[,] gameBoard;

    public void Awake()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }
    public void Start()
    {
        _levelNumber++;
        SceneManager.LoadScene(_levelNumber, LoadSceneMode.Additive);

        _uiManager.DisplayNextLevelButton(false);

        GenerateGameBoard(numberOfLines, numberOfColumns);

        InitializeHeroes();

        _effectReference = Instantiate(_selectedEffect, Vector3.zero, Quaternion.identity);

    }

    public void Update()
    {
        if (_gameOver)
        {
            DestroyMoveTiles();
            _uiManager.DisplayNextLevelButton(true);
            return;
        }

        if(!_sceneLoaded)
        {
            return;
        }

        _uiManager.DisplayNextLevelButton(false);

        if (_heroTurn)
        {
            _effectReference.SetActive(true);

            if (_heroManager.heroesAlive[currentHero] != null)
            {
                _effectReference.transform.position = _heroManager.heroesAlive[currentHero].transform.position;
            }
        }
        else
        {
            _effectReference.SetActive(false);
        }

        if (speedLeft <= 0)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }
    }

    public void LevelLoaded()
    {
        InitializeEnemies();

        StartHeroTurns();
        GameObject.Find("Stats Displayer").GetComponent<StatsDisplayer>().SetHeroManager();
        GameObject.Find("Stats Displayer").GetComponent<StatsDisplayer>().SetEnemyManager();
        GameObject.Find("Stats Displayer").GetComponent<StatsDisplayer>().GetActiveChars();

        _sceneLoaded = true;
    }

    public void CheckLevelProgress()
    {
        if(_heroManager.GetHeroCount() <= 0)
        {
            _gameOver = true;

            Debug.Log("Heroes Lost");
        }

        else if (_enemyManager.GetEnemyCount() <= 0)
        {
            _gameOver = true;

            DestroyMoveTiles();
            _uiManager.HideSkills();


            for (int i = 0; i < _heroManager.GetHeroCount(); i++)
            {
                _heroManager.heroesAlive[i].GetComponent<HeroScript>().StartWinAniamtion();
            }

            Debug.Log("Heroes Won");
        }
    }

    public void GoToNextLevel()
    {
        SceneManager.UnloadSceneAsync("Level_" + _levelNumber);

        _levelNumber++;

        SceneManager.LoadScene(_levelNumber, LoadSceneMode.Additive);

        ResetBoard();
    }

    public void ResetBoard()
    {
        ResetHeroes();

        _gameOver = false;

        _uiManager.DisplayNextLevelButton(false);
    }

    public void ResetHeroes()
    {
        for(int i = 0; i < _heroManager.heroesAlive.Length; i++)
        {
            Destroy(_heroManager.heroesAlive[i]);
        }

        _heroManager.SpawnHeroes();
    }

    private void InitializeEnemies()
    {
        _enemyManager = GameObject.Find("Enemy Manager").GetComponent<EnemyManager>();
        _enemyManager.SpawnEnemies();
    }

    public void InitializeHeroes()
    {
        _heroManager = GameObject.Find("Hero Manager").GetComponent<HeroManager>();
        _heroManager.SpawnHeroes();

        hsScript = _heroManager.heroesAlive[currentHero].GetComponent<HeroScript>();
        speedLeft = hsScript.GetSpeed();
    }

    private void GenerateGameBoard(int sizeX, int sizeY)
    {
        float xPos = -2.25f;
        float yPos = 3.5f;
        float positionIncrement = Mathf.Abs(xPos) * 2 / 4;

        tiles = new GameObject[sizeX, sizeY];
        gameBoard = new GameObject[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            xPos = -2.25f;

            for (int j = 0; j < sizeY; j++)
            {
                tiles[i, j] = Instantiate(_tile, new Vector3(xPos, yPos, -1), Quaternion.identity, GameObject.Find("Tile Container").transform);
                tiles[i, j].GetComponent<TileScript>().SetCoords(i, j);

                xPos += positionIncrement;
            }

            yPos -= positionIncrement;
        }

    }

    public void EndTurn()
    {
        _uiManager.HideSkills();
        _uiManager.CancelSkill();
        currentHero++;

        if(currentHero >= _heroManager.GetHeroCount() || currentHero == -1)
        {
            currentHero = 0;

            StartEnemyTurns();

            return;
        }

        hsScript = _heroManager.heroesAlive[currentHero].GetComponent<HeroScript>();
        speedLeft = hsScript.GetSpeed();
        attacksLeft = hsScript.GetSpeed();

        _uiManager.DisplaySkills(hsScript.skills);
        GenerateMoveTiles();
    }

    public void StartEnemyTurns()
    {
        DestroyMoveTiles();
        //Destroy(_effectReference);

        currentEnemy = 0;

        enemyScript = _enemyManager.enemiesAlive[currentEnemy].GetComponent<EnemyScript>();

        _heroTurn = false;
        enemyScript.StartTurn();
    }

    public void StartHeroTurns()
    {
        currentHero = 0;
        _heroTurn = true;
        //_effectReference = Instantiate(_selectedEffect, Vector3.zero, Quaternion.identity);

        hsScript = _heroManager.heroesAlive[currentHero].GetComponent<HeroScript>();
        speedLeft = hsScript.GetSpeed();
        attacksLeft = hsScript.GetNumberOfAttacks();

        _uiManager.DisplaySkills(hsScript.skills);

        GenerateMoveTiles();
    }

    public void GenerateMoveTiles()
    {
        DestroyMoveTiles();

        CreateMoveTiles();

        if (attacksLeft != 0){
            CreateAttackTiles();
        }

    }


    public void DestroyMoveTiles()
    {
        GameObject[] moveTiles = GameObject.FindGameObjectsWithTag("Move Tile");

        for (int i = 0; i < moveTiles.Length; i++)
        {
            Destroy(moveTiles[i]);
        }
    }

    public void CreateMoveTiles()
    {
        string mvmt = hsScript.GetMovementType();

        switch (mvmt)
        {
            case "basic":
                SpawnBasicTiles(speedLeft);
                break;
            case "fast":
                SpawnBasicTiles(2);
                break;
            case "teleport":
                SpawnTeleportTiles();
                break;
            default: break;
        }

    }

    public void CreateAttackTiles()
    {
        string attackType = hsScript.GetAttackType();

        if (attackType == "mixed" || attackType == "ranged")
        {
            int range = hsScript.GetRange();

            DirectionalCheck(0, 1, range);
            DirectionalCheck(1, 0, range);
            DirectionalCheck(0, -1, range);
            DirectionalCheck(-1, 0, range);
            DirectionalCheck(1, 1, range);
            DirectionalCheck(1, -1, range);
            DirectionalCheck(-1, -1, range);
            DirectionalCheck(-1, 1, range);
        }

        if (attackType == "melee")
        {
            DirectionalCheck(0, 1, 1);
            DirectionalCheck(1, 0, 1);
            DirectionalCheck(0, -1, 1);
            DirectionalCheck(-1, 0, 1);
        }
    }

    public void DirectionalCheck(int line, int col, int n)
    {
        int currentLine = hsScript.GetXPos();
        int currentCol = hsScript.GetYPos();

        for(int i = 0; i< n; i++)
        {
            currentLine += line;
            currentCol += col;

            if(!PositionIsValid(currentLine, currentCol))
            {
                continue;
            }

            if(gameBoard[currentLine, currentCol] == null)
            {
                continue;
            }

            if (gameBoard[currentLine, currentCol].tag == "Enemy")
            {
                SpawnTile(true, currentLine, currentCol);
            }
        }
    }

    public bool PositionIsValid(int x, int y)
    {
        if(x >= 0 && x < numberOfLines && y >= 0 && y < numberOfColumns)
        {
            return true;
        }

        return false;
    }

    public void SpawnBasicTiles(int speed)
    {
        int firstLine = hsScript.GetXPos() - speed;
        int firstCol = hsScript.GetYPos() - speed;

        int lastLine = hsScript.GetXPos() + speed;
        int lastCol = hsScript.GetYPos() + speed;

        for (int i = firstLine; i <= lastLine; i++)
        {
            for(int j = firstCol; j <= lastCol; j++)
            {
                _attacking = false;

                if(!PositionIsValid(i, j))
                {
                    continue;
                }

                if(Mathf.Abs(hsScript.GetXPos() - i) + Mathf.Abs(hsScript.GetYPos() - j) > speed)
                {
                    continue;
                }

                if (gameBoard[i, j] != null)
                {
                    continue;
                }

                SpawnTile(_attacking, i, j);
            }
        }
    }

    public void SpawnTile(bool isAttackTile, int line, int col)
    {
        Vector3 tilePosition = tiles[line, col].transform.position;
        tilePosition -= new Vector3(0, 0, 1);

        GameObject reference = Instantiate(_moveTile, tilePosition, Quaternion.identity); 
        reference.GetComponent<MoveTileScript>().SetCoords(line, col);
        reference.GetComponent<MoveTileScript>().SetAttacking(isAttackTile);
    }

    public void SpawnTeleportTiles()
    {

    }

    public void HeroDeath(GameObject deadChar, int posX, int posY)
    {
        gameBoard[posX, posY] = null;

        for(int i = 0; i < _heroManager.GetHeroCount(); i++) 
        {
            if(deadChar == _heroManager.heroesAlive[i])
            {
                _heroManager.SetHeroCount(_heroManager.GetHeroCount() - 1);

                CheckLevelProgress();

                RemoveDeadHero(i);

                return;
            }
        }
    }

    public void RemoveDeadHero(int index)
    {
        for(int i = index; i < _heroManager.GetHeroCount(); i++)
        {
            _heroManager.heroesAlive[i] = _heroManager.heroesAlive[i + 1];
        }

    }

    public void EnemyDeath(GameObject deadChar, int posX, int posY)
    {
        gameBoard[posX, posY] = null;

        for (int i = 0; i < _enemyManager.GetEnemyCount(); i++)
        {
            if (deadChar == _enemyManager.enemiesAlive[i])
            {
                _enemyManager.SetEnemyCount(_enemyManager.GetEnemyCount() - 1);
                CheckLevelProgress();

                RemoveDeadEnemy(i);

                return;
            }
        }
    }

    public void RemoveDeadEnemy(int index)
    {
        for (int i = index; i < _enemyManager.GetEnemyCount(); i++)
        {
            _enemyManager.enemiesAlive[i] = _enemyManager.enemiesAlive[i  + 1];
        }
    }

    public bool IsAttacking()
    {
        return _attacking;
    }
    
    public bool IsHeroTurn()
    {
        return _heroTurn;
    }

    public int GetNumberOfLines(){ return numberOfLines; }

    public int GetNumberOfColumns(){ return numberOfColumns; }

    public GameObject GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    public void SetZPos(GameObject thing, int z)
    {
        thing.transform.position = new Vector3(thing.transform.position.x, thing.transform.position.y, z);
    }

    public bool IsSceneLoaded() { return _sceneLoaded; }

    public bool IsGameOver() { return _gameOver; }
}
