using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelector : MonoBehaviour
{
    public UIManager uiManager;

    public void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    public void OnMouseUp()
    {
        uiManager.DisplayCofirmButtons(true);

        uiManager.SetSkill(gameObject.name);

        switch(gameObject.name)
        {
            case "Global Heal":

                break;
            default:
                break;
        }
    }
}
