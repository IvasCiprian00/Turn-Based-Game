using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTileScript : MonoBehaviour
{
    public GameManager gmManager;

    private int _xPos;
    private int _yPos;

    public void Start()
    {
        gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void SetCoords(int x, int y)
    {
        _xPos = x;
        _yPos = y;
    }

    public void OnMouseUp()
    {
        /*if (gmManager.gameBoard[_xPos, _yPos] != null || gmManager.gameBoard[_xPos, _yPos].tag != "Move Tile")
        {
            Debug.Log(gmManager.gameBoard[_xPos, _yPos].name);
            return;
        }*/

        if (gmManager.canMove)
        {
            HeroScript hsScript = gmManager.heroes[gmManager.currentHero].GetComponent<HeroScript>();
            int pastXPos = hsScript.GetXPos();
            int pastYPos = hsScript.GetYPos();

            gmManager.gameBoard[pastXPos, pastYPos] = null;

            gmManager.heroes[gmManager.currentHero].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2f);
            hsScript.SetCoords(_xPos, _yPos);
            gmManager.gameBoard[_xPos, _yPos] = gmManager.heroes[gmManager.currentHero];
        }

        gmManager.canMove = false;

        gmManager.EndTurn();
    }
}
