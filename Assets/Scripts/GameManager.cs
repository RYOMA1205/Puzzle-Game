using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;      // DOTween を利用するために必要になるため、追加します
using System.Linq;
using UnityEngine.Events;

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

    // 発展で削除
    //[SerializeField, Header("干支の画像データ")]
    //private Sprite[] etoSprites;

    // 11でここから追加
    // 最初にドラッグした干支の情報
    private Eto firstSelectEto;

    // 最後にドラッグした干支の情報
    private Eto lastSelectEto;

    // 最初にドラッグした干支の種類
    private EtoType? currentEtoType;

    [SerializeField, Header("削除対象となる干支を登録するリスト")]
    private List<Eto> eraseEtoList = new List<Eto>();

    [SerializeField, Header("つながっている干支の数")]
    private int linkCount = 0;

    [Header("スワイプでつながる干支の範囲")]
    public float etoDistance = 1.0f;

    [SerializeField]
    private UIManager uiManager;

    // 残り時間計測用
    private float timer;

    [SerializeField]
    private ResultPopUp resultPopUp;           // 型を GameObject から ResultPopUp に変更する

    // 発展で追加
    [SerializeField, Header("今回のゲームで生成する干支の種類")]
    private List<GameData.EtoData> selectedEtoDataList = new List<GameData.EtoData>();

    [SerializeField, Header("干支の削除演出エフェクトのプレファブ")]
    private GameObject eraseEffectPrefab;

    // EtoSelectPopUpを扱うため
    [SerializeField]
    private EtoSelectPopUp etoSelectPopUp;

    /// <summary>
    /// ゲームの進行状況
    /// </summary>
    public enum GameState
    {
        Select,   // 干支の選択中
        Ready,    // ゲームの準備中
        Play,     // ゲームのプレイ中
        Result    // リザルト中
    }

    [Header("現在のゲームの進行状況")]
    public GameState gameState = GameState.Select;

    // 戻り値をvoidからIEnumerator型に変更して、コルーチンメソッドにする
    IEnumerator Start()
    {
        StartCoroutine(TransitionManager.instance.FadePanel(0.0f));

        // 発展13で追加
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Select);

        // スコアなどを初期化
        GameData.instance.InitGame();

        // gameStateを準備中に変更
        // 発展6でステートを干支選択中に変更
        gameState = GameState.Select;       // <= 前のGameState.Readyから変える

        // 発展3で処理を追加
        // UIManagerの初期設定
        yield return StartCoroutine(uiManager.Initialize());

        // 干支データのリストが作成されてなければ
        if (GameData.instance.etoDataList.Count == 0)
        {
            // 干支データのリストを作成。この処理が終了するまで、次の処理へはいかないようにする
            yield return StartCoroutine(GameData.instance.InitEtoDataList());
        }

        // 今回のゲームに登場する干支をランダムで選択。この処理が終了するまで、次の処理へはいかないようにする
        // 発展6で干支の選択ポップアップに干支選択ボタンを生成。この処理が終了するまで、次の処理へはいかないようにする
        yield return StartCoroutine(etoSelectPopUp.CreateEtoButtons(this));

        Debug.Log("処理確認用");

        // 発展で削除
        // 干支の画像を読みこむ。
        // この処理が終了するまで、次の処理へはいかないようにする
        //yield return StartCoroutine(LoadEtoSprites());

        // ここから発展6で削除(他のメソッドへ移行)
        //yield return StartCoroutine(SetUpEtoTypes(GameData.instance.etoTypeCount));

        // 残り時間の表示
        //uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);

        // 引数で指定した数の干支を生成する
        //StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
    }

    // 10でここから追加

    // 干支の画像を読み込んで配列から使用できるようにする
    // 発展でこのメソッド自体を削除
    //private IEnumerator LoadEtoSprites()
    //{
    // 配列の初期化(12個の画像が入るようにSprite型の配列を12個用意する)
    //etoSprites = new Sprite[(int)EtoType.Count];

    // Resources.LoadAllを行い、分割されている干支の画像を順番に全て読み込んで配列に代入
    //etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

    // ※　1つのファイルを12分割していない場合には以下の処理を使う
    //     12分割している場合には使用しない
    //for(int i = 0; i < etoSprites.Length; i++)
    //{
    //etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
    //}

    //yield break;
    //}

    // PreparateGameメソッド内にスキルボタンの設定を行う処理を追加(発展11で)
    public IEnumerator PreparateGame()
    {
        // ステートを準備中に変更
        gameState = GameState.Ready;

        // 残り時間の表示
        uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);

        // ゲームに登場させる干支の種類を設定する
        yield return StartCoroutine(SetUpEtoTypes(GameData.instance.etoTypeCount));

        // GameDataのselectedSkillTypeを渡して、スキルボタンに登録するメソッド(スキル実行時)を設定
        yield return StartCoroutine(SetUpSkill(GameData.instance.selectedSkillType));

        // 引数で指定した数の干支を生成する
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));

        // 発展13で追加
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Game);
    }
    
    /// <summary>
    /// ゲームに登場させる干支の種類を設定する
    /// </summary>
    /// <param name="typeCount">生成する干支の種類数</param>
    /// <returns></returns>
    private IEnumerator SetUpEtoTypes(int typeCount)
    {
        // 新しくリストを用意して初期化に合わせてetoDataListを複製して、干支の候補リストとする
        List<GameData.EtoData> candidateEtoDataList = new List<GameData.EtoData>(GameData.instance.etoDataList);

        // 発展6で処理を追加
        // 選択中の干支を探して生成する干支のリストに追加
        GameData.EtoData myEto = candidateEtoDataList.Find((x) => x.etoType == GameData.instance.selectedEtoData.etoType);
        selectedEtoDataList.Add(myEto);
        candidateEtoDataList.Remove(myEto);
        typeCount--;

        // 干支を指定数だけをランダムに選ぶ(干支の種類は重複させない)
        // while文は 条件式 を満たす限りループする。ここでは typeCount 変数の値が 0 よりも大きい間は while 内の処理が繰り返し実行される
        while (typeCount > 0)
        {
            // ランダムに数字を選ぶ
            int randomValue = Random.Range(0, candidateEtoDataList.Count);

            // 今回のゲームに生成する干支リストに追加
            selectedEtoDataList.Add(candidateEtoDataList[randomValue]);

            // 干支のリストから選択された干支の情報を削除(干支を重複させないため)
            candidateEtoDataList.Remove(candidateEtoDataList[randomValue]);

            // 選択した数を減らす
            typeCount--;

            // 処理を1フレーム待機させて再開する。よって毎フレームのループ処理になる
            yield return null;
        }
    }

    /// <summary>
    /// 干支を生成
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private IEnumerator CreateEtos(int count)
    {
        // 発展3で処理を追加
        // 干支の生成中はシャッフルボタンを押さないようにする
        uiManager.ActivateShuffleButton(false);

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
            // 発展で修正
            // 今回のゲームに登場する干支の中から、ランダムな干支を1つ選択
            //int randomValue = Random.Range(0, (int)EtoType.Count);
            int randomValue = Random.Range(0, selectedEtoDataList.Count);

            // 生成された干支の初期設定(干支の種類と干支の画像を引数を使ってEtoへ渡す)
            // 干支の初期設定
            //eto.SetUpEto((EtoType)randomValue, etoSprites[randomValue]);
            eto.SetUpEto(selectedEtoDataList[randomValue].etoType, selectedEtoDataList[randomValue].sprite);

            // etoListに追加
            etoList.Add(eto);

            // 0.03秒待って次の干支を生成
            yield return new WaitForSeconds(0.03f);
        }

        // 発展3で処理を追加
        // 干支の生成が終了したらシャッフルボタンを押せるようにする
        uiManager.ActivateShuffleButton(true);

        // gameStateが準備中のときだけゲームプレイ中に変更
        if (gameState == GameState.Ready)
        {
            gameState = GameState.Play;
        }
    }

    // 11でここから追加
    void Update()
    {
        // ゲームのプレイ中以外のgameStateでは処理を行わない
        if (gameState != GameState.Play)
        {
            return;
        }

        // 干支をつなげる処理
        if (Input.GetMouseButtonDown(0) && firstSelectEto == null)
        {
            // 干支を最初にドラッグした際の処理
            OnStartDrag();
        }
        // 13で追加
        else if (Input.GetMouseButtonUp(0))
        {
            // 干支のドラッグをやめた（指を離した）際の処理
            OnEndDrag();
        }
        // 12で追加
        else if (firstSelectEto != null)
        {
            // 干支のドラッグ（スワイプ）中の処理
            OnDragging();
        }

        // ゲームの残り時間のカウント処理
        timer += Time.deltaTime;

        // timerが１以上になったら
        if (timer >= 1)
        {
            // リセットして再度加算できるように
            timer = 0;

            // 残り時間をマイナス
            GameData.instance.gameTime--;

            //  残り時間がマイナスになったら
            if (GameData.instance.gameTime <= 0)
            {
                // 0で止める
                GameData.instance.gameTime = 0;

                // TODO  ゲーム終了を追加する
                //Debug.Log("ゲーム終了");

                // ゲーム終了を追加する
                StartCoroutine(GameUp());
            }

            // 残り時間の表示更新
            uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);
        }
    }

    /// <summary>
    /// 干支を最初にドラッグした際の処理
    /// </summary>
    private void OnStartDrag()
    {
        // 画面をタップした際の位置情報を、CameraクラスのScreenToWorldPointメソッドを利用してCanvas上の座標に変換
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // 干支がつながっている数を初期化
        linkCount = 0;

        // 変換した座標のコライダーを持つゲームオブジェクトがあるか確認
        if (hit.collider != null)
        {
            // ゲームオブジェクトがあった場合、そのゲームオブジェクトがEtoクラスを持っているかどうか確認
            if (hit.collider.gameObject.TryGetComponent(out Eto dragEto))
            {
                // Etoクラスを持っていた場合には、以下の処理を行う

                // 最初にドラッグした干支の情報を変数に代入
                firstSelectEto = dragEto;

                // 最後にドラッグした干支の情報を変数に代入(最初のドラッグなので、最後のドラッグも同じ干支)
                lastSelectEto = dragEto;

                // 最初にドラッグした干支の情報を代入 ＝ 後ほど、この情報を使ってつながる干支かどうかを判別する
                currentEtoType = dragEto.etoType;

                // 干支の状態が「選択中」であると更新
                dragEto.isSelected = true;

                // 干支に何番目に選択されているのか、通し番号を登録
                dragEto.num = linkCount;

                // 削除する対象の干支を登録するリストを初期化
                eraseEtoList = new List<Eto>();

                // ドラッグ中の干支を削除の対象としてリストに登録
                AddEraseEtoList(dragEto);
            }
        }
    }

    /// <summary>
    /// 干支のドラッグ（スワイプ）中処理
    /// </summary>
    private void OnDragging()
    {
        // OnStartDragメソッドと同じ処理で、指の位置をワールド座標に変換しRayを発射しその位置にあるコライダーを持つオブジェクトを取得してhit変数へ代入
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // Rayの戻り値があり(hit変数がnullではない)hit変数のゲームオブジェクトがEtoクラスを持っていたら
        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Eto dragEto))
        {
            // 現在選択中の干支の種類がnullなら処理は行わない
            if (currentEtoType == null)
            {
                return;
            }

            // dragEto変数の干支の種類が最初に選択した干支の種類と同じであり、最後にタップしている干支と現在の干支が違うオブジェクトであり、かつ、現在の干支がすでに「選択中」でなければ
            if (dragEto.etoType == currentEtoType && lastSelectEto != dragEto && !dragEto.isSelected)
            {
                // 現在タップしている干支の位置情報と最後にタップした干支の位置情報と比べて、差分の値(干支通しの距離)を取る
                float distance = Vector2.Distance(dragEto.transform.position, lastSelectEto.transform.position);

                // 干支同士の距離が設定値よりも小さければ(2つの干支が離れていなければ)、干支をつなげる
                if (distance < etoDistance)
                {
                    // 現在の干支を選択中にする
                    dragEto.isSelected = true;

                    // 最後に選択している干支を現在の干支に更新
                    lastSelectEto = dragEto;

                    // 干支のつながった数のカウントを1つ増やす
                    linkCount++;

                    // 干支に通し番号を設定
                    dragEto.num = linkCount;

                    // 削除リストに現在の干支を追加
                    AddEraseEtoList(dragEto);
                }
            }

            // 現在の干支の種類を確認(現在の干支(dragEtoの情報であれば他の情報でもよい。ちゃんと選択されているかの確認用))
            Debug.Log(dragEto.etoType);

            // 削除リストに2つ以上の干支が追加されている場合
            if (eraseEtoList.Count > 1)
            {
                // 現在の干支の通し番号を確認
                Debug.Log(dragEto.num);

                // 条件に合致する場合、削除リストから干支を除外する(ドラッグしたまま1つ前の干支の戻る場合、現在の干支を削除リストから除外する)
            　　if (eraseEtoList[linkCount - 1] != lastSelectEto && eraseEtoList[linkCount - 1].num == dragEto.num && dragEto.isSelected)
                {
                    // 選択中のボールを取り除く
                    RemoveEraseEtoList(lastSelectEto);

                    lastSelectEto.GetComponent<Eto>().isSelected = false;

                    // 最後のボールの情報を前のボールに戻す
                    lastSelectEto = dragEto;

                    // つながっている干支の数を減らす
                    linkCount--;
                }
            }
        }
    }

    /// <summary>
    /// 干支のドラッグをやめた（指を画面から離した）際の処理
    /// 発展11でメソッド内でスキルポイントを加算する処理を追加
    /// </summary>
    private void OnEndDrag()
    {
        // つながっている干支が3つ以上あったら削除する処理にうつる
        if (eraseEtoList.Count >= 3)
        {
            // 選択されている干支を消す
            for (int i = 0; i < eraseEtoList.Count; i++)
            {
                // 干支リストから取り除く
                etoList.Remove(eraseEtoList[i]);

                // 発展4で追加
                // 干支の削除演出エフェクト生成
                GameObject effect = Instantiate(eraseEffectPrefab, eraseEtoList[i].gameObject.transform);

                // エフェクトの位置をEtoSetTran内に変更(干支の子オブジェクトのままだと、干支が破棄されると同時にエフェクトも破棄されてしまうため)
                effect.transform.SetParent(etoSetTran);

                // 干支を削除
                Destroy(eraseEtoList[i].gameObject);

                //  発展13で追加
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Erase);
            }

            // スコアと消した干支の数の加算
            AddScores(currentEtoType, eraseEtoList.Count);

            // スキルポイントを加算
            uiManager.AddSkillPoint(eraseEtoList.Count);

            // 消した干支の数だけ新しい干支をランダムに生成
            StartCoroutine(CreateEtos(eraseEtoList.Count));

            // 削除リストをクリアする
            eraseEtoList.Clear();
        }
        else
        {
            // つながっている干支が2つ以下なら、削除はしない
            
            // 削除リストから、削除候補であった干支を取り除く
            for (int i = 0; i < eraseEtoList.Count; i++)
            {
                // 各干支の選択中の状態を解除する
                eraseEtoList[i].isSelected = false;

                // 干支の色の透明度を元の透明度に戻す
            }
        }

        // 次回の干支を消す処理のために、各変数の値を null にする
        firstSelectEto = null;
        lastSelectEto = null;
        currentEtoType = null;
    }

    /// <summary>
    /// 選択された干支を削除リストに追加
    /// </summary>
    /// <param name="dragEto"></param>
    private void AddEraseEtoList(Eto dragEto)
    {
        // 削除リストにドラッグ中の干支を追加
        eraseEtoList.Add(dragEto);

        // ドラッグ中の干支のアルファ値を0.5fにする(半透明にすることで、選択中であることをユーザーに伝える)
        ChangeEtoAlpha(dragEto, 0.5f);
    }

    private void RemoveEraseEtoList(Eto dragEto)
    {
        // 削除リストから削除
        eraseEtoList.Remove(dragEto);

        // 干支の透明度を元の値(1.0f)に戻す
        ChangeEtoAlpha(dragEto, 1.0f);

        // 干支の「選択中」の情報がtrueの場合
        if (dragEto.isSelected)
        {
            // falseにして選択中ではない状態に戻す
            dragEto.isSelected = false;
        }
    }

    /// <summary>
    /// 干支のアルファ値を変更
    /// </summary>
    /// <param name="dragEto"></param>
    /// <param name="alphaValue"></param>
    private void ChangeEtoAlpha(Eto dragEto, float alphaValue)
    {
        // 現在ドラッグしている干支のアルファ値を変更
        dragEto.imgEto.color = new Color(dragEto.imgEto.color.r, dragEto.imgEto.color.g, dragEto.imgEto.color.b, alphaValue);
    }

    /// <summary>
    /// スコアと消した干支の数を加算
    /// </summary>
    /// <param name="etoType">消した干支の種類</param>
    /// <param name="count">消した干支の数</param>
    private void AddScores(EtoType? etoType, int count)
    {
        // スコアを加算
        // ここから発展8で追加＆修正
        // 消した干支が選択されている干支かどうかを判定する変数。trueなら選択されている干支とする
        bool isChooseEto = false;
        
        if (etoType == GameData.instance.selectedEtoData.etoType)
        {
            // 選択している干支の場合にはスコアを多く加算　etoPoint * 消した干支の数　* etoRate
            GameData.instance.score += Mathf.CeilToInt(GameData.instance.etoPoint * count * GameData.instance.etoRate);
            isChooseEto = true;
        }
        else
        {
            // それ以外は etoPoint * 消した干支の数　を加算
            GameData.instance.score += GameData.instance.etoPoint * count;
        }

        // 消した干支の数を加算
        GameData.instance.eraseEtoCount += count;

        // スコア加算と画面の更新処理
        uiManager.UpdateDisplayScore(isChooseEto);
    }

    /// <summary>
    /// ゲーム終了処理
    /// 発展11でメソッド内でシャッフルボタンを非活性化する処理を削除し、新しくシャッフルとスキルの両ボタンの非活性化を追加
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameUp()
    {
        // 発展3で処理を追加
        // 発展11で削除
        // シャッフルボタンを非活性化して押せなくする
        //uiManager.ActivateShuffleButton(false);

        // シャッフルボタンとスキルボタンを非活性化して押せなくする
        uiManager.InActiveButtons();

        // gameStateをリザルトに変更する = Updateメソッドが動かなくなる
        gameState = GameState.Result;

        yield return new WaitForSeconds(1.5f);

        // TODO 処理を実装します
        yield return StartCoroutine(MoveResultPopUp());

        // TODOリザルトの処理を実装する
        //Debug.Log("リザルトのポップアップを移動させます");
    }


    /// <summary>
    /// リザルトポップアップを画面内に移動　　　　　　　//　☆　メソッド内のTODO部分を変更して処理を実装　☆
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveResultPopUp()
    {
        // DoTweenの機能を使って、ResultPopUpゲームオブジェクトを画面外から画面内に移動させる(Y座標を0に)
        resultPopUp.transform.DOMoveY(0, 1.0f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // リザルト表示(スコアと消した干支の数を渡す)TODOを実装する
                resultPopUp.DisplayResult(GameData.instance.score, GameData.instance.eraseEtoCount);

                // TODO 移動完了後に、リザルト内容を表示
                // Debug.Log("リザルト内容を表示します");
            }
         );

        // 発展13で追加
        // SEを鳴らすまでの待機時間
        yield return new WaitForSeconds(0.5f);       // <= 待機時間は音源に合わせて適宜調整

        // ドラムロールのSE再生
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Result);

        yield return new WaitForSeconds(2.5f);       // <= 待機時間は音源に合わせて適宜調整

        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Result);
    }

    /// <summary>
    /// 最も数の多い干支のタイプをまとめて削除する
    /// </summary>
    public void DeleteMaxEtoType()
    {
        // Dictinaryの宣言と定義。干支のタイプとその数を代入できるようにする
        Dictionary<EtoType, int> dictionary = new Dictionary<EtoType, int>();

        // リストの中から干支のタイプごとにDictionaryの要素を作成(ここで5つの干支タイプごとにいくつ数があるかわかる)
        foreach (Eto eto in etoList)
        {
            if (dictionary.ContainsKey(eto.etoType))
            {
                // 既にある要素(干支のタイプ)の場合には数のカウントを加算
                dictionary[eto.etoType]++;
            }
            else
            {
                // まだ作られていない要素(干支のタイプ)の場合には新しく要素を作り、カウントを1とする
                dictionary.Add(eto.etoType, 1);
            }
        }

        // Debug
        foreach (KeyValuePair<EtoType, int> keyValuePair in dictionary)
        {
            Debug.Log("干支 : " + keyValuePair.Key + " 数 : " + keyValuePair.Value);
        }

        // Dictionaryを検索し、最も数の多い干支のタイプを見つけて、消す干支のタイプと数を決定
        EtoType maxEtoType = dictionary.OrderByDescending(x => x.Value).First().Key;
        int removeNum = dictionary.OrderByDescending(x => x.Value).First().Value;

        Debug.Log("消す干支のタイプ : " + maxEtoType + " 数 : " + removeNum);

        // 対象の干支を破壊
        for (int i = 0; i < etoList.Count; i ++)
        {
            if (etoList[i].etoType == maxEtoType)
            {
                Destroy(etoList[i].gameObject);
            }
        }

        // etoListから対象の干支を削除
        etoList.RemoveAll(x => x.etoType == maxEtoType);

        // 点数と消した干支への加算
        AddScores(maxEtoType, removeNum);

        // 破壊した干支の数だけ干支を生成
        StartCoroutine(CreateEtos(removeNum));
    }

    /// <summary>
    /// 選択されたスキルをボタンに登録
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

            // TODO スキルが増えた場合には追加する
        }
        return null;
    }

}
