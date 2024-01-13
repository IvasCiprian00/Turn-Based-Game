using JetBrains.Annotations;
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
    [SerializeField] private int _maxHp;
    [SerializeField] private int _damage;
    [SerializeField] private int _speed;
    [SerializeField] private int _nrOfAttacks;
    public GameObject[] skills;

    [SerializeField] private string _movementType;
    [SerializeField] private string _attackType;
    [SerializeField] private int _range;

    public GameManager gmManager;

    public void Start()
    {
        gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void Heal(int healValue)
    {
        _hp += healValue;

        if(_hp > _maxHp)
        {
            _hp = _maxHp;
        }
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
            gmManager.CharacterDeath(gameObject, gmManager.heroes, ref gmManager.nrOfHeroes, _xPos, _yPos);
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
    public string GetAttackType() { return _attackType;}
    public int GetRange() { return _range;}
    public int GetNumberOfAttacks() { return _nrOfAttacks; }
}
