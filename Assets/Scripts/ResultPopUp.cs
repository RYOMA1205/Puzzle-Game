using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultPopUp : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Text txtEraseEtoCount;

    [SerializeField]
    private Button btnClosePopUp;

    void Start()
    {
        // btnClosePopUpゲームオブジェクトの持つCanvasGruopのAlphaを 0 に設定して透明にしておく(最初はタップできず、かつ見えないようにしておく)
        btnClosePopUp.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    public void DisplayResult(int score, int eraseEtoCount)
    {
        // 計算用の初期値を設定
        int initValue = 0;

        // DOTweenのSeapuence(シーケンス)機能を初期化して使用できるようにする
        Sequence sequence = DOTween.Sequence();

        // シーケンスを利用して、DOTweenの処理を制御したい順番で記述する。まずは①スコアの数字をアニメいて表示
        sequence.Append(DOTween.To(() => initValue,
            (num) =>
            {
                initValue = num;
                txtScore.text = num.ToString();
            },
            score,
            1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));  // 次の処理のためにOnCompleteを使ってinitValueを初期化

        // ②シーケンス処理を0.5秒だけ待機
        sequence.AppendInterval(0.5f);

        // ③消した干支の数字をアニメして表示
        sequence.Append(DOTween.To(() => initValue,
            (num) =>
            {
                initValue = num;
                txtEraseEtoCount.text = num.ToString();
            },
            eraseEtoCount,
            1.0f).SetEase(Ease.InCirc));

        // ④シーケンス処理を0.5秒だけ待機
        sequence.AppendInterval(1.0f);

        // ⑤透明になっているbtnClosePopUpとその子要素をCanvasGroupのAlphaを使用して徐々に表示
        sequence.Append(btnClosePopUp.gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
    }

    
}
