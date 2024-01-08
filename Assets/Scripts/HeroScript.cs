using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroScript : MonoBehaviour
{
    [SerializeField] private int _xPos;
    [SerializeField] private int _yPos;
    [SerializeField] Animator _animator;

    [Header("Hero Attributes")]
    [SerializeField] private int _hp;
    [SerializeField] private int _damage;
    [SerializeField] private int _speed;

    [SerializeField]
    private string _movementType;

    public GameManager gmManager;

    public void Start()
    {
        gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;

        if (_animator != null)
        {
            _animator.SetTrigger("take_damage");
        }

        if (_hp <= 0 )
        {
            gmManager.HeroDeath(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetCoords(int x, int y)
    {
        _xPos = x;
        _yPos = y;
    }

    public int GetXPos() { return _xPos; }
    public int GetYPos() { return _yPos; }
    public string GetMovementType() { return _movementType; }
    public int GetDamage() { return _damage; }
    public int GetSpeed() { return _speed;}
    public int GetHp() { return _hp;}
}
