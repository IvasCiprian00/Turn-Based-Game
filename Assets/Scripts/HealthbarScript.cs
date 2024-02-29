using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour
{
    private int _hp;
    private int _maxHp;
    private GameObject _target;

    public Slider slider;
    public Image fill;

    public float redColor;
    public float greenColor;

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

        slider.value = (float) _hp / _maxHp;
    }

    public void SetHpColor()
    {
        float hpPercent = (float)_hp / _maxHp;

        if (hpPercent <= 0.5)
        {
            redColor = 1 - hpPercent + 0.3f;
            greenColor = 1 * hpPercent;
        }
        else
        {
            redColor = 1 - hpPercent + 0.3f;
            greenColor = 1 * hpPercent + 0.1f;
        }
        fill.color = new Color(redColor, greenColor, 0, 255);
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }
}
