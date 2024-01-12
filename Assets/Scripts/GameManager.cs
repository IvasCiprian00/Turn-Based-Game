using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _moveTile;
    [SerializeField] private GameObject _dummy;


    [Header("Hero Section")]
    public GameObject[] heroes;
    public int nrOfHeroes;
    public int currentHero;
    public int speedLeft;
    public bool canMove = true;
    public HeroScript hsScript;
    [SerializeField] private GameObject _selectedEffect;
    private GameObject _effectReference;

    [Header("Enemy Section")]
    public GameObject[] enemies;
    public int nrOfEnemies;
    public int currentEnemy;
    public EnemyScript enemyScript;

    private int numberOfLines = 7;
    private int numberOfColumns = 6;

    private bool _heroTurn = true;
    private bool _attacking = false;
    private bool _levelIsOver = false;

    public GameObject[,] tiles;
    public GameObject[,] gameBoard;


    public void Start()
    {
        GenerateGameBoard(numberOfLines, numberOfColumns);

        nrOfHeroes = heroes.Length;
        nrOfEnemies = enemies.Length;

        InitializeBoardElements();

        StartHeroTurns();
    }

    public void Update()
    {
        CheckLevelProgress();

        if (!_levelIsOver)
        {
            if (_heroTurn)
            {
                if (heroes[currentHero] != null)
                {
                    _effectReference.transform.position = heroes[currentHero].transform.position;
                }
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
    }

    public void CheckLevelProgress()
    {
        if(nrOfHeroes == 0)
        {
            _levelIsOver = true;

            Debug.Log("Heroes Lost");
        }
        else if (nrOfEnemies == 0)
        {
            _levelIsOver = true;

            Debug.Log("Heroes Won");
        }
    }

    private void InitializeBoardElements()
    {
        for (int i = 0; i < nrOfHeroes; i++)
        {
            int linePos = 5;
            int colPos = i + 1;

            heroes[i].GetComponent<HeroScript>().SetCoords(linePos, colPos);
            gameBoard[linePos, colPos] = heroes[i];
            heroes[i].transform.position = new Vector3(tiles[linePos, colPos].transform.position.x, tiles[linePos, colPos].transform.position.y, tiles[linePos, colPos].transform.position.z - 1f);
        }

        for(int i = 0; i < nrOfEnemies; i++)
        {
            int linePos = 3;

            enemies[i] = Instantiate(enemies[i]);
            enemies[i].GetComponent<EnemyScript>().SetCoords(linePos, i);
            gameBoard[linePos, i] = enemies[i];
            enemies[i].transform.position = new Vector3(tiles[linePos, i].transform.position.x, tiles[linePos, i].transform.position.y, tiles[linePos, i].transform.position.z - 1f);
        }

        hsScript = heroes[currentHero].GetComponent<HeroScript>();
        speedLeft = hsScript.GetSpeed();
        gameBoard[4, 3] = Instantiate(_dummy, tiles[4, 3].transform.position - new Vector3(0, 0, 2), Quaternion.identity);
    }

    private void GenerateGameBoard(int sizeX, int sizeY)
    {
        float xPos = -2.33f;
        float yPos = 3f;
        float positionIncrement = 0.932f;

        tiles = new GameObject[sizeX, sizeY];
        gameBoard = new GameObject[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            xPos = -2.33f;

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

        if(currentHero >= nrOfHeroes || currentHero == -1)
        {
            currentHero = 0;

            StartEnemyTurns();

            return;
        }

        hsScript = heroes[currentHero].GetComponent<HeroScript>();
        speedLeft = hsScript.GetSpeed();

        _uiManager.DisplaySkills(hsScript.skills);
        GenerateMoveTiles();
    }

    public void StartEnemyTurns()
    {
        DestroyMoveTiles();
        Destroy(_effectReference);

        currentEnemy = 0;

        enemyScript = enemies[currentEnemy].GetComponent<EnemyScript>();

        _heroTurn = false;
        enemyScript.StartTurn();
    }

    public void StartHeroTurns()
    {
        currentHero = 0;
        _heroTurn = true;
        _effectReference = Instantiate(_selectedEffect, Vector3.zero, Quaternion.identity);

        hsScript = heroes[currentHero].GetComponent<HeroScript>();
        speedLeft = hsScript.GetSpeed();

        _uiManager.DisplaySkills(hsScript.skills);

        GenerateMoveTiles();
    }

    public void GenerateMoveTiles()
    {
        DestroyMoveTiles();

        CreateMoveTiles();
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

                if(i < 0 || j < 0 || i > numberOfLines - 1 || j > numberOfColumns - 1)
                {
                    continue;
                }

                if(Mathf.Abs(hsScript.GetXPos() - i) + Mathf.Abs(hsScript.GetYPos() - j) > speed)
                {
                    continue;
                }

                if (gameBoard[i, j] != null)
                {
                    if (gameBoard[i, j].tag == "Enemy" && (hsScript.GetAttackType() == "melee" || hsScript.GetAttackType() == "mixed"))
                    {
                        _attacking = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                Vector3 tilePosition = tiles[i, j].transform.position;
                tilePosition -= new Vector3(0, 0, 1);

                GameObject reference = Instantiate(_moveTile, tilePosition, Quaternion.identity);   // without reference, the moveplates don't work correctly
                reference.GetComponent<MoveTileScript>().SetCoords(i, j);
                reference.GetComponent<MoveTileScript>().SetAttacking(_attacking);
            }
        }
    }

    public void SpawnTeleportTiles()
    {

    }

    public void CharacterDeath(GameObject deadChar, GameObject[] charArray, ref int nrOfCharacters)
    {
        for(int i = 0; i < nrOfCharacters; i++) 
        {
            if(deadChar == charArray[i])
            {
                RemoveDeadChar(i, charArray, nrOfCharacters);

                nrOfCharacters--;

                return;
            }
        }
    }


    public void RemoveDeadChar(int index, GameObject[] array, int charNumber)
    {
        for(int i = index; i < charNumber - 1; i++)
        {
            array[i] = array[i + 1];
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
}
