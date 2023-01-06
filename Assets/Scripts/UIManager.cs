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

    // DOTweenの処理を代入する変数
    private Tweener tweener = null;

    // UnityEventとしてメソッドを代入する変数
    private UnityEvent unityEvent;

    /// <summary>
    /// UIManagerの初期設定
    /// </summary>
    /// <returns></returns>
    public IEnumerator Initialize()
    {
        // シャッフルボタンを非活性化(半透明の押せない状態にする)
        ActivateShuffleButton(false);

        // シャッフル機能の設定
        shuffle.SetUpShuffle(this);

        // シャッフルボタンにメソッドを登録
        btnShuffle.onClick.AddListener(TriggerShuffle);

        yield break;
    }

    /// <summary>
    /// シャッフルボタンの活性化/非活性化の切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ActivateShuffleButton(bool isSwitch)
    {
        btnShuffle.interactable = isSwitch;
    }

    private void TriggerShuffle()
    {
        // 発展13で追加
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Shuffle);

        // シャッフルボタンを押せなくする。重複タップ防止
        ActivateShuffleButton(false);

        // シャッフル開始
        shuffle.StartShuffle();
    }

    /// <summary>
    /// 画面表示スコアの更新処理
    /// </summary>
    public void UpdateDisplayScore(bool isChooseEto = false)
    {
        if (isChooseEto)
        {
            // 選択している干支の場合にはスコアを大きく表示する演出を入れる
            Sequence sequence = DOTween.Sequence();
            sequence.Append(txtScore.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f)).SetEase(Ease.InCirc);
            sequence.AppendInterval(0.1f);
            sequence.Append(txtScore.transform.DOScale(Vector3.one, 0.1f)).SetEase(Ease.Linear);
        }
        // 画面に表示しているスコアの値を更新
        txtScore.text = GameData.instance.score.ToString();
    }

    /// <summary>
    /// ゲームの残り時間の表示更新
    /// </summary>
    /// <param name="times"></param>
    public void UpdateDisplayGameTime(float time)
    {
        txtTimer.text = time.ToString("F0");
    }

    /// <summary>
    /// 選択した干支の持つスキルを登録
    /// </summary>
    /// <param name="unityAction"></param>
    /// <returns></returns>
    public IEnumerator SetUpSkillButton(UnityAction unityAction)
    {
        // スキルポイントが0からスタートするので、スキルボタンを押せなくしておく
        btnSkill.interactable = false;

        // スキルの登録がない場合、スキルボタンには何も登録しない
        if (unityAction == null)
        {
            yield break;
        }

        // UnityEvent初期化
        unityEvent = new UnityEvent();

        // UnityEventにunityActionを登録(UnityActionにはメソッドが代入されている)
        unityEvent.AddListener(unityAction);

        // スキルボタンにメソッドを登録
        btnSkill.onClick.AddListener(TriggerSkill);
    }

    /// <summary>
    /// スキルポイント加算
    /// </summary>
    /// <param name="count"></param>
    public void AddSkillPoint(int count)
    {
        // FillAmountの現在地を代入
        float a = imgSkillPoint.fillAmount;

        // 消した干支の数をFillAmount用に計算し代入
        float value = a += count * 0.05f;

        // DOTweenのDOFillAmountメソッドを使用してアニメさせながらFillAmountを操作
        imgSkillPoint.DOFillAmount(value, 0.5f);

        // FillAmountが　1　になり、スキルボタンがアニメしていなければ
        if (imgSkillPoint.fillAmount >= 1.0f && tweener == null)
        {
            Debug.Log(imgSkillPoint.fillAmount);

            // スキルボタンを押せるようにする
            btnSkill.interactable = true;

            // ループ処理を行い、スキルボタンが押されるまでスケールを変化させるアニメを実行する。それをtweener変数に代入しておく
            tweener = imgSkillPoint.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo);
        }
    }

    /// <summary>
    /// スキル使用
    /// </summary>
    public void TriggerSkill()
    {
        // 発展13で追加
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Skill);

        // ボタンの重複タップ防止
        btnSkill.interactable = false;

        // 登録されているスキル(UnityActionに代入されているメソッド)を使用
        unityEvent.Invoke();

        // スキルポイント関連を初期化
        imgSkillPoint.DOFillAmount(0, 1.0f);

        // スキルボタンのループアニメを破棄し、tweener変数をnullにする
        tweener.Kill();
        tweener = null;

        // スキルボタンのスケールを元の大きさに戻す
        imgSkillPoint.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 複数のボタンを押せないようにまとめて制御する
    /// </summary>
    public void InActiveButtons()
    {
        btnSkill.interactable = false;
        btnShuffle.interactable = false;
    }
}
