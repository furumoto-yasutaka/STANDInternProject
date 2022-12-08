using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownTruckManager : MonoBehaviour
{
    // トラックステート
    public enum TruckState
    {
        Wait = 0,
        PutOnMove,
        Move,
        Destroy,
    }

    // トラック単位の情報
    public class TruckInfo
    {
        // トラック用
        public float SpownWaitTimeCount;            // スポーン開始までの時間
        public TruckState State = TruckState.Wait;  // ステート
        public Transform Truck;                     // トラックのトランスフォーム
        public float InitialPosX;                   // 走行開始座標
        public float TargetPosX;                    // 走行先座標

        // ジャンプ用
        public bool IsJump = false;         // ジャンプ中か
        public float JumpTimeCount = 0.0f;  // ジャンプの残り時間
        public List<SpownPlayerInfo> SpownPlayerInfoList = new List<SpownPlayerInfo>();    // スポーンするプレイヤーの情報

        public TruckInfo(float waitTime, Transform truck, float initialPosX, float targetPosX)
        {
            SpownWaitTimeCount = waitTime;
            Truck = truck;
            InitialPosX = initialPosX;
            TargetPosX = targetPosX;
        }
    }

    // トラック内のプレイヤー単位の情報
    public class SpownPlayerInfo
    {
        public int PlayerId;                    // 対象のプレイヤーID
        public Vector2 Start = Vector2.zero;    // ジャンプ開始位置
        public Vector2 End = Vector2.zero;      // ジャンプ終了位置
        public Vector2 Half = Vector2.zero;     // ジャンプの中間位置

        public SpownPlayerInfo(int id)
        {
            PlayerId = id;
        }
    }

    [Header("初期スポーン関係")]
    [SerializeField]
    private GameObject firstSpownTruckPrefab;   // 初期スポーン用トラックのプレハブ
    [SerializeField]
    private int firstSpownSortLayer;            // 初期スポーン時のプレイヤーの描画順

    [SerializeField]
    [Header("リスポーン関係")]
    private GameObject respownTruckPrefab;      // リスポーン用トラックのプレハブ
    [SerializeField]
    private int respownSortLayer;               // リスポーン時のプレイヤーの描画順

    [SerializeField]
    [Header("ジャンプ関係")]
    private Transform players;                  // 全プレイヤーの親オブジェクト
    [SerializeField]
    private float jumpTime;                     // トラックから飛び出すジャンプの時間
    [SerializeField]
    private BattleFirstPositionDataBase firstPositionDataBase;  // プレイヤーのスポーン位置を保存したデータベース
    [SerializeField]
    private int defaultSortLayer;               // プレイヤーの通常の描画順

    [SerializeField]
    [Header("その他")]
    private float truckMoveSpeed = 1.0f;        // トラックの移動速度
    [SerializeField]
    private float spownWaitTime = 2.0f;         // スポーンまでの待機時間

    private Transform[] playerBodys;                    // プレイヤーのボディオブジェクトのトランスフォーム
    private SpriteRenderer[] playerBodySpriteRenderer;  // プレイヤーのボディのスプライトレンダラー
    private List<TruckInfo> truckList;                  // トラックのリスト
    private Vector3 firstSpownTruckInitialPos;          // 初期スポーン用トラックの走行開始座標
    private Vector3 firstSpownTruckTargetPos;           // 初期スポーン用トラックの走行先座標
    private Vector3 respownTruckInitialPos;             // リスポーン用トラックの走行開始座標
    private Vector3 respownTruckTargetPos;              // リスポーン用トラックの走行先座標
    private Transform truckParent;                      // トラックをまとめる親オブジェクト
    private Vector3 moveAngle;                          // トラックの移動方向

    void Start()
    {
        playerBodys = new Transform[players.childCount];
        playerBodySpriteRenderer = new SpriteRenderer[players.childCount];
        truckList = new List<TruckInfo>();
        for (int i = 0; i < players.childCount; i++)
        {
            playerBodys[i] = players.GetChild(i).GetChild(0);
            playerBodySpriteRenderer[i] = playerBodys[i].GetChild(0).GetComponent<SpriteRenderer>();
        }
        firstSpownTruckInitialPos = transform.GetChild(0).position;
        firstSpownTruckTargetPos = transform.GetChild(1).position;
        respownTruckInitialPos = transform.GetChild(2).position;
        respownTruckTargetPos = transform.GetChild(3).position;
        truckParent = transform.GetChild(4);
        moveAngle = Vector2.right;

        // 初期スポーンの準備をする
        SetSpownAll();
    }

    void Update()
    {
        for (int i = 0; i < truckList.Count; i++)
        {
            // トラックのステートに応じた処理を行う
            switch (truckList[i].State)
            {
                case TruckState.Wait:
                    WaitAction(truckList[i]);
                    break;
                case TruckState.PutOnMove:
                    PutOnMoveAction(truckList[i]);
                    break;
                case TruckState.Move:
                    MoveAction(truckList[i]);
                    break;
                case TruckState.Destroy:
                    // 積んでいたプレイヤーのジャンプが終了していたら削除する
                    if (!truckList[i].IsJump)
                    {
                        truckList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    break;
            }

            // ジャンプの曲線移動処理
            if (truckList[i].IsJump)
            {
                JumpMoveAction(truckList[i]);
            }
        }
    }

    /// <summary>
    /// スポーン開始待ち処理
    /// </summary>
    private void WaitAction(TruckInfo info)
    {
        if (info.SpownWaitTimeCount <= 0.0f)
        {
            info.State = TruckState.PutOnMove;

            info.SpownWaitTimeCount = 0.0f;

            // リスポーンの場合のみ効果音再生
            if (info.SpownPlayerInfoList.Count == 1)
            {
                AudioManager.Instance.PlaySe("クラクション");
            }
        }
        else
        {
            info.SpownWaitTimeCount -= Time.deltaTime;
        }
    }

    /// <summary>
    /// スポーン(開始～プレイヤー分離)処理
    /// </summary>
    private void PutOnMoveAction(TruckInfo info)
    {
        // 走行範囲の中間地点までの距離
        float toHalfDistance = (info.InitialPosX + info.TargetPosX) * 0.5f - info.Truck.position.x;
        // このフレームの移動距離
        float moveLength = truckMoveSpeed * Time.deltaTime;

        if (toHalfDistance <= moveLength)
        {//=====移動距離より中間地点までの距離が小さい場合
            info.Truck.position += moveAngle * toHalfDistance;
            info.State = TruckState.Move;
            // プレイヤーを分離の準備をする
            SetJumpParam(info);

            // エフェクト、効果音をそれぞれ再生
            if (info.SpownPlayerInfoList.Count == 1)
            {
                EffectContainer.Instance.PlayEffect("リスポーン", info.Truck.position + new Vector3(-1.0f, 0.0f, 0.0f));
                AudioManager.Instance.PlaySe("リスポーン時の音");
            }
            else
            {
                EffectContainer.Instance.PlayEffect("スポーン", info.Truck.position + new Vector3(-1.0f, 0.0f, 0.0f));
            }
        }
        else
        {//=====移動距離より中間地点までの距離が大きい場合
            info.Truck.position += moveAngle * moveLength;

            if (info.SpownPlayerInfoList.Count == 1)
            {
                for (int i = 0; i < info.SpownPlayerInfoList.Count; i++)
                {
                    playerBodys[info.SpownPlayerInfoList[i].PlayerId].position = info.Truck.position + new Vector3(-1.0f, 0.0f, 0.0f);
                }
            }
            else
            {
                for (int i = 0; i < info.SpownPlayerInfoList.Count; i++)
                {
                    playerBodys[info.SpownPlayerInfoList[i].PlayerId].position = info.Truck.position + new Vector3(i * 0.666f, 0.0f, 0.0f);
                }
            }
        }
    }

    /// <summary>
    /// スポーン(プレイヤー分離～終了)処理
    /// </summary>
    private void MoveAction(TruckInfo info)
    {
        // 走行先地点までの距離
        float toTargetDistance = info.TargetPosX - info.Truck.position.x;
        // このフレームの移動距離
        float moveLength = truckMoveSpeed * Time.deltaTime;

        if (toTargetDistance <= moveLength)
        {//=====移動距離より走行先地点までの距離が小さい場合
            info.Truck.position += moveAngle * toTargetDistance;
            info.State = TruckState.Destroy;
        }
        else
        {//=====移動距離より走行先地点までの距離が大さい場合
            info.Truck.position += moveAngle * moveLength;
        }
    }

    /// <summary>
    /// スポーン地点へのジャンプ準備
    /// </summary>
    private void SetJumpParam(TruckInfo info)
    {
        //int stageId = BattleSumoManager.StageId;
        int stageId = 0;
        //int playerNumId = BattleSumoManager.
        int playerNumId = 2;
        FirstPosition jumpPositions = firstPositionDataBase.BattleStageInfos[stageId].FirstPositions[playerNumId];

        info.IsJump = true;
        info.JumpTimeCount = jumpTime;
        //=====全プレイヤー分パラメータを設定する
        for (int i = 0; i < info.SpownPlayerInfoList.Count; i++)
        {
            int playerId = info.SpownPlayerInfoList[i].PlayerId;
            Vector3 start = playerBodys[playerId].position;
            Vector3 end = jumpPositions.Position[playerId];
            Vector3 half = end - start * 0.5f + start;

            info.SpownPlayerInfoList[i].Start = start;
            info.SpownPlayerInfoList[i].End = end;
            info.SpownPlayerInfoList[i].Half = half;
            info.SpownPlayerInfoList[i].Half.y = end.y + firstPositionDataBase.BattleStageInfos[stageId].JumpRerativeHeight;
        }
    }

    /// <summary>
    /// スポーン地点へのジャンプ処理
    /// </summary>
    private void JumpMoveAction(TruckInfo info)
    {
        // ジャンプの進行割合を計算
        float rate = (jumpTime - info.JumpTimeCount) / jumpTime;
        
        // 座標をベジエ曲線で更新
        for (int i = 0; i < info.SpownPlayerInfoList.Count; i++)
        {
            SpownPlayerInfo jumpPlayerInfo = info.SpownPlayerInfoList[i];
            playerBodys[jumpPlayerInfo.PlayerId].position = CalcLarpPoint(
                jumpPlayerInfo.Start, jumpPlayerInfo.Half, jumpPlayerInfo.End, rate);
        }

        info.JumpTimeCount -= Time.deltaTime;

        // ジャンプ処理が終了したら
        if (info.JumpTimeCount <= 0.0f)
        {
            info.JumpTimeCount = 0.0f;
            info.IsJump = false;
            //=====復活処理を行う
            for (int i = 0; i < info.SpownPlayerInfoList.Count; i++)
            {
                int playerId = info.SpownPlayerInfoList[i].PlayerId;
                playerBodySpriteRenderer[playerId].sortingOrder = defaultSortLayer;
                players.GetChild(playerId).GetComponent<PlayerController>().Revival();
            }
        }
    }

    /// <summary>
    /// ベジエ曲線内のどの位置かどうか計算する
    /// </summary>
    /// <param name="start"> 開始地点 </param>
    /// <param name="half"> 中間地点(最高高度地点) </param>
    /// <param name="end"> 終了地点 </param>
    /// <param name="rate"> 進行割合 </param>
    private Vector2 CalcLarpPoint(Vector2 start, Vector2 half, Vector2 end, float rate)
    {
        Vector2 a = Vector2.Lerp(start, half, rate);
        Vector2 b = Vector2.Lerp(half, end, rate);
        return Vector2.Lerp(a, b, rate);
    }

    /// <summary>
    /// リスポーン予約
    /// </summary>
    public void SetSpown(int playerIndex)
    {
        Transform truck;

        // トラックを生成・初期化
        truck = Instantiate(respownTruckPrefab, truckParent).transform;
        truck.position = respownTruckInitialPos;
        truckList.Add(new TruckInfo(spownWaitTime, truck,
            respownTruckInitialPos.x, respownTruckTargetPos.x));

        // トラックに付随するプレイヤーの情報を追加
        truckList[truckList.Count - 1].SpownPlayerInfoList.Add(new SpownPlayerInfo(playerIndex));
        // プレイヤーの描画順を変更
        playerBodySpriteRenderer[playerIndex].sortingOrder = respownSortLayer;
    }

    /// <summary>
    /// 初期スポーン予約
    /// </summary>
    public void SetSpownAll()
    {
        Transform truck;

        // トラックを生成・初期化
        truck = Instantiate(firstSpownTruckPrefab, truckParent).transform;
        truck.position = firstSpownTruckInitialPos;
        truckList.Add(new TruckInfo(0.0f, truck,
            firstSpownTruckInitialPos.x, firstSpownTruckTargetPos.x));

        //=====トラックに付随するプレイヤーの情報を追加
        for (int i = 0; i < players.childCount; i++)
        {
            if (!DeviceManager.Instance.GetIsConnect(i)) { continue; }

            truckList[truckList.Count - 1].SpownPlayerInfoList.Add(new SpownPlayerInfo(i));
            // プレイヤーの描画順を変更
            playerBodySpriteRenderer[i].sortingOrder = firstSpownSortLayer;
        }
    }
}
