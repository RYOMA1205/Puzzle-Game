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
