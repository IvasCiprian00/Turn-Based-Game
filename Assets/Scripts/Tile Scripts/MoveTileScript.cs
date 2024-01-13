using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTileScript : MonoBehaviour
{
    public GameManager gmManager;
    private bool _attackTile;

    [SerializeField] private Sprite _attackTileSprite;

    [SerializeField]private int _xPos;
    [SerializeField]private int _yPos;

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
        HeroScript hsScript = gmManager.heroes[gmManager.currentHero].GetComponent<HeroScript>();
        int pastXPos = hsScript.GetXPos();
        int pastYPos = hsScript.GetYPos();


        if (_attackTile)
        {
            int damageDealt = hsScript.GetDamage();

            gmManager.gameBoard[_xPos, _yPos].GetComponent<EnemyScript>().TakeDamage(damageDealt);
            gmManager.attacksLeft--;
            gmManager.GenerateMoveTiles();

            return;
        }

        gmManager.speedLeft -= Mathf.Abs(_xPos - pastXPos) + Mathf.Abs(_yPos - pastYPos);

        gmManager.gameBoard[pastXPos, pastYPos] = null;

        gmManager.heroes[gmManager.currentHero].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2f);

        hsScript.SetCoords(_xPos, _yPos);
        gmManager.gameBoard[_xPos, _yPos] = gmManager.heroes[gmManager.currentHero];
        gmManager.GenerateMoveTiles();
    }

    public void ShowCoords()
    {
        Debug.Log(_xPos + " " +  _yPos);
    }

    public void SetAttacking(bool attacking)
    {
        if(attacking)
        {
            GetComponent<SpriteRenderer>().sprite = _attackTileSprite;
        }

        _attackTile = attacking;
    }
}
