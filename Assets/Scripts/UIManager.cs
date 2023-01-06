using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Text txtTimer;

    [SerializeField]
    private Shuffle shuffle;

    [SerializeField]
    private Button btnShuffle;

    [SerializeField]
    private Button btnSkill;

    [SerializeField]
    private Image imgSkillPoint;

    // DOTween�̏�����������ϐ�
    private Tweener tweener = null;

    // UnityEvent�Ƃ��ă��\�b�h��������ϐ�
    private UnityEvent unityEvent;

    /// <summary>
    /// UIManager�̏����ݒ�
    /// </summary>
    /// <returns></returns>
    public IEnumerator Initialize()
    {
        // �V���b�t���{�^����񊈐���(�������̉����Ȃ���Ԃɂ���)
        ActivateShuffleButton(false);

        // �V���b�t���@�\�̐ݒ�
        shuffle.SetUpShuffle(this);

        // �V���b�t���{�^���Ƀ��\�b�h��o�^
        btnShuffle.onClick.AddListener(TriggerShuffle);

        yield break;
    }

    /// <summary>
    /// �V���b�t���{�^���̊�����/�񊈐����̐؂�ւ�
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ActivateShuffleButton(bool isSwitch)
    {
        btnShuffle.interactable = isSwitch;
    }

    private void TriggerShuffle()
    {
        // ���W13�Œǉ�
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Shuffle);

        // �V���b�t���{�^���������Ȃ�����B�d���^�b�v�h�~
        ActivateShuffleButton(false);

        // �V���b�t���J�n
        shuffle.StartShuffle();
    }

    /// <summary>
    /// ��ʕ\���X�R�A�̍X�V����
    /// </summary>
    public void UpdateDisplayScore(bool isChooseEto = false)
    {
        if (isChooseEto)
        {
            // �I�����Ă��銱�x�̏ꍇ�ɂ̓X�R�A��傫���\�����鉉�o������
            Sequence sequence = DOTween.Sequence();
            sequence.Append(txtScore.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f)).SetEase(Ease.InCirc);
            sequence.AppendInterval(0.1f);
            sequence.Append(txtScore.transform.DOScale(Vector3.one, 0.1f)).SetEase(Ease.Linear);
        }
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

    /// <summary>
    /// �I���������x�̎��X�L����o�^
    /// </summary>
    /// <param name="unityAction"></param>
    /// <returns></returns>
    public IEnumerator SetUpSkillButton(UnityAction unityAction)
    {
        // �X�L���|�C���g��0����X�^�[�g����̂ŁA�X�L���{�^���������Ȃ����Ă���
        btnSkill.interactable = false;

        // �X�L���̓o�^���Ȃ��ꍇ�A�X�L���{�^���ɂ͉����o�^���Ȃ�
        if (unityAction == null)
        {
            yield break;
        }

        // UnityEvent������
        unityEvent = new UnityEvent();

        // UnityEvent��unityAction��o�^(UnityAction�ɂ̓��\�b�h���������Ă���)
        unityEvent.AddListener(unityAction);

        // �X�L���{�^���Ƀ��\�b�h��o�^
        btnSkill.onClick.AddListener(TriggerSkill);
    }

    /// <summary>
    /// �X�L���|�C���g���Z
    /// </summary>
    /// <param name="count"></param>
    public void AddSkillPoint(int count)
    {
        // FillAmount�̌��ݒn����
        float a = imgSkillPoint.fillAmount;

        // ���������x�̐���FillAmount�p�Ɍv�Z�����
        float value = a += count * 0.05f;

        // DOTween��DOFillAmount���\�b�h���g�p���ăA�j�������Ȃ���FillAmount�𑀍�
        imgSkillPoint.DOFillAmount(value, 0.5f);

        // FillAmount���@1�@�ɂȂ�A�X�L���{�^�����A�j�����Ă��Ȃ����
        if (imgSkillPoint.fillAmount >= 1.0f && tweener == null)
        {
            Debug.Log(imgSkillPoint.fillAmount);

            // �X�L���{�^����������悤�ɂ���
            btnSkill.interactable = true;

            // ���[�v�������s���A�X�L���{�^�����������܂ŃX�P�[����ω�������A�j�������s����B�����tweener�ϐ��ɑ�����Ă���
            tweener = imgSkillPoint.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo);
        }
    }

    /// <summary>
    /// �X�L���g�p
    /// </summary>
    public void TriggerSkill()
    {
        // ���W13�Œǉ�
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Skill);

        // �{�^���̏d���^�b�v�h�~
        btnSkill.interactable = false;

        // �o�^����Ă���X�L��(UnityAction�ɑ������Ă��郁�\�b�h)���g�p
        unityEvent.Invoke();

        // �X�L���|�C���g�֘A��������
        imgSkillPoint.DOFillAmount(0, 1.0f);

        // �X�L���{�^���̃��[�v�A�j����j�����Atweener�ϐ���null�ɂ���
        tweener.Kill();
        tweener = null;

        // �X�L���{�^���̃X�P�[�������̑傫���ɖ߂�
        imgSkillPoint.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// �����̃{�^���������Ȃ��悤�ɂ܂Ƃ߂Đ��䂷��
    /// </summary>
    public void InActiveButtons()
    {
        btnSkill.interactable = false;
        btnShuffle.interactable = false;
    }
}
