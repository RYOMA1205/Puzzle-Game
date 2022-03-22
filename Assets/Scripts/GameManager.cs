using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 10�ŕύX
    // �錾����^�� GameObject�^ ���� Eto�^�ɕύX
    // EtoPrefab�͑O�Ɠ����悤��Project������A�T�C���ł���
    [Header("���x�̃v���t�@�u")]
    public Eto etoPrefab;

    [SerializeField, Header("���x�̐����ʒu")]
    private Transform etoSetTran;

    [SerializeField, Header("���x�������̍ő��]�p�x")]
    private float maxRotateAngle = 35.0f;

    //[SerializeField, Header("���x�������̃����_����")]
    //private float maxRange = 400.0f;

    //[SerializeField, Header("���x�������̗����ʒu")]
    //private float fallPos = 1400.0f;

    // 10�ł�������ǉ�
    [SerializeField, Header("�������ꂽ���x�̃��X�g")]
    private List<Eto> etoList = new List<Eto>();

    [SerializeField, Header("���x�̉摜�f�[�^")]
    private Sprite[] etoSprites;

    // �߂�l��void����IEnumerator�^�ɕύX���āA�R���[�`�����\�b�h�ɂ���
    IEnumerator Start()
    {
        // ���x�̉摜��ǂ݂��ށB
        // ���̏������I������܂ŁA���̏����ւ͂����Ȃ��悤�ɂ���
        yield return StartCoroutine(LoadEtoSprites());

        // �����Ŏw�肵�����̊��x�𐶐�����
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
    }

    // 10�ł�������ǉ�

    /// <summary>
    /// ���x�̉摜��ǂݍ���Ŕz�񂩂�g�p�ł���悤�ɂ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadEtoSprites()
    {
        // �z��̏�����(12�̉摜������悤��Sprite�^�̔z���12�p�ӂ���)
        etoSprites = new Sprite[(int)EtoType.Count];

        // Resources.LoadAll���s���A��������Ă��銱�x�̉摜�����ԂɑS�ēǂݍ���Ŕz��ɑ��
        etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

        // ���@1�̃t�@�C����12�������Ă��Ȃ��ꍇ�ɂ͈ȉ��̏������g��
        //     12�������Ă���ꍇ�ɂ͎g�p���Ȃ�
        //for(int i = 0; i < etoSprites.Length; i++)
        //{
        //etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
        //}

        yield break;
    }

    /// <summary>
    /// ���x�𐶐�
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private IEnumerator CreateEtos(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // ���x�v���t�@�u�̃N���[�����A���x�̐����ʒu�ɐ���
            Eto eto = Instantiate(etoPrefab, etoSetTran, false);

            // �������ꂽ���x�̉�]����ݒ�(�F�X�Ȋp�x�ɂȂ�悤��)
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-maxRotateAngle, maxRotateAngle), Vector3.forward);

            // �����ʒu�������_���ɂ��ė����ʒu��ω�������
            eto.transform.localPosition = new Vector2(Random.Range(-400.0f, 400.0f), 1400f);

            // 10�ł�������ǉ�
            // �����_���Ȋ��x��12��ނ̒�����1�I��
            int randomValue = Random.Range(0, (int)EtoType.Count);

            // �������ꂽ���x�̏����ݒ�(���x�̎�ނƊ��x�̉摜���������g����Eto�֓n��)
            eto.SetUpEto((EtoType)randomValue, etoSprites[randomValue]);

            // etoList�ɒǉ�
            etoList.Add(eto);

            // 0.03�b�҂��Ď��̊��x�𐶐�
            yield return new WaitForSeconds(0.03f);
        }
    }
}
