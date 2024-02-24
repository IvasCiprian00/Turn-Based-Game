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

        int x = Random.Range(1, 10);

        _spriteRenderer.sprite = _tileSet[x];
    }

    public void SetCoords(int x, int y)
    {
        _xPos = x;
        _yPos = y;
    }
}
