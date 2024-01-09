using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTileScript : MonoBehaviour
{
    private int _xPos;
    private int _yPos;

    private UIManager _uiManager;

    public void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    public void SetCoords(int x, int y)
    {
        _xPos = x;
        _yPos = y;
    }

    public void OnMouseUp()
    {
        _uiManager.DisplayCofirmButtons(true);
        DestroyOtherTiles();
    }

    public void DestroyOtherTiles()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Select Tile");

        for(int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null) {
                Debug.Log("?");
                continue;
            }

            SelectTileScript stScript = tiles[i].GetComponent<SelectTileScript>();

            if(stScript.GetXPos() != _xPos || stScript.GetYPos() != _yPos)
            {
                Destroy(tiles[i]);
            }
        }
    }

    public int GetXPos() { return _xPos; }
    public int GetYPos() { return _yPos; }
}
