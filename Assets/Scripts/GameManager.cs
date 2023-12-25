using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _moveTile;
    [SerializeField] private GameObject _dummy;


    [Header("Hero Section")]
    public GameObject[] heroes;
    public int numberOfHeroes;
    public int currentHero;
    public bool canMove = true;

    private int numberOfLines = 8;
    private int numberOfColumns = 8;

    private bool _attacking = false;

    public GameObject[,] tiles;
    public GameObject[,] gameBoard;


    public void Start()
    {
        float xPos = -2.33f;
        float yPos = 1f;
        float positionIncrement = 0.66f;

        tiles = new GameObject[numberOfLines, numberOfColumns];
        gameBoard = new GameObject[numberOfLines, numberOfColumns];

        for (int i = 0; i < numberOfLines; i++)
        {
            xPos = -2.33f;

            for(int j = 0; j < numberOfColumns; j++)
            {
                tiles[i, j] = Instantiate(_tile, new Vector3(xPos, yPos, -1), Quaternion.identity, GameObject.Find("Tile Container").transform);
                tiles[i, j].GetComponent<TileScript>().SetCoords(i, j);

                xPos += positionIncrement;
            }

            yPos -= positionIncrement;
        }

        for(int i = 0; i < 4; i++)
        {
            int linePos = 5;
            int colPos = i + 1;

            heroes[i].GetComponent<HeroScript>().SetCoords(linePos, colPos);
            gameBoard[linePos, colPos] = heroes[i];
            heroes[i].transform.position = new Vector3(tiles[linePos, colPos].transform.position.x, tiles[linePos, colPos].transform.position.y, tiles[linePos, colPos].transform.position.z - 1f);
        }

        gameBoard[4, 3] = Instantiate(_dummy, tiles[4, 3].transform.position - new Vector3(0, 0, 2), Quaternion.identity);

        GenerateMoveTiles();
    }

    public void EndTurn()
    {
        currentHero++;
        canMove = true;

        if(currentHero >= numberOfHeroes)
        {
            currentHero = 0;
        }

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
        HeroScript hsScript = heroes[currentHero].GetComponent<HeroScript>();
        string mvmt = hsScript.GetMovementType();
        int[] lineVector;
        int[] colVector;
        switch (mvmt)
        {
            case "basic":
                lineVector = new int[8]{ -1, -1, -1, 0, 1, 1, 1, 0 };
                colVector = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };

                SpawnBasicTiles(lineVector, colVector);
                break;
            case "fast":
                lineVector = new int[12] { -1, -1, -1, 0, 1, 1, 1, 0 , -2, 0, 2, 0};
                colVector = new int[12] { -1, 0, 1, 1, 1, 0, -1, -1 ,0 , 2, 0, -2};

                SpawnBasicTiles(lineVector, colVector);
                break;
            case "teleport":
                SpawnTeleportTiles();
                break;
            default: break;
        }
    }

    public void SpawnBasicTiles(int[] lineChange, int[] colChange)
    {
        HeroScript hsScript = heroes[currentHero].GetComponent<HeroScript>();

        int startingXPos = hsScript.GetXPos();
        int startingYPos = hsScript.GetYPos();

        for(int i = 0; i < lineChange.Length; i++) 
        {
            int newXPos = startingXPos + lineChange[i];
            int newYPos = startingYPos + colChange[i];

            _attacking = false;

            //Debug.Log(newXPos + " " + newYPos);

            if (newXPos < 0 || newXPos > numberOfLines - 1 || newYPos < 0 || newYPos > numberOfColumns - 1)
            {
                continue;
            }

            if (gameBoard[newXPos, newYPos] != null)
            {
                if (gameBoard[newXPos, newYPos].tag == "Enemy")
                {
                    _attacking = true;
                }
                else
                {
                    continue;
                }
            }

            Vector3 tilePosition = tiles[newXPos, newYPos].transform.position;
            tilePosition -= new Vector3(0, 0, 1);

            GameObject reference = Instantiate(_moveTile, tilePosition, Quaternion.identity);// without reference, the moveplates don't work correctly
            reference.GetComponent<MoveTileScript>().SetCoords(newXPos, newYPos);
            reference.GetComponent<MoveTileScript>().SetAttacking(_attacking);
        }
    }

    public void SpawnTeleportTiles()
    {

    }

    public bool IsAttacking()
    {
        return _attacking;
    }
}
