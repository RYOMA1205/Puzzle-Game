using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultPopUp : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Text txtEraseEtoCount;

    [SerializeField]
    private Button btnClosePopUp;

    void Start()
    {
        // btnClosePopUp�Q�[���I�u�W�F�N�g�̎���CanvasGruop��Alpha�� 0 �ɐݒ肵�ē����ɂ��Ă���(�ŏ��̓^�b�v�ł����A�������Ȃ��悤�ɂ��Ă���)
        btnClosePopUp.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    public void DisplayResult(int score, int eraseEtoCount)
    {
        // �v�Z�p�̏����l��ݒ�
        int initValue = 0;

        // DOTween��Seapuence(�V�[�P���X)�@�\�����������Ďg�p�ł���悤�ɂ���
        Sequence sequence = DOTween.Sequence();

        // �V�[�P���X�𗘗p���āADOTween�̏����𐧌䂵�������ԂŋL�q����B�܂��͇@�X�R�A�̐������A�j�����ĕ\��
        sequence.Append(DOTween.To(() => initValue,
            (num) =>
            {
                initValue = num;
                txtScore.text = num.ToString();
            },
            score,
            1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));  // ���̏����̂��߂�OnComplete���g����initValue��������

        // �A�V�[�P���X������0.5�b�����ҋ@
        sequence.AppendInterval(0.5f);

        // �B���������x�̐������A�j�����ĕ\��
        sequence.Append(DOTween.To(() => initValue,
            (num) =>
            {
                initValue = num;
                txtEraseEtoCount.text = num.ToString();
            },
            eraseEtoCount,
            1.0f).SetEase(Ease.InCirc));

        // �C�V�[�P���X������0.5�b�����ҋ@
        sequence.AppendInterval(1.0f);

        // �D�����ɂȂ��Ă���btnClosePopUp�Ƃ��̎q�v�f��CanvasGroup��Alpha���g�p���ď��X�ɕ\��
        sequence.Append(btnClosePopUp.gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
    }

    
}
