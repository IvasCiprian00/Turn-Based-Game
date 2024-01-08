using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalHealSkill : MonoBehaviour
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
    }
}
