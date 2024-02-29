using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    enum TargetType
    {
        closest,
        lowestHp
    }

    [SerializeField] private GameManager _gmManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private HeroManager _heroManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameObject _target;
    [SerializeField] private HeroScript _hsScript;

    [SerializeField] private Animator _animator;

    [Header("Enemy Stats")]
    [SerializeField] private int _hp;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _damage;
    [SerializeField] private int _xPos;
    [SerializeField] private int _yPos;
    [SerializeField] private TargetType _targetType;
    [SerializeField] private float _waitDuration = 0.2f;


    private bool _isMoving;
    private GameObject _targetTile;

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
    private PathGrid[,] grid;

    private int[] dl = { -1, 0, 1, 0 };
    private int[] dc = { 0, 1, 0, -1 };

    public void Awake()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _enemyManager = GameObject.Find("Enemy Manager").GetComponent<EnemyManager>();
        _heroManager = GameObject.Find("Hero Manager").GetComponent<HeroManager>();
    }

    public void Start()
    {
        _hp = _maxHp;
    }
    public void Update()
    {
        if (_isMoving)
        {
            var step = 10 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _targetTile.transform.position, step);

            if (Vector3.Distance(transform.position, _targetTile.transform.position) < 0.001f)
            {
                transform.position = _targetTile.transform.position;
                _isMoving = false;
                //Camera.main.transform.parent = null;
            }
        }
    }

    public void StartTurn()
    {
        StartCoroutine(TakeTurn());
    }

    IEnumerator TakeTurn()
    {
        //Find target
        //Attack if it can
        //Otherwise move towards target
        //Attack if it can
        //Move again if it can
        //While(attacksLeft != 0 && speedLeft != 0)
        //try to attack or move
        //decrease attacks left or speed left depending on action taken
        //end turn
        FindTarget();

        yield return new WaitForSeconds(_waitDuration);

        Movement();

        yield return new WaitForSeconds(_waitDuration);

        EndTurn();
    }

    public void FindTarget()
    {
        int minDistance = 99;

        switch (_targetType)
        {
            case TargetType.closest:

                for (int i = 0; i < _heroManager.GetHeroCount(); i++)
                {
                    HeroScript hsScript = _heroManager.heroesAlive[i].GetComponent<HeroScript>();
                    int distance = Mathf.Abs(_xPos - hsScript.GetXPos()) + Mathf.Abs(_yPos - hsScript.GetYPos());

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        _target = _heroManager.heroesAlive[i];
                    }
                }

                break;

            case TargetType.lowestHp:

                int minHp = 99;

                for(int i = 0; i < _heroManager.GetHeroCount(); i++)
                {
                    HeroScript hsScript = _heroManager.heroesAlive[i].GetComponent<HeroScript>();

                    if(hsScript.GetHp() < minHp)
                    {
                        minHp = hsScript.GetHp();
                        _target = _heroManager.heroesAlive[i];
                    }
                }

                break;

            default: break;
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
        //Camera.main.transform.parent = transform;
        //Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
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

        UpdatePosition(pastXPos, pastYPos);
    }

    public void UpdatePosition(int x, int y)
    {
        //transform.position = _gmManager.tiles[_xPos, _yPos].transform.position;

        _gmManager.gameBoard[x, y] = null;
        _gmManager.gameBoard[_xPos, _yPos] = gameObject;

        _targetTile = _gmManager.tiles[_xPos, _yPos];
        _isMoving = true;
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
        _uiManager.DisplayDamageDealt(gameObject, damage);

        if (_hp <= 0)
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

    public int GetHp() { return _hp; }
    public int GetMaxHp() { return _maxHp; }
}
