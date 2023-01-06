using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EtoSelectPopUp : MonoBehaviour
{
    [SerializeField, Header("���x�{�^���̃v���t�@�u")]
    private EtoButton etoButtonPrefab;

    [SerializeField, Header("���x�{�^���̐����ʒu")]
    private Transform etoButtonTran;

    // �X�^�[�g�{�^���̐���p
    public Button btnStart;

    // CanvasGroup�̐���p
    public CanvasGroup canvasGroup;

    // �����������x�{�^�����Ǘ����郊�X�g
    private List<EtoButton> etoButtonList = new List<EtoButton>();

    // GameManager�N���X�ւ̕R�Â�
    private GameManager gameManager;

    /// <summary>
    /// ���x�{�^���̐���
    /// </summary>
    /// <param name="gameManager"></param>
    /// <returns></returns>
    public IEnumerator CreateEtoButtons(GameManager gameManager)
    {
        // ���x�̃{�^���������ł���܂ŃX�^�[�g�͉����Ȃ�
        btnStart.interactable = false;

        this.gameManager = gameManager;

        // ���x�f�[�^�����ɑI���{�^�����쐬
        for (int i = 0; i < (int)EtoType.Count; i++)
        {
            // ���x�{�^�������Ɋ��x�̑I���{�^�����쐬
            EtoButton etoButton = Instantiate(etoButtonPrefab, etoButtonTran, false);

            // ���x�{�^���̏����ݒ�
            etoButton.SetUpEtoButton(this, GameData.instance.etoDataList[i]);

            if (i == 0)
            {
                // �����͊��x�̎q(��)��I�����Ă����Ԃɂ���
                etoButton.imgEto.color = new Color(0.65f, 0.65f, 0.65f);
                GameData.instance.selectedEtoData = GameData.instance.etoDataList[i];
            }

            // ���x�{�^����List�֒ǉ�
            etoButtonList.Add(etoButton);

            yield return new WaitForSeconds(0.15f);
        }

        // Start�{�^���ւ̃��\�b�h�̓o�^
        btnStart.onClick.AddListener(OnClickStart);

        // ���x�̃{�^���̏������ł����̂ŃX�^�[�g��������悤�ɂ���
        btnStart.interactable = true;

        Debug.Log("Init Eto Buttons");

        yield break;
    }

    /// <summary>
    /// �X�^�[�g�{�^�����������ۂ̏���
    /// </summary>
    private void OnClickStart()
    {
        // ���W13�Œǉ�
        SoundManager.instance.PlaySE(SoundManager.SE_Type.OK);

        // �X�^�[�g�{�^���������Ȃ��悤�ɂ��ďd���^�b�v��h�~
        btnStart.interactable = false;

        // �Q�[���̏����J�n
        StartCoroutine(gameManager.PreparateGame());

        // ���X�Ɋ��x�I���̃|�b�v�A�b�v�������Ȃ��悤�ɂ��Ă����\���ɂ���
        canvasGroup.DOFade(0.0f, 0.5f);
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// EtoButton����Ă΂��
    /// �I�����ꂽ���x�{�^���̐F��I�𒆂̐F(�D�F)�ɕύX
    /// ���̃{�^���͑I�𒆂ł͂Ȃ��F(�ʏ�̐F)�ɕύX
    /// </summary>
    /// <param name="etoType"></param>
    public void ChangeColorToEtoButton(EtoType etoType)
    {
        for (int i =0; i < etoButtonList.Count; i++)
        {
            // ���x�{�^���̐F��I�𒆂��A�I�𒆂łȂ����ŕύX
            if (etoButtonList[i].etoData.etoType == etoType)
            {
                // �I�𒆂̐F�ɕύX(�D�F)
                etoButtonList[i].imgEto.color = new Color(0.65f, 0.65f, 0.65f);
            }
            else
            {
                // �ʏ�̐F�ɕύX
                etoButtonList[i].imgEto.color = new Color(1f, 1f, 1f);
            }
        }
    }
}
