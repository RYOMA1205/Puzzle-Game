using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [Header("�Q�[���ɓo�ꂷ�銱�x�̍ő��ސ�")]
    public int etoTypeCount = 5;

    [Header("�Q�[���J�n���ɐ������銱�x�̐�")]
    public int createEtoCount = 50;

    [Header("���݂̃X�R�A")]
    public int score = 0;

    [Header("���x���������ۂɉ��Z�����X�R�A")]
    public int etoPoint = 100;

    [Header("���������x�̐�")]
    public int eraseEtoCount = 0;

    [SerializeField, Header("1��ӂ�̃Q�[������")]
    private int initTime = 60;

    [Header("���݂̃Q�[���̎c�莞��")]
    public float gameTime;

    [Header("�I�����Ă��銱�x")]
    public EtoData selectedEtoData;

    [Header("�I�����Ă��銱�x�����������̃X�R�A�{��")]
    public float etoRate = 3;

    // ���W11�Œǉ�
    [Header("�I�𒆂̃X�L��")]
    public SkillType selectedSkillType;

    /// <summary>
    /// ���x�̊�{���
    /// </summary>
    [System.Serializable]
    public class EtoData
    {
        public EtoType etoType;
        public Sprite sprite;

        // �R���X�g���N�^(�C���X�^���X(new)���ɗp�ӂ��Ă�������ւ̒l�̑�����������郁�\�b�h)
        public EtoData(EtoType etoType, Sprite sprite)
        {
            this.etoType = etoType;
            this.sprite = sprite;
        }
    }

    [Header("���x12��ނ̃��X�g")]
    public List<EtoData> etoDataList = new List<EtoData>();

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

        // ���W6�ō폜
        // �Q�[���̏�����
        //InitGame();
    }

    // �l���p
    //private void Update()
    //{
        //gameTime -= Time.deltaTime;

        //initTime = (int)gameTime;
    //}

    /// <summary>
    /// �Q�[��������
    /// </summary>
    public void InitGame()
    {
        score = 0;
        eraseEtoCount = 0;

        // �Q�[�����Ԃ�ݒ�
        gameTime = initTime;

        Debug.Log("Init Game");
    }

    /// <summary>
    /// ���݂̃Q�[���V�[�����ēǂݍ���
    /// </summary>
    /// <returns></returns>
    public IEnumerator RestartGame()
    {
        // ���W12�ō폜
        //yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine(TransitionManager.instance.FadePanel(1.0f));

        // ���݂̃Q�[���V�[�����擾���A�V�[���̖��O���g����LoadScene�������s��(�ēx�A�����Q�[���V�[�����Ăяo��)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // ���W6�ō폜
        // �������@GameData�Q�[���I�u�W�F�N�g�̓V�[���J�ڂ��Ă��j������Ȃ��ݒ�ɂȂ��Ă���̂ŁA�����ōēx�������̏������s���K�v������B
        //InitGame();
    }

    /// <summary>
    /// ���x�f�[�^�̃��X�g���쐬
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitEtoDataList()
    {
        // ���x�̉摜��ǂ݂��ނ��߂̕ϐ���z��ŗp��(GameManager�̐錾�t�B�[���h�ŗp�ӂ��Ă������̂��A���̃��\�b�h���݂̂Ŏg�p����悤�ɕύX)
        Sprite[] etoSprites = new Sprite[(int)EtoType.Count];

        // Resources.LoadAll���s���A��������Ă��銱�x�̉摜�����Ԃɂ��ׂēǂݍ���Ŕz��ɑ��
        etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

        //for (int i = 0; i < etoSprites.Length; i++)
        //{
             //etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
        //}

        // �Q�[���ɓo�ꂷ��12��ނ̊��x�f�[�^���쐬
        for (int i = 0; i < (int)EtoType.Count; i++)
        {
            // ���x�̏��������N���X EtoData ���C���X�^���X(new EtoData())���A�R���X�g���N�^���g���Ēl����
            EtoData etoData = new EtoData((EtoType)i, etoSprites[i]);

            // ���x�f�[�^��List�֒ǉ�
            etoDataList.Add(etoData);
        }
        

        yield break;
    }
}
