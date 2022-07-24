using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Text txtTimer;

    /// <summary>
    /// ��ʕ\���X�R�A�̍X�V����
    /// </summary>
    public void UpdateDisplayScore()
    {
        // ��ʂɕ\�����Ă���X�R�A�̒l���X�V
        txtScore.text = GameData.instance.score.ToString();
    }

    /// <summary>
    /// �Q�[���̎c�莞�Ԃ̕\���X�V
    /// </summary>
    /// <param name="times"></param>
    public void UpdateDisplayGameTime(float time)
    {
        txtTimer.text = time.ToString("F0");
    }
}
