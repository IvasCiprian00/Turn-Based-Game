using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    [SerializeField] private GameManager _gmManager;
    [SerializeField] private GameObject _target;
    [SerializeField] private HeroScript _hsScript;

    [SerializeField] private Animator _animator;

    [Header("Enemy Stats")]
    [SerializeField] private int _hp;
    [SerializeField] private int _damage;
    [SerializeField] private int _xPos;
    [SerializeField] private int _yPos;
    [SerializeField] private float _waitDuration = 0.2f;

    public void Start()
    {
        SetZPos(-3);
        _gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void StartTurn()
    {
        StartCoroutine(TakeTurn());
    }

    IEnumerator TakeTurn()
    {
        FindTarget();

        yield return new WaitForSeconds(_waitDuration);

        Movement();

        yield return new WaitForSeconds(_waitDuration);

        EndTurn();
    }

    public void FindTarget()
    {
        int minDistance = 99;

        for(int i = 0; i < _gmManager.heroes.Length; i++)
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

    public void EndTurn()
    {
        _gmManager.currentEnemy++;

        if(_gmManager.currentEnemy >= _gmManager.enemies.Length)
        {
            //Debug.Log("YEY");
            _gmManager.currentEnemy = 0;

            _gmManager.StartHeroTurns();

            return;
        }

        _gmManager.enemies[_gmManager.currentEnemy].GetComponent<EnemyScript>().StartTurn();
    }

    public void Attack()
    {
        _hsScript.TakeDamage(_damage);
    }

    public void MoveTowards()
    {
        int pastXPos = _xPos;
        int pastYPos = _yPos;

        if(_xPos <  _hsScript.GetXPos() && _gmManager.gameBoard[_xPos + 1, _yPos] == null)
        {
            _xPos++;
        }
        else if(_xPos > _hsScript.GetXPos() && _gmManager.gameBoard[_xPos - 1, _yPos] == null)
        {
            _xPos--;
        }
        else if(_yPos < _hsScript.GetYPos() && _gmManager.gameBoard[_xPos, _yPos + 1] == null)
        {
            _yPos++;
        }
        else if(_yPos > _hsScript.GetYPos() && _gmManager.gameBoard[_xPos, _yPos - 1] == null)
        {
            _yPos--;
        }

        UpdatePosition(pastXPos, pastYPos);
    }

    public void UpdatePosition(int x, int y)
    {
        transform.position = _gmManager.tiles[_xPos, _yPos].transform.position;
        SetZPos(-3);

        _gmManager.gameBoard[x, y] = null;
        _gmManager.gameBoard[_xPos, _yPos] = gameObject;
    }

    public bool CanAttack(HeroScript targetScript)
    {
        //Debug.Log(GetDistance(_xPos, targetScript.GetXPos(), _yPos, targetScript.GetYPos()));

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

        if(_hp <= 0)
        {
            _gmManager.CharacterDeath(gameObject, _gmManager.enemies);
            Destroy(gameObject);
        }

        if(_animator != null)
        {
            _animator.SetTrigger("take_damage");
        }
    }

    public void SetCoords(int x, int y)
    {
        _xPos = x;
        _yPos = y;
    }

    public void SetZPos(int z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
}
