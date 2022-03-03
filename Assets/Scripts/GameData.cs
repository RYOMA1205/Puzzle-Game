using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [Header("ƒQ[ƒ€‚É“oê‚·‚éŠ±x‚ÌÅ‘åí—Ş”")]
    public int etoTypeCount = 5;

    [Header("ƒQ[ƒ€ŠJn‚É¶¬‚·‚éŠ±x‚Ì”")]
    public int createEtoCount = 50;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
