using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class SkillSelector : MonoBehaviour
{
    public UIManager uiManager;
    public GameManager gmManager;
    public SkillManager skManager;

    [SerializeField] private int damage;

    public void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        skManager = GameObject.Find("Skill Manager").GetComponent<SkillManager>();
    }

    public void OnMouseUp()
    {
        uiManager.ToggleMoveTiles(false);

        uiManager.SetSkill(gameObject.name);

        switch(gameObject.name)
        {
            case "Global Heal(Clone)":
                uiManager.DisplayCofirmButtons(true);
                break;
            case "Great Strike(Clone)":
                skManager.SpawnGreatStrikeTiles();
                skManager.SetSkillDamage(damage);
                break;
            case "Meteor(Clone)":
                skManager.SpawnMeteorTiles();
                skManager.SetSkillDamage(damage);
                break;
            default:
                break;
        }
    }
}
