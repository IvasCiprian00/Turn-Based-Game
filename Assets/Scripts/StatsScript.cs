using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScript : MonoBehaviour
{
    private int _hp;
    private int _maxHp;
    private GameObject _target;

    public void Update()
    {
        if(_target != null)
        {
            transform.position = _target.transform.position;
            SetHp();
            SetHpColor();
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void SetHp()
    {
        if(_target.tag == "Enemy")
        {
            _hp = _target.GetComponent<EnemyScript>().GetHp();
            _maxHp = _target.GetComponent<EnemyScript>().GetMaxHp();
        }
        else
        {
            _hp = _target.GetComponent<HeroScript>().GetHp();
            _maxHp = _target.GetComponent<HeroScript>().GetMaxHp();
        }

        gameObject.GetComponent<TextMeshProUGUI>().text = _hp + " / " + _maxHp;
    }

    public void SetHpColor()
    {
        if(_hp >= (float) 7 / 10 * _maxHp)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(0, 255, 0, 255);
            //color is green
        }
        if(_hp < (float) 7 / 10 * _maxHp && _hp >= (float) 3 / 10 * _maxHp)
        {
            //color is yellow
            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 0, 255);
        }
        if(_hp < (float) 3 / 10 * _maxHp)
        {
            //color is red
            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0, 255);
        }
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }
}
