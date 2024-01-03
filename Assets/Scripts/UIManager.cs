using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gmManager;

    [SerializeField] private GameObject _endTurnButton;
    [Header("Hero Stats")]
    [SerializeField] private GameObject _statsContainer;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _dmgText;

    public void Update()
    {
        if (gmManager.IsHeroTurn())
        {
            DisplayHeroStats();
        }
        else
        {
            HideHeroStats();
        }
    }

    public void DisplayHeroStats()
    {
        if (!_statsContainer.activeSelf)
        {
            _statsContainer.SetActive(true);
            _endTurnButton.SetActive(true);
        }

        _hpText.text = gmManager.hsScript.GetHp().ToString() + " HP";
        _dmgText.text = gmManager.hsScript.GetDamage().ToString() + " DMG";
    }

    public void HideHeroStats()
    {
        if(_statsContainer.activeSelf)
        {
            _statsContainer.SetActive(false);
            _endTurnButton.SetActive(false);
        }
    }
}
