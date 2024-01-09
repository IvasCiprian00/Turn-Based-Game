using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
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
}
