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

    // 11�ł�������ǉ�
    // �ŏ��Ƀh���b�O�������x�̏��
    private Eto firstSelectEto;

    // �Ō�Ƀh���b�O�������x�̏��
    private Eto lastSelectEto;

    // �ŏ��Ƀh���b�O�������x�̎��
    private EtoType? currentEtoType;

    [SerializeField, Header("�폜�ΏۂƂȂ銱�x��o�^���郊�X�g")]
    private List<Eto> eraseEtoList = new List<Eto>();

    [SerializeField, Header("�Ȃ����Ă��銱�x�̐�")]
    private int linkCount = 0;

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

    // 11�ł�������ǉ�
    void Update()
    {
        // ���x���Ȃ��鏈��
        if (Input.GetMouseButtonDown(0) && firstSelectEto == null)
        {
            // ���x���ŏ��Ƀh���b�O�����ۂ̏���
            OnStartDrag();
        }
    }

    /// <summary>
    /// ���x���ŏ��Ƀh���b�O�����ۂ̏���
    /// </summary>
    private void OnStartDrag()
    {
        // ��ʂ��^�b�v�����ۂ̈ʒu�����ACamera�N���X��ScreenToWorldPoint���\�b�h�𗘗p����Canvas��̍��W�ɕϊ�
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // ���x���Ȃ����Ă��鐔��������
        linkCount = 0;

        // �ϊ��������W�̃R���C�_�[�����Q�[���I�u�W�F�N�g�����邩�m�F
        if (hit.collider != null)
        {
            // �Q�[���I�u�W�F�N�g���������ꍇ�A���̃Q�[���I�u�W�F�N�g��Eto�N���X�������Ă��邩�ǂ����m�F
            if (hit.collider.gameObject.TryGetComponent(out Eto dragEto))
            {
                // Eto�N���X�������Ă����ꍇ�ɂ́A�ȉ��̏������s��

                // �ŏ��Ƀh���b�O�������x�̏���ϐ��ɑ��
                firstSelectEto = dragEto;

                // �Ō�Ƀh���b�O�������x�̏���ϐ��ɑ��(�ŏ��̃h���b�O�Ȃ̂ŁA�Ō�̃h���b�O���������x)
                lastSelectEto = dragEto;

                // �ŏ��Ƀh���b�O�������x�̏����� �� ��قǁA���̏����g���ĂȂ��銱�x���ǂ����𔻕ʂ���
                currentEtoType = dragEto.etoType;

                // ���x�̏�Ԃ��u�I�𒆁v�ł���ƍX�V
                dragEto.isSelected = true;

                // ���x�ɉ��ԖڂɑI������Ă���̂��A�ʂ��ԍ���o�^
                dragEto.num = linkCount;

                // �폜����Ώۂ̊��x��o�^���郊�X�g��������
                eraseEtoList = new List<Eto>();

                // �h���b�O���̊��x���폜�̑ΏۂƂ��ă��X�g�ɓo�^
                AddEraseEtoList(dragEto);
            }
        }
    }

    /// <summary>
    /// �I�����ꂽ���x���폜���X�g�ɒǉ�
    /// </summary>
    /// <param name="dragEto"></param>
    private void AddEraseEtoList(Eto dragEto)
    {
        // �폜���X�g�Ƀh���b�O���̊��x��ǉ�
        eraseEtoList.Add(dragEto);

        // �h���b�O���̊��x�̃A���t�@�l��0.5f�ɂ���(�������ɂ��邱�ƂŁA�I�𒆂ł��邱�Ƃ����[�U�[�ɓ`����)
        ChangeEtoAlpha(dragEto, 0.5f);
    }

    /// <summary>
    /// ���x�̃A���t�@�l��ύX
    /// </summary>
    /// <param name="dragEto"></param>
    /// <param name="alphaValue"></param>
    private void ChangeEtoAlpha(Eto dragEto, float alphaValue)
    {
        // ���݃h���b�O���Ă��銱�x�̃A���t�@�l��ύX
        dragEto.imgEto.color = new Color(dragEto.imgEto.color.r, dragEto.imgEto.color.g, dragEto.imgEto.color.b, alphaValue);
    }
}
