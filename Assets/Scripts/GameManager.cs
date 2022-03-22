using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 10で変更
    // 宣言する型を GameObject型 から Eto型に変更
    // EtoPrefabは前と同じようにProject内からアサインできる
    [Header("干支のプレファブ")]
    public Eto etoPrefab;

    [SerializeField, Header("干支の生成位置")]
    private Transform etoSetTran;

    [SerializeField, Header("干支生成時の最大回転角度")]
    private float maxRotateAngle = 35.0f;

    //[SerializeField, Header("干支生成時のランダム幅")]
    //private float maxRange = 400.0f;

    //[SerializeField, Header("干支生成時の落下位置")]
    //private float fallPos = 1400.0f;

    // 10でここから追加
    [SerializeField, Header("生成された干支のリスト")]
    private List<Eto> etoList = new List<Eto>();

    [SerializeField, Header("干支の画像データ")]
    private Sprite[] etoSprites;

    // 戻り値をvoidからIEnumerator型に変更して、コルーチンメソッドにする
    IEnumerator Start()
    {
        // 干支の画像を読みこむ。
        // この処理が終了するまで、次の処理へはいかないようにする
        yield return StartCoroutine(LoadEtoSprites());

        // 引数で指定した数の干支を生成する
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
    }

    // 10でここから追加

    /// <summary>
    /// 干支の画像を読み込んで配列から使用できるようにする
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadEtoSprites()
    {
        // 配列の初期化(12個の画像が入るようにSprite型の配列を12個用意する)
        etoSprites = new Sprite[(int)EtoType.Count];

        // Resources.LoadAllを行い、分割されている干支の画像を順番に全て読み込んで配列に代入
        etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

        // ※　1つのファイルを12分割していない場合には以下の処理を使う
        //     12分割している場合には使用しない
        //for(int i = 0; i < etoSprites.Length; i++)
        //{
        //etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
        //}

        yield break;
    }

    /// <summary>
    /// 干支を生成
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private IEnumerator CreateEtos(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 干支プレファブのクローンを、干支の生成位置に生成
            Eto eto = Instantiate(etoPrefab, etoSetTran, false);

            // 生成された干支の回転情報を設定(色々な角度になるように)
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-maxRotateAngle, maxRotateAngle), Vector3.forward);

            // 生成位置をランダムにして落下位置を変化させる
            eto.transform.localPosition = new Vector2(Random.Range(-400.0f, 400.0f), 1400f);

            // 10でここから追加
            // ランダムな干支を12種類の中から1つ選択
            int randomValue = Random.Range(0, (int)EtoType.Count);

            // 生成された干支の初期設定(干支の種類と干支の画像を引数を使ってEtoへ渡す)
            eto.SetUpEto((EtoType)randomValue, etoSprites[randomValue]);

            // etoListに追加
            etoList.Add(eto);

            // 0.03秒待って次の干支を生成
            yield return new WaitForSeconds(0.03f);
        }
    }
}
