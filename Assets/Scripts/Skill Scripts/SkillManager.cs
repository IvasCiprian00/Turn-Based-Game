using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public GameManager gmManager;

    public UIManager uiManager;

    public void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void UseSkill(string skillName)
    {
        switch(skillName)
        {
            case "Global Heal(Clone)":
                GlobalHeal();
                break;
            case "Great Strike(Clone)":
                GreatStrike();
                break;
            default:
                break;
        }
    }

    public void GlobalHeal()
    {
        for(int i = 0; i < gmManager.heroes.Length; i++)
        {
            HeroScript hsScript = gmManager.heroes[i].GetComponent<HeroScript>();

            if (hsScript != null)
            {
                hsScript.Heal(1);
            }
        }
    }

    public void GreatStrike()
    {

    }
}
