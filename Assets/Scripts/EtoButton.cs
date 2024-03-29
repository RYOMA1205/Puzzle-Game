using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EtoButton : MonoBehaviour
{
    [Header("このボタンの干支データ")]
    public GameData.EtoData etoData;

    // 画像設定用
    public Image imgEto;

    // ボタン制御用
    public Button btnEto;

    private EtoSelectPopUp etoSelectPopUp;

    [SerializeField]
    private CanvasGroup canvasGroup;

    /// <summary>
    /// 干支ボタンの初期設定
    /// </summary>
    /// <param name="etoSelectPopUp"></param>
    /// <param name="etoData"></param>
    public void SetUpEtoButton(EtoSelectPopUp etoSelectPopUp, GameData.EtoData etoData)
    {
        // 干支ボタン全体を透明にする
        canvasGroup.alpha = 0.0f;

        // 引数の値を代入
        this.etoSelectPopUp = etoSelectPopUp;
        this.etoData = etoData;

        // 干支ボタンの画像を干支データの画像に変更(この処理によってImageを各干支の画像に差し替える)
        imgEto.sprite = this.etoData.sprite;

        // ボタンにメソッドを登録
        btnEto.onClick.AddListener(() => StartCoroutine(OnClickEtoButton()));

        // アニメさせながらボタンを徐々に表示する
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.Linear));
        sequence.Join(transform.DOPunchScale(new Vector3(1, 1, 1), 0.5f));
    }

    /// <summary>
    /// 干支ボタンをタップした際の処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnClickEtoButton()
    {
        // 発展13で追加
        SoundManager.instance.PlaySE(SoundManager.SE_Type.OK);

        // 干支ボタンの保持する干支データをGameDataに代入(選択した干支データとする)
        GameData.instance.selectedEtoData = etoData;

        // 干支ボタンをポップアニメさせる
        transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.15f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.15f);
        transform.DOScale(Vector3.one, 0.15f);

        // 干支ボタンの色を選択中の色に変更し、他の干支ボタンの色を選択中でない色に変更
        etoSelectPopUp.ChangeColorToEtoButton(etoData.etoType);
    }

}
