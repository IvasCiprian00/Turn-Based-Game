using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Hero Section")]
    public GameObject[] heroes;
    public int numberOfHeroes;
    public int currentHero;
    public bool canMove = true;

    public GameObject[,] tiles = new GameObject[8, 8];
    public GameObject[,] gameBoard = new GameObject[8, 8];

    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _moveTile;

    public void Start()
    {
        float xPos = -2.33f;
        float yPos = 1f;
        float positionIncrement = 0.66f;

        for (int i = 0; i < 8; i++)
        {
            xPos = -2.33f;

            for(int j = 0; j < 8; j++)
            {
                tiles[i, j] = Instantiate(_tile, new Vector3(xPos, yPos, -1), Quaternion.identity, GameObject.Find("Tile Container").transform);
                tiles[i, j].GetComponent<TileScript>().SetCoords(i, j);

                xPos += positionIncrement;
            }

            yPos -= positionIncrement;
        }

        for(int i = 0; i < 4; i++)
        {
            int linePos = 3;
            int colPos = i + 1;
            heroes[i].GetComponent<HeroScript>().SetCoords(linePos, colPos);
            gameBoard[linePos, colPos] = heroes[i];
            heroes[i].transform.position = new Vector3(tiles[linePos, colPos].transform.position.x, tiles[linePos, colPos].transform.position.y, tiles[linePos, colPos].transform.position.z - 1f);
        }

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

        switch (mvmt)
        {
            case "basic":
                SpawnBasicTiles();
                break;
            case "fast":
                SpawnFastTiles();
                break;
            case "teleport":
                SpawnTeleportTiles();
                break;
            default: break;
        }
    }

    public void SpawnBasicTiles()
    {
        HeroScript hsScript = heroes[currentHero].GetComponent<HeroScript>();

        int startingXPos = hsScript.GetXPos();
        int startingYPos = hsScript.GetYPos();

        int[] lineChange = {-1, -1, -1, 0, 1, 1, 1, 0};
        int[] colChange = {-1, 0, 1, 1, 1, 0, -1, -1};

        for(int i = 0; i < lineChange.Length; i++) 
        {
            int newXPos = startingXPos + lineChange[i];
            int newYPos = startingYPos + colChange[i];

            //Debug.Log(newXPos + " " + newYPos);

            if (newXPos < 0 || newXPos > 7 || newYPos < 0 || newYPos > 7)
            {
                continue;
            }

            if (gameBoard[newXPos, newYPos] != null)
            {
                continue;
            }


            Vector3 tilePosition = tiles[newXPos, newYPos].transform.position;
            tilePosition -= new Vector3(0, 0, 1);

            //Debug.Log(newXPos + " " + newYPos);
            _moveTile.GetComponent<MoveTileScript>().SetCoords(newXPos, newYPos);
            //_moveTile.GetComponent<MoveTileScript>().ShowCoords();

            Instantiate(_moveTile, tilePosition, Quaternion.identity);
        }
    }

    public void SpawnFastTiles()
    {

    }

    public void SpawnTeleportTiles()
    {

    }
}
