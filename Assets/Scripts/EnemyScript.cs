using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private int _hp;
    [SerializeField] private int _damage;
    [SerializeField] private int _xPos;
    [SerializeField] private int _yPos;

    [SerializeField] private GameManager _gmManager;
    [SerializeField] private GameObject _target;
    [SerializeField] private HeroScript _hsScript;

    public void Start()
    {
        _gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void StartTurn()
    {
        FindTarget();
        Movement();
    }

    public void FindTarget()
    {
        int minDistance = 99;

        for(int i = 0; i < _gmManager.numberOfHeroes; i++)
        {
            HeroScript hsScript = _gmManager.heroes[i].GetComponent<HeroScript>();
            int distance = Mathf.Abs(_xPos - hsScript.GetXPos()) + Mathf.Abs(_yPos - hsScript.GetYPos());

            if(distance < minDistance)
            {
                minDistance = distance;
                _target = _gmManager.heroes[i];
            }
        }

        _hsScript = _target.GetComponent<HeroScript>();
    }

    public void Movement()
    {
        if (!CanAttack(_hsScript))
        {
            MoveTowards();
        }
        else
        {
            Attack();
        }
    }

    public void Attack()
    {
        _hsScript.TakeDamage(_damage);
    }

    public void MoveTowards()
    {
        int pastXPos = _xPos;
        int pastYPos = _yPos;

        if(_xPos <  _hsScript.GetXPos())
        {
            _xPos++;
        }
        else if(_xPos > _hsScript.GetXPos())
        {
            _xPos--;
        }
        else if(_yPos < _hsScript.GetYPos())
        {
            _yPos++;
        }
        else if(_yPos > _hsScript.GetYPos())
        {
            _yPos--;
        }

        UpdatePosition(pastXPos, pastYPos);
    }

    public void UpdatePosition(int x, int y)
    {
        //transform.parent.transform.position = _gmManager.tiles[_xPos, _yPos].transform.position - new Vector3(0, 0, 1);
        transform.position = _gmManager.tiles[_xPos, _yPos].transform.position - new Vector3(0, 0, 1);
        _gmManager.gameBoard[x, y] = null;
        _gmManager.gameBoard[_xPos, _yPos] = gameObject;
    }

    public bool CanAttack(HeroScript targetScript)
    {
        Debug.Log(GetDistance(_xPos, targetScript.GetXPos(), _yPos, targetScript.GetYPos()));

        if (GetDistance(_xPos, targetScript.GetXPos(), _yPos, targetScript.GetYPos()) == 1)
        {
            return true;
        }

        return false;
    }

    public int GetDistance(int x, int x2, int y, int y2)
    {
        return Mathf.Abs(x - x2) + Mathf.Abs(y - y2);
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
    }

    public void SetCoords(int x, int y)
    {
        _xPos = x;
        _yPos = y;
    }
}
