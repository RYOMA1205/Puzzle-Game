using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;      // DOTween �𗘗p���邽�߂ɕK�v�ɂȂ邽�߁A�ǉ����܂�
using System.Linq;
using UnityEngine.Events;

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

    // ���W�ō폜
    //[SerializeField, Header("���x�̉摜�f�[�^")]
    //private Sprite[] etoSprites;

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
    private ResultPopUp resultPopUp;           // �^�� GameObject ���� ResultPopUp �ɕύX����

    // ���W�Œǉ�
    [SerializeField, Header("����̃Q�[���Ő������銱�x�̎��")]
    private List<GameData.EtoData> selectedEtoDataList = new List<GameData.EtoData>();

    [SerializeField, Header("���x�̍폜���o�G�t�F�N�g�̃v���t�@�u")]
    private GameObject eraseEffectPrefab;

    // EtoSelectPopUp����������
    [SerializeField]
    private EtoSelectPopUp etoSelectPopUp;

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
        StartCoroutine(TransitionManager.instance.FadePanel(0.0f));

        // ���W13�Œǉ�
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Select);

        // �X�R�A�Ȃǂ�������
        GameData.instance.InitGame();

        // gameState���������ɕύX
        // ���W6�ŃX�e�[�g�����x�I�𒆂ɕύX
        gameState = GameState.Select;       // <= �O��GameState.Ready����ς���

        // ���W3�ŏ�����ǉ�
        // UIManager�̏����ݒ�
        yield return StartCoroutine(uiManager.Initialize());

        // ���x�f�[�^�̃��X�g���쐬����ĂȂ����
        if (GameData.instance.etoDataList.Count == 0)
        {
            // ���x�f�[�^�̃��X�g���쐬�B���̏������I������܂ŁA���̏����ւ͂����Ȃ��悤�ɂ���
            yield return StartCoroutine(GameData.instance.InitEtoDataList());
        }

        // ����̃Q�[���ɓo�ꂷ�銱�x�������_���őI���B���̏������I������܂ŁA���̏����ւ͂����Ȃ��悤�ɂ���
        // ���W6�Ŋ��x�̑I���|�b�v�A�b�v�Ɋ��x�I���{�^���𐶐��B���̏������I������܂ŁA���̏����ւ͂����Ȃ��悤�ɂ���
        yield return StartCoroutine(etoSelectPopUp.CreateEtoButtons(this));

        Debug.Log("�����m�F�p");

        // ���W�ō폜
        // ���x�̉摜��ǂ݂��ށB
        // ���̏������I������܂ŁA���̏����ւ͂����Ȃ��悤�ɂ���
        //yield return StartCoroutine(LoadEtoSprites());

        // �������甭�W6�ō폜(���̃��\�b�h�ֈڍs)
        //yield return StartCoroutine(SetUpEtoTypes(GameData.instance.etoTypeCount));

        // �c�莞�Ԃ̕\��
        //uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);

        // �����Ŏw�肵�����̊��x�𐶐�����
        //StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
    }

    // 10�ł�������ǉ�

    // ���x�̉摜��ǂݍ���Ŕz�񂩂�g�p�ł���悤�ɂ���
    // ���W�ł��̃��\�b�h���̂��폜
    //private IEnumerator LoadEtoSprites()
    //{
    // �z��̏�����(12�̉摜������悤��Sprite�^�̔z���12�p�ӂ���)
    //etoSprites = new Sprite[(int)EtoType.Count];

    // Resources.LoadAll���s���A��������Ă��銱�x�̉摜�����ԂɑS�ēǂݍ���Ŕz��ɑ��
    //etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

    // ���@1�̃t�@�C����12�������Ă��Ȃ��ꍇ�ɂ͈ȉ��̏������g��
    //     12�������Ă���ꍇ�ɂ͎g�p���Ȃ�
    //for(int i = 0; i < etoSprites.Length; i++)
    //{
    //etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
    //}

    //yield break;
    //}

    // PreparateGame���\�b�h���ɃX�L���{�^���̐ݒ���s��������ǉ�(���W11��)
    public IEnumerator PreparateGame()
    {
        // �X�e�[�g���������ɕύX
        gameState = GameState.Ready;

        // �c�莞�Ԃ̕\��
        uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);

        // �Q�[���ɓo�ꂳ���銱�x�̎�ނ�ݒ肷��
        yield return StartCoroutine(SetUpEtoTypes(GameData.instance.etoTypeCount));

        // GameData��selectedSkillType��n���āA�X�L���{�^���ɓo�^���郁�\�b�h(�X�L�����s��)��ݒ�
        yield return StartCoroutine(SetUpSkill(GameData.instance.selectedSkillType));

        // �����Ŏw�肵�����̊��x�𐶐�����
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));

        // ���W13�Œǉ�
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Game);
    }
    
    /// <summary>
    /// �Q�[���ɓo�ꂳ���銱�x�̎�ނ�ݒ肷��
    /// </summary>
    /// <param name="typeCount">�������銱�x�̎�ސ�</param>
    /// <returns></returns>
    private IEnumerator SetUpEtoTypes(int typeCount)
    {
        // �V�������X�g��p�ӂ��ď������ɍ��킹��etoDataList�𕡐����āA���x�̌�⃊�X�g�Ƃ���
        List<GameData.EtoData> candidateEtoDataList = new List<GameData.EtoData>(GameData.instance.etoDataList);

        // ���W6�ŏ�����ǉ�
        // �I�𒆂̊��x��T���Đ������銱�x�̃��X�g�ɒǉ�
        GameData.EtoData myEto = candidateEtoDataList.Find((x) => x.etoType == GameData.instance.selectedEtoData.etoType);
        selectedEtoDataList.Add(myEto);
        candidateEtoDataList.Remove(myEto);
        typeCount--;

        // ���x���w�萔�����������_���ɑI��(���x�̎�ނ͏d�������Ȃ�)
        // while���� ������ �𖞂������胋�[�v����B�����ł� typeCount �ϐ��̒l�� 0 �����傫���Ԃ� while ���̏������J��Ԃ����s�����
        while (typeCount > 0)
        {
            // �����_���ɐ�����I��
            int randomValue = Random.Range(0, candidateEtoDataList.Count);

            // ����̃Q�[���ɐ������銱�x���X�g�ɒǉ�
            selectedEtoDataList.Add(candidateEtoDataList[randomValue]);

            // ���x�̃��X�g����I�����ꂽ���x�̏����폜(���x���d�������Ȃ�����)
            candidateEtoDataList.Remove(candidateEtoDataList[randomValue]);

            // �I�������������炷
            typeCount--;

            // ������1�t���[���ҋ@�����čĊJ����B����Ė��t���[���̃��[�v�����ɂȂ�
            yield return null;
        }
    }

    /// <summary>
    /// ���x�𐶐�
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private IEnumerator CreateEtos(int count)
    {
        // ���W3�ŏ�����ǉ�
        // ���x�̐������̓V���b�t���{�^���������Ȃ��悤�ɂ���
        uiManager.ActivateShuffleButton(false);

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
            // ���W�ŏC��
            // ����̃Q�[���ɓo�ꂷ�銱�x�̒�����A�����_���Ȋ��x��1�I��
            //int randomValue = Random.Range(0, (int)EtoType.Count);
            int randomValue = Random.Range(0, selectedEtoDataList.Count);

            // �������ꂽ���x�̏����ݒ�(���x�̎�ނƊ��x�̉摜���������g����Eto�֓n��)
            // ���x�̏����ݒ�
            //eto.SetUpEto((EtoType)randomValue, etoSprites[randomValue]);
            eto.SetUpEto(selectedEtoDataList[randomValue].etoType, selectedEtoDataList[randomValue].sprite);

            // etoList�ɒǉ�
            etoList.Add(eto);

            // 0.03�b�҂��Ď��̊��x�𐶐�
            yield return new WaitForSeconds(0.03f);
        }

        // ���W3�ŏ�����ǉ�
        // ���x�̐������I��������V���b�t���{�^����������悤�ɂ���
        uiManager.ActivateShuffleButton(true);

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
    /// ���W11�Ń��\�b�h���ŃX�L���|�C���g�����Z���鏈����ǉ�
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

                // ���W4�Œǉ�
                // ���x�̍폜���o�G�t�F�N�g����
                GameObject effect = Instantiate(eraseEffectPrefab, eraseEtoList[i].gameObject.transform);

                // �G�t�F�N�g�̈ʒu��EtoSetTran���ɕύX(���x�̎q�I�u�W�F�N�g�̂܂܂��ƁA���x���j�������Ɠ����ɃG�t�F�N�g���j������Ă��܂�����)
                effect.transform.SetParent(etoSetTran);

                // ���x���폜
                Destroy(eraseEtoList[i].gameObject);

                //  ���W13�Œǉ�
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Erase);
            }

            // �X�R�A�Ə��������x�̐��̉��Z
            AddScores(currentEtoType, eraseEtoList.Count);

            // �X�L���|�C���g�����Z
            uiManager.AddSkillPoint(eraseEtoList.Count);

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
    /// <param name="etoType">���������x�̎��</param>
    /// <param name="count">���������x�̐�</param>
    private void AddScores(EtoType? etoType, int count)
    {
        // �X�R�A�����Z
        // �������甭�W8�Œǉ����C��
        // ���������x���I������Ă��銱�x���ǂ����𔻒肷��ϐ��Btrue�Ȃ�I������Ă��銱�x�Ƃ���
        bool isChooseEto = false;
        
        if (etoType == GameData.instance.selectedEtoData.etoType)
        {
            // �I�����Ă��銱�x�̏ꍇ�ɂ̓X�R�A�𑽂����Z�@etoPoint * ���������x�̐��@* etoRate
            GameData.instance.score += Mathf.CeilToInt(GameData.instance.etoPoint * count * GameData.instance.etoRate);
            isChooseEto = true;
        }
        else
        {
            // ����ȊO�� etoPoint * ���������x�̐��@�����Z
            GameData.instance.score += GameData.instance.etoPoint * count;
        }

        // ���������x�̐������Z
        GameData.instance.eraseEtoCount += count;

        // �X�R�A���Z�Ɖ�ʂ̍X�V����
        uiManager.UpdateDisplayScore(isChooseEto);
    }

    /// <summary>
    /// �Q�[���I������
    /// ���W11�Ń��\�b�h���ŃV���b�t���{�^����񊈐������鏈�����폜���A�V�����V���b�t���ƃX�L���̗��{�^���̔񊈐�����ǉ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameUp()
    {
        // ���W3�ŏ�����ǉ�
        // ���W11�ō폜
        // �V���b�t���{�^����񊈐������ĉ����Ȃ�����
        //uiManager.ActivateShuffleButton(false);

        // �V���b�t���{�^���ƃX�L���{�^����񊈐������ĉ����Ȃ�����
        uiManager.InActiveButtons();

        // gameState�����U���g�ɕύX���� = Update���\�b�h�������Ȃ��Ȃ�
        gameState = GameState.Result;

        yield return new WaitForSeconds(1.5f);

        // TODO �������������܂�
        yield return StartCoroutine(MoveResultPopUp());

        // TODO���U���g�̏�������������
        //Debug.Log("���U���g�̃|�b�v�A�b�v���ړ������܂�");
    }


    /// <summary>
    /// ���U���g�|�b�v�A�b�v����ʓ��Ɉړ��@�@�@�@�@�@�@//�@���@���\�b�h����TODO������ύX���ď����������@��
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveResultPopUp()
    {
        // DoTween�̋@�\���g���āAResultPopUp�Q�[���I�u�W�F�N�g����ʊO�����ʓ��Ɉړ�������(Y���W��0��)
        resultPopUp.transform.DOMoveY(0, 1.0f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // ���U���g�\��(�X�R�A�Ə��������x�̐���n��)TODO����������
                resultPopUp.DisplayResult(GameData.instance.score, GameData.instance.eraseEtoCount);

                // TODO �ړ�������ɁA���U���g���e��\��
                // Debug.Log("���U���g���e��\�����܂�");
            }
         );

        // ���W13�Œǉ�
        // SE��炷�܂ł̑ҋ@����
        yield return new WaitForSeconds(0.5f);       // <= �ҋ@���Ԃ͉����ɍ��킹�ēK�X����

        // �h�������[����SE�Đ�
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Result);

        yield return new WaitForSeconds(2.5f);       // <= �ҋ@���Ԃ͉����ɍ��킹�ēK�X����

        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Result);
    }

    /// <summary>
    /// �ł����̑������x�̃^�C�v���܂Ƃ߂č폜����
    /// </summary>
    public void DeleteMaxEtoType()
    {
        // Dictinary�̐錾�ƒ�`�B���x�̃^�C�v�Ƃ��̐������ł���悤�ɂ���
        Dictionary<EtoType, int> dictionary = new Dictionary<EtoType, int>();

        // ���X�g�̒����犱�x�̃^�C�v���Ƃ�Dictionary�̗v�f���쐬(������5�̊��x�^�C�v���Ƃɂ����������邩�킩��)
        foreach (Eto eto in etoList)
        {
            if (dictionary.ContainsKey(eto.etoType))
            {
                // ���ɂ���v�f(���x�̃^�C�v)�̏ꍇ�ɂ͐��̃J�E���g�����Z
                dictionary[eto.etoType]++;
            }
            else
            {
                // �܂�����Ă��Ȃ��v�f(���x�̃^�C�v)�̏ꍇ�ɂ͐V�����v�f�����A�J�E���g��1�Ƃ���
                dictionary.Add(eto.etoType, 1);
            }
        }

        // Debug
        foreach (KeyValuePair<EtoType, int> keyValuePair in dictionary)
        {
            Debug.Log("���x : " + keyValuePair.Key + " �� : " + keyValuePair.Value);
        }

        // Dictionary���������A�ł����̑������x�̃^�C�v�������āA�������x�̃^�C�v�Ɛ�������
        EtoType maxEtoType = dictionary.OrderByDescending(x => x.Value).First().Key;
        int removeNum = dictionary.OrderByDescending(x => x.Value).First().Value;

        Debug.Log("�������x�̃^�C�v : " + maxEtoType + " �� : " + removeNum);

        // �Ώۂ̊��x��j��
        for (int i = 0; i < etoList.Count; i ++)
        {
            if (etoList[i].etoType == maxEtoType)
            {
                Destroy(etoList[i].gameObject);
            }
        }

        // etoList����Ώۂ̊��x���폜
        etoList.RemoveAll(x => x.etoType == maxEtoType);

        // �_���Ə��������x�ւ̉��Z
        AddScores(maxEtoType, removeNum);

        // �j�󂵂����x�̐��������x�𐶐�
        StartCoroutine(CreateEtos(removeNum));
    }

    /// <summary>
    /// �I�����ꂽ�X�L�����{�^���ɓo�^
    /// </summary>
    /// <param name="skillType"></param>
    /// <returns></returns>
    private IEnumerator SetUpSkill(SkillType skillType)
    {
        yield return StartCoroutine(uiManager.SetUpSkillButton(GetSkill(skillType)));
    }

    public UnityAction GetSkill(SkillType chooseSkillType)
    {
        switch (chooseSkillType)
        {
            case SkillType.DeleteMaxEtoType:
                return DeleteMaxEtoType;

            // TODO �X�L�����������ꍇ�ɂ͒ǉ�����
        }
        return null;
    }

}
