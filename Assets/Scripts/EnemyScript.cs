using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]private int _hp;

    public void TakeDamage(int damage)
    {
        _hp -= damage;
    }
}
