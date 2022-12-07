using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownTruckManager : MonoBehaviour
{
    // �g���b�N�X�e�[�g
    public enum TruckState
    {
        Wait = 0,
        PutOnMove,
        Move,
        Destroy,
    }

    // �g���b�N�P�ʂ̏��
    public class TruckInfo
    {
        // �g���b�N�p
        public float SpownWaitTimeCount;            // �X�|�[���J�n�܂ł̎���
        public TruckState State = TruckState.Wait;  // �X�e�[�g
        public Transform Truck;                     // �g���b�N�̃g�����X�t�H�[��
        public float InitialPosX;                   // ���s�J�n���W
        public float TargetPosX;                    // ���s����W

        // �W�����v�p
        public bool IsJump = false;         // �W�����v����
        public float JumpTimeCount = 0.0f;  // �W�����v�̎c�莞��
        public List<SpownPlayerInfo> SpownPlayerInfoList = new List<SpownPlayerInfo>();    // �X�|�[������v���C���[�̏��

        public TruckInfo(float waitTime, Transform truck, float initialPosX, float targetPosX)
        {
            SpownWaitTimeCount = waitTime;
            Truck = truck;
            InitialPosX = initialPosX;
            TargetPosX = targetPosX;
        }
    }

    // �g���b�N���̃v���C���[�P�ʂ̏��
    public class SpownPlayerInfo
    {
        public int PlayerId;                    // �Ώۂ̃v���C���[ID
        public Vector2 Start = Vector2.zero;    // �W�����v�J�n�ʒu
        public Vector2 End = Vector2.zero;      // �W�����v�I���ʒu
        public Vector2 Half = Vector2.zero;     // �W�����v�̒��Ԉʒu

        public SpownPlayerInfo(int id)
        {
            PlayerId = id;
        }
    }

    [Header("�����X�|�[���֌W")]
    [SerializeField]
    private GameObject firstSpownTruckPrefab;   // �����X�|�[���p�g���b�N�̃v���n�u
    [SerializeField]
    private int firstSpownSortLayer;            // �����X�|�[�����̃v���C���[�̕`�揇

    [SerializeField]
    [Header("���X�|�[���֌W")]
    private GameObject respownTruckPrefab;      // ���X�|�[���p�g���b�N�̃v���n�u
    [SerializeField]
    private int respownSortLayer;               // ���X�|�[�����̃v���C���[�̕`�揇

    [SerializeField]
    [Header("�W�����v�֌W")]
    private Transform players;                  // �S�v���C���[�̐e�I�u�W�F�N�g
    [SerializeField]
    private float jumpTime;                     // �g���b�N�����яo���W�����v�̎���
    [SerializeField]
    private BattleFirstPositionDataBase firstPositionDataBase;  // �v���C���[�̃X�|�[���ʒu��ۑ������f�[�^�x�[�X
    [SerializeField]
    private int defaultSortLayer;               // �v���C���[�̒ʏ�̕`�揇

    [SerializeField]
    [Header("���̑�")]
    private float truckMoveSpeed = 1.0f;        // �g���b�N�̈ړ����x
    [SerializeField]
    private float spownWaitTime = 2.0f;         // �X�|�[���܂ł̑ҋ@����

    private Transform[] playerBodys;                    // �v���C���[�̃{�f�B�I�u�W�F�N�g�̃g�����X�t�H�[��
    private SpriteRenderer[] playerBodySpriteRenderer;  // �v���C���[�̃{�f�B�̃X�v���C�g�����_���[
    private List<TruckInfo> truckList;                  // �g���b�N�̃��X�g
    private Vector3 firstSpownTruckInitialPos;          // �����X�|�[���p�g���b�N�̑��s�J�n���W
    private Vector3 firstSpownTruckTargetPos;           // �����X�|�[���p�g���b�N�̑��s����W
    private Vector3 respownTruckInitialPos;             // ���X�|�[���p�g���b�N�̑��s�J�n���W
    private Vector3 respownTruckTargetPos;              // ���X�|�[���p�g���b�N�̑��s����W
    private Transform truckParent;                      // �g���b�N���܂Ƃ߂�e�I�u�W�F�N�g
    private Vector3 moveAngle;                          // �g���b�N�̈ړ�����

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

        // �����X�|�[���̏���������
        SetSpownAll();
    }

    void Update()
    {
        for (int i = 0; i < truckList.Count; i++)
        {
            // �g���b�N�̃X�e�[�g�ɉ������������s��
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
                    // �ς�ł����v���C���[�̃W�����v���I�����Ă�����폜����
                    if (!truckList[i].IsJump)
                    {
                        truckList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    break;
            }

            // �W�����v�̋Ȑ��ړ�����
            if (truckList[i].IsJump)
            {
                JumpMoveAction(truckList[i]);
            }
        }
    }

    /// <summary>
    /// �X�|�[���J�n�҂�����
    /// </summary>
    private void WaitAction(TruckInfo info)
    {
        if (info.SpownWaitTimeCount <= 0.0f)
        {
            info.State = TruckState.PutOnMove;

            info.SpownWaitTimeCount = 0.0f;

            // ���X�|�[���̏ꍇ�̂݌��ʉ��Đ�
            if (info.SpownPlayerInfoList.Count == 1)
            {
                AudioManager.Instance.PlaySe("�N���N�V����");
            }
        }
        else
        {
            info.SpownWaitTimeCount -= Time.deltaTime;
        }
    }

    /// <summary>
    /// �X�|�[��(�J�n�`�v���C���[����)����
    /// </summary>
    private void PutOnMoveAction(TruckInfo info)
    {
        // ���s�͈͂̒��Ԓn�_�܂ł̋���
        float toHalfDistance = (info.InitialPosX + info.TargetPosX) * 0.5f - info.Truck.position.x;
        // ���̃t���[���̈ړ�����
        float moveLength = truckMoveSpeed * Time.deltaTime;

        if (toHalfDistance <= moveLength)
        {//=====�ړ�������蒆�Ԓn�_�܂ł̋������������ꍇ
            info.Truck.position += moveAngle * toHalfDistance;
            info.State = TruckState.Move;
            // �v���C���[�𕪗��̏���������
            SetJumpParam(info);

            // �G�t�F�N�g�A���ʉ������ꂼ��Đ�
            if (info.SpownPlayerInfoList.Count == 1)
            {
                EffectContainer.Instance.PlayEffect("���X�|�[��", info.Truck.position + new Vector3(-1.0f, 0.0f, 0.0f));
                AudioManager.Instance.PlaySe("���X�|�[�����̉�");
            }
            else
            {
                EffectContainer.Instance.PlayEffect("�X�|�[��", info.Truck.position + new Vector3(-1.0f, 0.0f, 0.0f));
            }
        }
        else
        {//=====�ړ�������蒆�Ԓn�_�܂ł̋������傫���ꍇ
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
    /// �X�|�[��(�v���C���[�����`�I��)����
    /// </summary>
    private void MoveAction(TruckInfo info)
    {
        // ���s��n�_�܂ł̋���
        float toTargetDistance = info.TargetPosX - info.Truck.position.x;
        // ���̃t���[���̈ړ�����
        float moveLength = truckMoveSpeed * Time.deltaTime;

        if (toTargetDistance <= moveLength)
        {//=====�ړ�������葖�s��n�_�܂ł̋������������ꍇ
            info.Truck.position += moveAngle * toTargetDistance;
            info.State = TruckState.Destroy;
        }
        else
        {//=====�ړ�������葖�s��n�_�܂ł̋������傳���ꍇ
            info.Truck.position += moveAngle * moveLength;
        }
    }

    /// <summary>
    /// �X�|�[���n�_�ւ̃W�����v����
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
        //=====�S�v���C���[���p�����[�^��ݒ肷��
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
    /// �X�|�[���n�_�ւ̃W�����v����
    /// </summary>
    private void JumpMoveAction(TruckInfo info)
    {
        // �W�����v�̐i�s�������v�Z
        float rate = (jumpTime - info.JumpTimeCount) / jumpTime;
        
        // ���W���x�W�G�Ȑ��ōX�V
        for (int i = 0; i < info.SpownPlayerInfoList.Count; i++)
        {
            SpownPlayerInfo jumpPlayerInfo = info.SpownPlayerInfoList[i];
            playerBodys[jumpPlayerInfo.PlayerId].position = CalcLarpPoint(
                jumpPlayerInfo.Start, jumpPlayerInfo.Half, jumpPlayerInfo.End, rate);
        }

        info.JumpTimeCount -= Time.deltaTime;

        // �W�����v�������I��������
        if (info.JumpTimeCount <= 0.0f)
        {
            info.JumpTimeCount = 0.0f;
            info.IsJump = false;
            //=====�����������s��
            for (int i = 0; i < info.SpownPlayerInfoList.Count; i++)
            {
                int playerId = info.SpownPlayerInfoList[i].PlayerId;
                playerBodySpriteRenderer[playerId].sortingOrder = defaultSortLayer;
                players.GetChild(playerId).GetComponent<PlayerController>().Revival();
            }
        }
    }

    /// <summary>
    /// �x�W�G�Ȑ����̂ǂ̈ʒu���ǂ����v�Z����
    /// </summary>
    /// <param name="start"> �J�n�n�_ </param>
    /// <param name="half"> ���Ԓn�_(�ō����x�n�_) </param>
    /// <param name="end"> �I���n�_ </param>
    /// <param name="rate"> �i�s���� </param>
    private Vector2 CalcLarpPoint(Vector2 start, Vector2 half, Vector2 end, float rate)
    {
        Vector2 a = Vector2.Lerp(start, half, rate);
        Vector2 b = Vector2.Lerp(half, end, rate);
        return Vector2.Lerp(a, b, rate);
    }

    /// <summary>
    /// ���X�|�[���\��
    /// </summary>
    public void SetSpown(int playerIndex)
    {
        Transform truck;

        // �g���b�N�𐶐��E������
        truck = Instantiate(respownTruckPrefab, truckParent).transform;
        truck.position = respownTruckInitialPos;
        truckList.Add(new TruckInfo(spownWaitTime, truck,
            respownTruckInitialPos.x, respownTruckTargetPos.x));

        // �g���b�N�ɕt������v���C���[�̏���ǉ�
        truckList[truckList.Count - 1].SpownPlayerInfoList.Add(new SpownPlayerInfo(playerIndex));
        // �v���C���[�̕`�揇��ύX
        playerBodySpriteRenderer[playerIndex].sortingOrder = respownSortLayer;
    }

    /// <summary>
    /// �����X�|�[���\��
    /// </summary>
    public void SetSpownAll()
    {
        Transform truck;

        // �g���b�N�𐶐��E������
        truck = Instantiate(firstSpownTruckPrefab, truckParent).transform;
        truck.position = firstSpownTruckInitialPos;
        truckList.Add(new TruckInfo(0.0f, truck,
            firstSpownTruckInitialPos.x, firstSpownTruckTargetPos.x));

        //=====�g���b�N�ɕt������v���C���[�̏���ǉ�
        for (int i = 0; i < players.childCount; i++)
        {
            if (!DeviceManager.Instance.GetIsConnect(i)) { continue; }

            truckList[truckList.Count - 1].SpownPlayerInfoList.Add(new SpownPlayerInfo(i));
            // �v���C���[�̕`�揇��ύX
            playerBodySpriteRenderer[i].sortingOrder = firstSpownSortLayer;
        }
    }
}
