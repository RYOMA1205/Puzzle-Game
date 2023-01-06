using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EtoButton : MonoBehaviour
{
    [Header("���̃{�^���̊��x�f�[�^")]
    public GameData.EtoData etoData;

    // �摜�ݒ�p
    public Image imgEto;

    // �{�^������p
    public Button btnEto;

    private EtoSelectPopUp etoSelectPopUp;

    [SerializeField]
    private CanvasGroup canvasGroup;

    /// <summary>
    /// ���x�{�^���̏����ݒ�
    /// </summary>
    /// <param name="etoSelectPopUp"></param>
    /// <param name="etoData"></param>
    public void SetUpEtoButton(EtoSelectPopUp etoSelectPopUp, GameData.EtoData etoData)
    {
        // ���x�{�^���S�̂𓧖��ɂ���
        canvasGroup.alpha = 0.0f;

        // �����̒l����
        this.etoSelectPopUp = etoSelectPopUp;
        this.etoData = etoData;

        // ���x�{�^���̉摜�����x�f�[�^�̉摜�ɕύX(���̏����ɂ����Image���e���x�̉摜�ɍ����ւ���)
        imgEto.sprite = this.etoData.sprite;

        // �{�^���Ƀ��\�b�h��o�^
        btnEto.onClick.AddListener(() => StartCoroutine(OnClickEtoButton()));

        // �A�j�������Ȃ���{�^�������X�ɕ\������
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.Linear));
        sequence.Join(transform.DOPunchScale(new Vector3(1, 1, 1), 0.5f));
    }

    /// <summary>
    /// ���x�{�^�����^�b�v�����ۂ̏���
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnClickEtoButton()
    {
        // ���W13�Œǉ�
        SoundManager.instance.PlaySE(SoundManager.SE_Type.OK);

        // ���x�{�^���̕ێ����銱�x�f�[�^��GameData�ɑ��(�I���������x�f�[�^�Ƃ���)
        GameData.instance.selectedEtoData = etoData;

        // ���x�{�^�����|�b�v�A�j��������
        transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.15f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.15f);
        transform.DOScale(Vector3.one, 0.15f);

        // ���x�{�^���̐F��I�𒆂̐F�ɕύX���A���̊��x�{�^���̐F��I�𒆂łȂ��F�ɕύX
        etoSelectPopUp.ChangeColorToEtoButton(etoData.etoType);
    }

}
