using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eto : MonoBehaviour
{
    [Header("���x�̎��")]
    public EtoType etoType;

    [Header("���x�̃C���[�W�ύX�p")]
    public Image imgEto;

    [Header("�X���C�v���ꂽ���x�ł��锻��Btrue�̏ꍇ�A���̊��x�͍폜�ΏۂƂȂ�")]
    public bool isSelected;

    [Header("�X���C�v���ꂽ�ʂ��ԍ��B�X���C�v���ꂽ���Ԃ���������")]
    public int num;

    /// <summary>
    /// ���x�̏����ݒ�
    /// </summary>
    /// <param name="etoType"></param>
    /// <param name="sprite"></param>
    public void SetUpEto(EtoType etoType, Sprite sprite)
    {
        // ���x�̎�ނ�ݒ�
        this.etoType = etoType;

        // ���x�̖��O���A�ݒ肵�����x�̎�ނ̖��O�ɕύX
        name = this.etoType.ToString();

        // �����œ͂������x�̃C���[�W�ɍ��킹�ăC���[�W��ύX
        ChangeEtoImage(sprite);
    }

    /// <summary>
    /// ���x�̃C���[�W��ύX
    /// </summary>
    /// <param name="changeSprite"></param>
    public void ChangeEtoImage(Sprite changeSprite)
    {
        imgEto.sprite = changeSprite;
    }
}
