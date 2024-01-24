using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepObjectsScript : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
