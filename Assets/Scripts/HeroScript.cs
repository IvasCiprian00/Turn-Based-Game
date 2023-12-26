using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroScript : MonoBehaviour
{
    [SerializeField] private int _xPos;
    [SerializeField] private int _yPos;

    [Header("Hero Attributes")]
    [SerializeField] private int _damage;
    [SerializeField] private int _speed;

    [SerializeField]
    private string _movementType;

    public GameManager gmManager;

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
}
