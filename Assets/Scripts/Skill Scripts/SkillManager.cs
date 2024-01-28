using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillManager : MonoBehaviour
{
    public GameManager gmManager;
    public UIManager uiManager;
    public HeroManager heroManager;

    public GameObject selectTile;

    private int _skillDamage;

    public void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        gmManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        heroManager = GameObject.Find("Hero Manager").GetComponent<HeroManager>();
    }

    public void UseSkill(string skillName)
    {
        switch(skillName)
        {
            case "Global Heal(Clone)":
                GlobalHeal();
                break;
            case "Great Strike(Clone)":
                GreatStrike(_skillDamage);
                break;
            default:
                break;
        }
    }

    public void CancelSkill(string skillName)
    {
        switch (skillName)
        {
            case "Global Heal(Clone)":
                break;
            case "Great Strike(Clone)":
                DestroySelectTiles();
                break;
            default:
                break;
        }
    }

    public void DestroySelectTiles()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Select Tile");

        for(int i = 0; i < tiles.Length; i++) 
        {
            Destroy(tiles[i]);
        }
    }

    public void GlobalHeal()
    {
        for(int i = 0; i < heroManager.GetHeroCount(); i++)
        {
            HeroScript hsScript = heroManager.heroesAlive[i].GetComponent<HeroScript>();

            if (hsScript != null)
            {
                hsScript.Heal(1);
            }
        }
    }

    public void GreatStrike(int damage)
    {
        GameObject[] remainingTiles= GameObject.FindGameObjectsWithTag("Select Tile");

        for(int i = 0; i < remainingTiles.Length; i++)
        {
            SelectTileScript stScript= remainingTiles[i].GetComponent<SelectTileScript>();

            if(gmManager.gameBoard[stScript.GetXPos(), stScript.GetYPos()] != null)
            {
                if (gmManager.gameBoard[stScript.GetXPos(), stScript.GetYPos()].tag == "Enemy")
                {
                    gmManager.gameBoard[stScript.GetXPos(), stScript.GetYPos()].GetComponent<EnemyScript>().TakeDamage(damage);
                }
            }

            Destroy(remainingTiles[i]);
        }
    }

    public void SpawnGreatStrikeTiles()
    {
        HeroScript hsScript = heroManager.heroesAlive[gmManager.currentHero].GetComponent<HeroScript>();

        int xPos = hsScript.GetXPos();
        int yPos = hsScript.GetYPos();

        int[] lineArray = { -1, 0, 1, 0 };
        int[] colArray = { 0, 1, 0, -1 };

        for(int i = 0; i < 4; i++)
        {
            int tileXPos = xPos + lineArray[i];
            int tileYPos = yPos + colArray[i];


            if (tileXPos >= 0 && tileXPos < gmManager.GetNumberOfLines() && tileYPos >= 0 && tileYPos < gmManager.GetNumberOfColumns())
            {
                GameObject tilePosition = gmManager.tiles[tileXPos, tileYPos];
                GameObject reference = Instantiate(selectTile, tilePosition.transform.position, Quaternion.identity);

                reference.transform.position = new Vector3(reference.transform.position.x, reference.transform.position.y, -3);
                reference.GetComponent<SelectTileScript>().SetCoords(tileXPos, tileYPos);
            }
        }
    }

    public void SetSkillDamage(int damage)
    {
        _skillDamage = damage;
    }
}
