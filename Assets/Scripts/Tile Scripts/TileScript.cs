using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    public GameManager gmManager;

    [SerializeField] private Sprite[] _tileSet;
    private SpriteRenderer _spriteRenderer;

    private int _xPos;
    private int _yPos;

    public void Start()
    {
        gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        int x = Random.Range(0, 10);

        if(x <= 5)
        {
            _spriteRenderer.sprite = _tileSet[0];
        }
        else if(x == 6)
        {
            _spriteRenderer.sprite = _tileSet[1];
        }
        else if (x == 7)
        {
            _spriteRenderer.sprite = _tileSet[2];
        }
        else if (x == 8)
        {
            _spriteRenderer.sprite = _tileSet[3];
        }
        else if (x == 9)
        {
            _spriteRenderer.sprite = _tileSet[4];
        }
    }

    public void SetCoords(int x, int y)
    {
        _xPos = x;
        _yPos = y;
    }
}
