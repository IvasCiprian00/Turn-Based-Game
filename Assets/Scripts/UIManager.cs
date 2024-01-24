using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gmManager;

    [SerializeField] private GameObject _nextLevelButton;
    [SerializeField] private GameObject _endTurnButton;
    [SerializeField] private GameObject[] _activeTiles;

    [Header("Skills")]
    [SerializeField] private SkillManager _skillManager;
    [SerializeField] private GameObject _confirmSkill;
    [SerializeField] private GameObject _cancelSkill;
    [SerializeField] private string _selectedSkill;
    [SerializeField] private GameObject[] _skillSlots;
    [SerializeField] private GameObject[] _skillReference;


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

    public void DisplayNextLevelButton()
    {
        _nextLevelButton.SetActive(true);
    }

    public void DisplayHeroStats()
    {
        if (!_statsContainer.activeSelf)
        {
            _statsContainer.SetActive(true);
            _endTurnButton.SetActive(true);
        }

        _hpText.text = gmManager.hsScript.GetHp().ToString();
        _dmgText.text = gmManager.hsScript.GetDamage().ToString();
    }

    public void HideHeroStats()
    {
        if(_statsContainer.activeSelf)
        {
            _statsContainer.SetActive(false);
            _endTurnButton.SetActive(false);
        }
    }

    public void DisplaySkills(GameObject[] skills)
    {
        int currentSkillSlot = 0;

        _skillReference = new GameObject[skills.Length];

        for(int i = 0; i < skills.Length; i++)
        {
            if (skills[i] == null)
            {
                continue;
            }

            _skillReference[i] = Instantiate(skills[i], _skillSlots[currentSkillSlot].transform.position, Quaternion.identity);
            _skillReference[i].transform.position = new Vector3(_skillReference[i].transform.position.x, _skillReference[i].transform.position.y, -2);
            currentSkillSlot++;
        }
    }

    public void HideSkills()
    {
        for(int i = 0; i < _skillReference.Length; i++)
        {
            if(_skillReference[i] == null)
            {
                continue;
            }

            Destroy(_skillReference[i]);
        }
    }

    public void DisplayCofirmButtons(bool isActive)
    {
        _confirmSkill.SetActive(isActive);
        _cancelSkill.SetActive(isActive);
    }

    public void SetSkill(string skillName)
    {
        _selectedSkill = skillName;
    }

    public void ConfirmSkill()
    {
        _skillManager.UseSkill(_selectedSkill);
        ToggleMoveTiles(true);
        DisplayCofirmButtons(false);
    }

    public void CancelSkill()
    {
        _skillManager.CancelSkill(_selectedSkill);
        _selectedSkill = null;
        ToggleMoveTiles(true);
        DisplayCofirmButtons(false);
    }

    public void ToggleMoveTiles(bool isActive) {
        if(!isActive)
        {
           _activeTiles = GameObject.FindGameObjectsWithTag("Move Tile");
        }

        for (int i = 0; i < _activeTiles.Length; i++)
        {
            if (_activeTiles[i] == null) 
            { 
                continue; 
            }

            _activeTiles[i].SetActive(isActive);
        }
    }
}
