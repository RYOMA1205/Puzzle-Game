using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [Header("�Q�[���ɓo�ꂷ�銱�x�̍ő��ސ�")]
    public int etoTypeCount = 5;

    [Header("�Q�[���J�n���ɐ������銱�x�̐�")]
    public int createEtoCount = 50;

    [Header("���݂̃X�R�A")]
    public int score = 0;

    [Header("���x���������ۂɉ��Z�����X�R�A")]
    public int etoPoint = 100;

    [Header("���������x�̐�")]
    public int eraseEtoCount = 0;

    [SerializeField, Header("1��ӂ�̃Q�[������")]
    private int initTime = 60;

    [Header("���݂̃Q�[���̎c�莞��")]
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

        // �Q�[���̏�����
        InitGame();
    }

    // �l���p
    //private void Update()
    //{
        //gameTime -= Time.deltaTime;

        //initTime = (int)gameTime;
    //}

    /// <summary>
    /// �Q�[��������
    /// </summary>
    private void InitGame()
    {
        score = 0;
        eraseEtoCount = 0;

        // �Q�[�����Ԃ�ݒ�
        gameTime = initTime;

        Debug.Log("Init Game");
    }
}
