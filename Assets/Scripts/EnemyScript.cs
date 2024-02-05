using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    [SerializeField] private GameManager _gmManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private HeroManager _heroManager;
    [SerializeField] private GameObject _target;
    [SerializeField] private HeroScript _hsScript;

    [SerializeField] private Animator _animator;

    [Header("Enemy Stats")]
    [SerializeField] private int _hp;
    [SerializeField] private int _damage;
    [SerializeField] private int _xPos;
    [SerializeField] private int _yPos;
    [SerializeField] private float _waitDuration = 0.2f;

    [Serializable]
    public struct PathCoordinates
    {
        public int x;
        public int y;
    }

    public struct PathGrid
    {
        public int state;
        public int distance;
    }

    [Header("Pathfinding")]
    [SerializeField] private int endX;
    [SerializeField] private int endY;
    [SerializeField] private int minPathLength;
    [SerializeField] private PathCoordinates[] currentPath = new PathCoordinates[50];
    [SerializeField] private PathCoordinates[] shortestPath = new PathCoordinates[50];
    //private int[,] grid;
    private PathGrid[,] grid;

    private int[] dl = { -1, 0, 1, 0 };
    private int[] dc = { 0, 1, 0, -1 };

    /*int[,] grid = {
            {0, 1, 0, 0, 0},
            {0, 1, 0, 1, 0},
            {0, 1, 0, 1, 0},
            {0, 1, 0, 1, 0},
            {0, 0, 0, 1, 0}
    };*/


    public void Awake()
    {
        _gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _enemyManager = GameObject.Find("Enemy Manager").GetComponent<EnemyManager>();
        _heroManager = GameObject.Find("Hero Manager").GetComponent<HeroManager>();
    }

    public void Start()
    {
        SetZPos(-3);
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

        for(int i = 0; i < _heroManager.GetHeroCount(); i++)
        {
            HeroScript hsScript = _heroManager.heroesAlive[i].GetComponent<HeroScript>();
            int distance = Mathf.Abs(_xPos - hsScript.GetXPos()) + Mathf.Abs(_yPos - hsScript.GetYPos());

            if(distance < minDistance)
            {
                minDistance = distance;
                _target = _heroManager.heroesAlive[i];
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

        if(_gmManager.currentEnemy >= _enemyManager.GetEnemyCount())
        {
            _gmManager.currentEnemy = 0;

            _gmManager.StartHeroTurns();

            return;
        }

        _enemyManager.enemiesAlive[_gmManager.currentEnemy].GetComponent<EnemyScript>().StartTurn();
    }

    public void Attack()
    {
        _hsScript.TakeDamage(_damage);
    }

    public void MoveTowards()
    {
        int pastXPos = _xPos;
        int pastYPos = _yPos;

        endX = _hsScript.GetXPos();
        endY = _hsScript.GetYPos();

        minPathLength = 30;
        CopyGrid();
        PathFinder(_xPos, _yPos, 0);
        if(minPathLength >= 2)
        {
            _xPos = shortestPath[1].x;
            _yPos = shortestPath[1].y;
        }
        Debug.Log(".");
        for(int i = 0; i < minPathLength; i++)
        {
            Debug.Log(shortestPath[i].x + " " + shortestPath[i].y);
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


    public void CopyGrid()
    {
        grid = new PathGrid[_gmManager.GetNumberOfLines(), _gmManager.GetNumberOfColumns()];

        for (int i = 0; i < _gmManager.GetNumberOfLines(); i++)
        {
            for (int j = 0; j < _gmManager.GetNumberOfColumns(); j++)
            {
                if (_gmManager.gameBoard[i, j] == null)
                {
                    grid[i, j].state = 0;
                }
                else
                {
                    grid[i, j].state = 1;
                }
            }
        }
    }

    public void PathFinder(int x, int y, int pathLength)
    {
        if (CanAttack(_hsScript, x, y))
        {
            currentPath[pathLength].x = x;
            currentPath[pathLength].y = y;
            pathLength++;
            if (pathLength < minPathLength)
            {
                minPathLength = pathLength;

                for (int i = 0; i < pathLength; i++)
                {
                    shortestPath[i] = currentPath[i];
                }
            }
            return;//change structure here for better nesting
        }
        for (int i = 0; i < 4; i++)
        {
            if (PositionIsValid(x + dl[i], y + dc[i], pathLength))
            {
                grid[x, y].state = -1;
                grid[x, y].distance = pathLength;
                currentPath[pathLength].x = x;
                currentPath[pathLength].y = y;
                PathFinder(x + dl[i], y + dc[i], pathLength + 1);
            }
        }
    }

    public bool PositionIsValid(int x, int y, int currentDistance)
    {
        if (x < 0 || x >= _gmManager.GetNumberOfLines() || y < 0 || y >= _gmManager.GetNumberOfColumns())
        {
            return false;
        }
        if (grid[x, y].state == 1)
        {
            return false;
        }
        if (grid[x, y].state == -1 && grid[x, y].distance <= currentDistance + 1)
        {
            return false;
        }
        return true;
    }

    public bool CanAttack(HeroScript targetScript, int x, int y)
    {
        if (GetDistance(x, targetScript.GetXPos(), y, targetScript.GetYPos()) == 1)
        {
            return true;
        }

        return false;
    }

    public bool CanAttack(HeroScript targetScript)
    {
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
            _gmManager.EnemyDeath(gameObject, _xPos, _yPos);
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
