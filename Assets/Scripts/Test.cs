using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("Game Manager") != null)
        {
            Debug.Log("YEY");

            StartCoroutine(UnloadScene());
        }
    }

    IEnumerator UnloadScene()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.UnloadSceneAsync("Level_2");
    }
}
