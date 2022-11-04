using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownTruckManager : MonoBehaviour
{
    public enum TruckState
    {
        Wait = 0,
        PutOnMove,
        Move,
        Destroy,
    }

    public class TruckInfo
    {
        public float SpownWaitTimeCount;
        public TruckState State = TruckState.Wait;
        public Transform Truck;
        public JumpInfo JumpInfo;
        public float InitialPosX;
        public float TargetPosX;

        public TruckInfo(float waitTime, Transform truck, float initialPosX, float targetPosX)
        {
            SpownWaitTimeCount = waitTime;
            Truck = truck;
            InitialPosX = initialPosX;
            TargetPosX = targetPosX;
        }
    }

    public class JumpInfo
    {
        public bool IsJump = false;
        public float JumpTimeCount = 0.0f;
        public List<JumpPlayerInfo> JumpPlayerInfoList = new List<JumpPlayerInfo>();
    }

    public class JumpPlayerInfo
    {
        public int PlayerId;
        public Vector2 Start = Vector2.zero;
        public Vector2 End = Vector2.zero;
        public Vector2 Half = Vector2.zero;

        public JumpPlayerInfo(int id)
        {
            PlayerId = id;
        }
    }

    [Header("初期スポーン関係")]
    [SerializeField]
    private GameObject firstSpownTruckPrefab;
    [SerializeField]
    private int firstSpownSortLayer;

    [SerializeField]
    [Header("リスポーン関係")]
    private GameObject respownTruckPrefab;
    [SerializeField]
    private int respownSortLayer;

    [SerializeField]
    [Header("ジャンプ関係")]
    private Transform players;
    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private BattleFirstPositionDataBase firstPositionDataBase;
    [SerializeField]
    private float invincibleTime = 2.0f;
    [SerializeField]
    private int defaultSortLayer;

    [SerializeField]
    [Header("その他")]
    private float truckMoveSpeed = 1.0f;
    [SerializeField]
    private float spownWaitTime = 2.0f;

    private Transform[] playerBodys;
    private SpriteRenderer[] playerSpriteRenderer;
    private Vector3 firstSpownTruckInitialPos;
    private Vector3 firstSpownTruckTargetPos;
    private Vector3 respownTruckInitialPos;
    private Vector3 respownTruckTargetPos;
    private Transform truckParent;
    private Vector3 moveAngle;
    private List<TruckInfo> truckList = new List<TruckInfo>();

    void Start()
    {
        playerBodys = new Transform[players.childCount];
        playerSpriteRenderer = new SpriteRenderer[players.childCount];
        for (int i = 0; i < players.childCount; i++)
        {
            playerBodys[i] = players.GetChild(i).GetChild(0);
            playerSpriteRenderer[i] = playerBodys[i].GetChild(0).GetComponent<SpriteRenderer>();
        }
        firstSpownTruckInitialPos = transform.GetChild(0).position;
        firstSpownTruckTargetPos = transform.GetChild(1).position;
        respownTruckInitialPos = transform.GetChild(2).position;
        respownTruckTargetPos = transform.GetChild(3).position;
        truckParent = transform.GetChild(4);
        moveAngle = Vector2.right;

        SetSpownAll();
    }

    void Update()
    {
        for (int i = 0; i < truckList.Count; i++)
        {
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
                    if (!truckList[i].JumpInfo.IsJump)
                    {
                        truckList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    break;
            }

            if (truckList[i].JumpInfo.IsJump)
            {
                JumpMoveAction(truckList[i]);
            }
        }
    }

    private void WaitAction(TruckInfo info)
    {
        if (info.SpownWaitTimeCount <= 0.0f)
        {
            info.State = TruckState.PutOnMove;

            info.SpownWaitTimeCount = 0.0f;

            if (info.JumpInfo.JumpPlayerInfoList.Count == 1)
            {
                AudioManager.Instance.PlaySe("クラクション");
            }
        }
        else
        {
            info.SpownWaitTimeCount -= Time.deltaTime;
        }
    }

    private void PutOnMoveAction(TruckInfo info)
    {
        float toHalfDistance = (info.InitialPosX + info.TargetPosX) * 0.5f - info.Truck.position.x;
        float moveLength = truckMoveSpeed * Time.deltaTime;

        if (toHalfDistance <= moveLength)
        {
            info.Truck.position += moveAngle * toHalfDistance;
            info.State = TruckState.Move;
            SetJumpParam(info);

            if (info.JumpInfo.JumpPlayerInfoList.Count == 1)
            {
                //EffectContainer.Instance.PlayEffect("スポーン", info.truck.position + new Vector3(-1.0f, 0.0f, 0.0f));
            }
            else
            {
                //EffectContainer.Instance.PlayEffect("リスポーン", info.truck.position + new Vector3(-1.0f, 0.0f, 0.0f));
                //AudioManager.Instance.PlaySe("リスポーン時の音");
            }
        }
        else
        {
            info.Truck.position += moveAngle * moveLength;
        }
    }

    private void MoveAction(TruckInfo info)
    {
        float toTargetDistance = info.TargetPosX - info.Truck.position.x;
        float moveLength = truckMoveSpeed * Time.deltaTime;

        if (toTargetDistance <= moveLength)
        {
            info.Truck.position += moveAngle * toTargetDistance;
            info.State = TruckState.Destroy;
        }
        else
        {
            info.Truck.position += moveAngle * moveLength;
        }
    }

    private void SetJumpParam(TruckInfo info)
    {
        //int stageId = BattleSumoManager.StageId;
        int stageId = 0;
        //int playerNumId = BattleSumoManager.
        int playerNumId = 2;
        FirstPosition jumpPositions = firstPositionDataBase.BattleStageInfos[stageId].FirstPositions[playerNumId];

        info.JumpInfo.IsJump = true;
        info.JumpInfo.JumpTimeCount = jumpTime;
        for (int i = 0; i < info.JumpInfo.JumpPlayerInfoList.Count; i++)
        {
            int playerId = info.JumpInfo.JumpPlayerInfoList[i].PlayerId;
            Vector3 start = playerBodys[playerId].position;
            Vector3 end = jumpPositions.Position[playerId];
            Vector3 half = end - start * 0.5f + start;

            info.JumpInfo.JumpPlayerInfoList[i].Start = start;
            info.JumpInfo.JumpPlayerInfoList[i].End = end;
            info.JumpInfo.JumpPlayerInfoList[i].Half = half;
            info.JumpInfo.JumpPlayerInfoList[i].Half.y += firstPositionDataBase.BattleStageInfos[stageId].JumpRerativeHeight;
        }
    }

    private void JumpMoveAction(TruckInfo info)
    {
        float rate = (jumpTime - info.JumpInfo.JumpTimeCount) / jumpTime;

        for (int i = 0; i < info.JumpInfo.JumpPlayerInfoList.Count; i++)
        {
            JumpPlayerInfo jumpPlayerInfo = info.JumpInfo.JumpPlayerInfoList[i];
            playerBodys[jumpPlayerInfo.PlayerId].position = CalcLarpPoint(jumpPlayerInfo.Start, jumpPlayerInfo.Half, jumpPlayerInfo.End, rate);
        }

        info.JumpInfo.JumpTimeCount -= Time.deltaTime;

        if (info.JumpInfo.JumpTimeCount <= 0.0f)
        {
            info.JumpInfo.JumpTimeCount = 0.0f;
            info.JumpInfo.IsJump = false;
            for (int i = 0; i < info.JumpInfo.JumpPlayerInfoList.Count; i++)
            {
                int playerId = info.JumpInfo.JumpPlayerInfoList[i].PlayerId;
                playerSpriteRenderer[playerId].sortingOrder = defaultSortLayer;
                players.GetChild(playerId).GetComponent<PlayerController>().Revival();
            }
        }
    }

    private Vector2 CalcLarpPoint(Vector2 start, Vector2 half, Vector2 end, float rate)
    {
        Vector2 a = Vector2.Lerp(start, half, rate);
        Vector2 b = Vector2.Lerp(half, end, rate);
        return Vector2.Lerp(a, b, rate);
    }

    public void SetSpown(int playerIndex)
    {
        Transform truck;

        truck = Instantiate(respownTruckPrefab, truckParent).transform;
        truck.position = respownTruckInitialPos;
        truckList.Add(new TruckInfo(spownWaitTime, truck,
            respownTruckInitialPos.x, respownTruckTargetPos.x));
        truckList[truckList.Count - 1].JumpInfo.JumpPlayerInfoList.Add(new JumpPlayerInfo(playerIndex));
        playerSpriteRenderer[playerIndex].sortingOrder = respownSortLayer;
    }

    public void SetSpownAll()
    {
        Transform truck;

        truck = Instantiate(firstSpownTruckPrefab, truckParent).transform;
        truckList.Add(new TruckInfo(spownWaitTime, truck,
            firstSpownTruckInitialPos.x, firstSpownTruckTargetPos.x));
        for (int i = 0; i < players.childCount; i++)
        {
            if (!BattleSumoManager.IsPlayerJoin[i]) { continue; }

            truckList[truckList.Count - 1].JumpInfo.JumpPlayerInfoList.Add(new JumpPlayerInfo(i));
            playerSpriteRenderer[i].sortingOrder = firstSpownSortLayer;
        }
        truck.position = firstSpownTruckInitialPos;
    }
}
