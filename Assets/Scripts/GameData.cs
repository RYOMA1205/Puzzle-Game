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

    [Header("Œ»İ‚ÌƒXƒRƒA")]
    public int score = 0;

    [Header("Š±x‚ğÁ‚µ‚½Û‚É‰ÁZ‚³‚ê‚éƒXƒRƒA")]
    public int etoPoint = 100;

    [Header("Á‚µ‚½Š±x‚Ì”")]
    public int eraseEtoCount = 0;

    [SerializeField, Header("1‰ñ•Ó‚è‚ÌƒQ[ƒ€ŠÔ")]
    private int initTime = 60;

    [Header("Œ»İ‚ÌƒQ[ƒ€‚Ìc‚èŠÔ")]
    public float gameTime;

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

        // ƒQ[ƒ€‚Ì‰Šú‰»
        InitGame();
    }

    // l‚¦—p
    //private void Update()
    //{
        //gameTime -= Time.deltaTime;

        //initTime = (int)gameTime;
    //}

    /// <summary>
    /// ƒQ[ƒ€‰Šú‰»
    /// </summary>
    private void InitGame()
    {
        score = 0;
        eraseEtoCount = 0;

        // ƒQ[ƒ€ŠÔ‚ğİ’è
        gameTime = initTime;

        Debug.Log("Init Game");
    }
}
