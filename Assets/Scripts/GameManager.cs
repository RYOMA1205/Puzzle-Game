using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;      // DOTween �𗘗p���邽�߂ɕK�v�ɂȂ邽�߁A�ǉ����܂�

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

    [Header("�X���C�v�łȂ��銱�x�͈̔�")]
    public float etoDistance = 1.0f;

    [SerializeField]
    private UIManager uiManager;

    // �c�莞�Ԍv���p
    private float timer;

    [SerializeField]
    private GameObject resultPopUp;

    /// <summary>
    /// �Q�[���̐i�s��
    /// </summary>
    public enum GameState
    {
        Select,   // ���x�̑I��
        Ready,    // �Q�[���̏�����
        Play,     // �Q�[���̃v���C��
        Result    // ���U���g��
    }

    [Header("���݂̃Q�[���̐i�s��")]
    public GameState gameState = GameState.Select;

    // �߂�l��void����IEnumerator�^�ɕύX���āA�R���[�`�����\�b�h�ɂ���
    IEnumerator Start()
    {
        // gameState���������ɕύX
        gameState = GameState.Ready;

        // ���x�̉摜��ǂ݂��ށB
        // ���̏������I������܂ŁA���̏����ւ͂����Ȃ��悤�ɂ���
        yield return StartCoroutine(LoadEtoSprites());

        // �c�莞�Ԃ̕\��
        uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);

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

        // gameState���������̂Ƃ������Q�[���v���C���ɕύX
        if (gameState == GameState.Ready)
        {
            gameState = GameState.Play;
        }
    }

    // 11�ł�������ǉ�
    void Update()
    {
        // �Q�[���̃v���C���ȊO��gameState�ł͏������s��Ȃ�
        if (gameState != GameState.Play)
        {
            return;
        }

        // ���x���Ȃ��鏈��
        if (Input.GetMouseButtonDown(0) && firstSelectEto == null)
        {
            // ���x���ŏ��Ƀh���b�O�����ۂ̏���
            OnStartDrag();
        }
        // 13�Œǉ�
        else if (Input.GetMouseButtonUp(0))
        {
            // ���x�̃h���b�O����߂��i�w�𗣂����j�ۂ̏���
            OnEndDrag();
        }
        // 12�Œǉ�
        else if (firstSelectEto != null)
        {
            // ���x�̃h���b�O�i�X���C�v�j���̏���
            OnDragging();
        }

        // �Q�[���̎c�莞�Ԃ̃J�E���g����
        timer += Time.deltaTime;

        // timer���P�ȏ�ɂȂ�����
        if (timer >= 1)
        {
            // ���Z�b�g���čēx���Z�ł���悤��
            timer = 0;

            // �c�莞�Ԃ��}�C�i�X
            GameData.instance.gameTime--;

            //  �c�莞�Ԃ��}�C�i�X�ɂȂ�����
            if (GameData.instance.gameTime <= 0)
            {
                // 0�Ŏ~�߂�
                GameData.instance.gameTime = 0;

                // TODO  �Q�[���I����ǉ�����
                //Debug.Log("�Q�[���I��");

                // �Q�[���I����ǉ�����
                StartCoroutine(GameUp());
            }

            // �c�莞�Ԃ̕\���X�V
            uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);
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
    /// ���x�̃h���b�O�i�X���C�v�j������
    /// </summary>
    private void OnDragging()
    {
        // OnStartDrag���\�b�h�Ɠ��������ŁA�w�̈ʒu�����[���h���W�ɕϊ���Ray�𔭎˂����̈ʒu�ɂ���R���C�_�[�����I�u�W�F�N�g���擾����hit�ϐ��֑��
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // Ray�̖߂�l������(hit�ϐ���null�ł͂Ȃ�)hit�ϐ��̃Q�[���I�u�W�F�N�g��Eto�N���X�������Ă�����
        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Eto dragEto))
        {
            // ���ݑI�𒆂̊��x�̎�ނ�null�Ȃ珈���͍s��Ȃ�
            if (currentEtoType == null)
            {
                return;
            }

            // dragEto�ϐ��̊��x�̎�ނ��ŏ��ɑI���������x�̎�ނƓ����ł���A�Ō�Ƀ^�b�v���Ă��銱�x�ƌ��݂̊��x���Ⴄ�I�u�W�F�N�g�ł���A���A���݂̊��x�����łɁu�I�𒆁v�łȂ����
            if (dragEto.etoType == currentEtoType && lastSelectEto != dragEto && !dragEto.isSelected)
            {
                // ���݃^�b�v���Ă��銱�x�̈ʒu���ƍŌ�Ƀ^�b�v�������x�̈ʒu���Ɣ�ׂāA�����̒l(���x�ʂ��̋���)�����
                float distance = Vector2.Distance(dragEto.transform.position, lastSelectEto.transform.position);

                // ���x���m�̋������ݒ�l�������������(2�̊��x������Ă��Ȃ����)�A���x���Ȃ���
                if (distance < etoDistance)
                {
                    // ���݂̊��x��I�𒆂ɂ���
                    dragEto.isSelected = true;

                    // �Ō�ɑI�����Ă��銱�x�����݂̊��x�ɍX�V
                    lastSelectEto = dragEto;

                    // ���x�̂Ȃ��������̃J�E���g��1���₷
                    linkCount++;

                    // ���x�ɒʂ��ԍ���ݒ�
                    dragEto.num = linkCount;

                    // �폜���X�g�Ɍ��݂̊��x��ǉ�
                    AddEraseEtoList(dragEto);
                }
            }

            // ���݂̊��x�̎�ނ��m�F(���݂̊��x(dragEto�̏��ł���Α��̏��ł��悢�B�����ƑI������Ă��邩�̊m�F�p))
            Debug.Log(dragEto.etoType);

            // �폜���X�g��2�ȏ�̊��x���ǉ�����Ă���ꍇ
            if (eraseEtoList.Count > 1)
            {
                // ���݂̊��x�̒ʂ��ԍ����m�F
                Debug.Log(dragEto.num);

                // �����ɍ��v����ꍇ�A�폜���X�g���犱�x�����O����(�h���b�O�����܂�1�O�̊��x�̖߂�ꍇ�A���݂̊��x���폜���X�g���珜�O����)
            �@�@if (eraseEtoList[linkCount - 1] != lastSelectEto && eraseEtoList[linkCount - 1].num == dragEto.num && dragEto.isSelected)
                {
                    // �I�𒆂̃{�[������菜��
                    RemoveEraseEtoList(lastSelectEto);

                    lastSelectEto.GetComponent<Eto>().isSelected = false;

                    // �Ō�̃{�[���̏���O�̃{�[���ɖ߂�
                    lastSelectEto = dragEto;

                    // �Ȃ����Ă��銱�x�̐������炷
                    linkCount--;
                }
            }
        }
    }

    /// <summary>
    /// ���x�̃h���b�O����߂��i�w����ʂ��痣�����j�ۂ̏���
    /// </summary>
    private void OnEndDrag()
    {
        // �Ȃ����Ă��銱�x��3�ȏ゠������폜���鏈���ɂ���
        if (eraseEtoList.Count >= 3)
        {
            // �I������Ă��銱�x������
            for (int i = 0; i < eraseEtoList.Count; i++)
            {
                // ���x���X�g�����菜��
                etoList.Remove(eraseEtoList[i]);

                // ���x���폜
                Destroy(eraseEtoList[i].gameObject);
            }

            // �X�R�A�Ə��������x�̐��̉��Z
            AddScores(currentEtoType, eraseEtoList.Count);

            // ���������x�̐������V�������x�������_���ɐ���
            StartCoroutine(CreateEtos(eraseEtoList.Count));

            // �폜���X�g���N���A����
            eraseEtoList.Clear();
        }
        else
        {
            // �Ȃ����Ă��銱�x��2�ȉ��Ȃ�A�폜�͂��Ȃ�
            
            // �폜���X�g����A�폜���ł��������x����菜��
            for (int i = 0; i < eraseEtoList.Count; i++)
            {
                // �e���x�̑I�𒆂̏�Ԃ���������
                eraseEtoList[i].isSelected = false;

                // ���x�̐F�̓����x�����̓����x�ɖ߂�
            }
        }

        // ����̊��x�����������̂��߂ɁA�e�ϐ��̒l�� null �ɂ���
        firstSelectEto = null;
        lastSelectEto = null;
        currentEtoType = null;
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

    private void RemoveEraseEtoList(Eto dragEto)
    {
        // �폜���X�g����폜
        eraseEtoList.Remove(dragEto);

        // ���x�̓����x�����̒l(1.0f)�ɖ߂�
        ChangeEtoAlpha(dragEto, 1.0f);

        // ���x�́u�I�𒆁v�̏��true�̏ꍇ
        if (dragEto.isSelected)
        {
            // false�ɂ��đI�𒆂ł͂Ȃ���Ԃɖ߂�
            dragEto.isSelected = false;
        }
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

    /// <summary>
    /// �X�R�A�Ə��������x�̐������Z
    /// </summary>
    /// <param name="etoType"></param>
    /// <param name="count"></param>
    private void AddScores(EtoType? etoType, int count)
    {
        // �X�R�A�����Z(EtoPoint * ��������)
        GameData.instance.score += GameData.instance.etoPoint * count;

        // ���������x�̐������Z
        GameData.instance.eraseEtoCount += count;

        // �X�R�A���Z�Ɖ�ʂ̍X�V����
        uiManager.UpdateDisplayScore();
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameUp()
    {
        // gameState�����U���g�ɕύX���� = Update���\�b�h�������Ȃ��Ȃ�
        gameState = GameState.Result;

        yield return new WaitForSeconds(1.5f);

        // TODO �������������܂�
        yield return StartCoroutine(MoveResultPopUp());

        // TODO���U���g�̏�������������
        //Debug.Log("���U���g�̃|�b�v�A�b�v���ړ������܂�");
    }

    private IEnumerator MoveResultPopUp()
    {
        // DoTween�̋@�\���g���āAResultPopUp�Q�[���I�u�W�F�N�g����ʊO�����ʓ��Ɉړ�������
        resultPopUp.transform.DOMoveY(0, 1.0f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // TODO �ړ�������ɁA���U���g���e��\��
                Debug.Log("���U���g���e��\�����܂�");
            }
         );

        yield return new WaitForSeconds(1.0f);
    }
}
