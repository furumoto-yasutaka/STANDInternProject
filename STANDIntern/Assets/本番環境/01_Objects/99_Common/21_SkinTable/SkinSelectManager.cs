using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SkinSelectManager : InputLockElement
{
    public class PlayerInfo
    {
        public int playerId;
        public int selectSkin;
        public bool isSkinSubmit = false;
        public bool isSelectAnimation = false;
        public InputState inputState = InputState.None;
        public float continueCount = 0.0f;

        public Animator Animator;
        public Image SkinPreview;
        public Image SkinPreviewBack;

        public PlayerInfo(int i, int v, Animator animator, Image preview, Image previewBack)
        {
            playerId = i;
            selectSkin = v;
            Animator = animator;
            SkinPreview = preview;
            SkinPreviewBack = previewBack;
        }
    }

    public enum InputState
    {
        None = 0,
        Wait,
        Interval,
    }

    public enum InputPattern
    {
        None = 0,
        Plus = 1 << 0,
        Minus = 1 << 1,
        Submit = 1 << 2,
        Cancel = 1 << 3,
    }

    [SerializeField]
    private PlayerSkinDataBase playerSkinDataBase;
    private bool[] isCanSelect;

    private List<PlayerInfo> playerInfo = new List<PlayerInfo>();
    private List<PlayerInfo> playerDeleteInfo = new List<PlayerInfo>();
    private int inputPattern = (int)InputPattern.None;
    private float inputMoveThreshold = 0.7f;
    private float continueWaitTime = 0.5f;
    private float continueInterval = 0.2f;

    void Start()
    {
        isCanSelect = new bool[playerSkinDataBase.PlayerSkinInfos.Length];
        for (int i = 0; i < playerSkinDataBase.PlayerSkinInfos.Length; i++)
        {
            isCanSelect[i] = true;
        }

        for (int i = 0; i < DeviceManager.Instance.deviceCount; i++)
        {
            int deviceId = DeviceManager.Instance.GetDeviceFromSystemInput(i).deviceId;
            int playerId = DeviceManager.Instance.IndexOfPlayerNum(deviceId);
            Transform tableParent = transform.GetChild(playerId).GetChild(0);
            playerInfo.Add(new PlayerInfo(playerId, 0,
                tableParent.GetComponent<Animator>(),
                tableParent.GetChild(0).GetComponent<Image>(),
                tableParent.GetChild(1).GetComponent<Image>()));
        }
    }

    void Update()
    {
        if (!IsCanInput) { return; }

        foreach (PlayerInfo info in playerInfo)
        {
            if (DeviceManager.Instance.GetIsConnect(info.playerId))
            {
                CheckInput(info.playerId);
                Execute(info);

                inputPattern = (int)InputPattern.None;
            }
            else
            {
                transform.GetChild(info.playerId).gameObject.SetActive(false);
                playerDeleteInfo.Add(info);
            }
        }

        foreach (PlayerInfo info in playerDeleteInfo)
        {
            playerInfo.Remove(info);
        }
        playerDeleteInfo.Clear();
    }

    private void CheckInput(int index)
    {
        Gamepad pad = DeviceManager.Instance.GetDeviceFromPlayerIndex(index);
        float stickHorizontal = pad.leftStick.ReadValue().x;
        float dpadHorizontal = pad.dpad.ReadValue().x;

        if (stickHorizontal <= -inputMoveThreshold ||
            dpadHorizontal <= -inputMoveThreshold)
        {
            inputPattern |= (int)InputPattern.Minus;
        }
        if (stickHorizontal >= inputMoveThreshold ||
            dpadHorizontal >= inputMoveThreshold)
        {
            inputPattern |= (int)InputPattern.Plus;
        }
        if (pad.bButton.wasPressedThisFrame)
        {
            inputPattern |= (int)InputPattern.Submit;
        }
        if (pad.aButton.wasPressedThisFrame)
        {
            inputPattern |= (int)InputPattern.Cancel;
        }
    }

    private void Execute(PlayerInfo info)
    {
        if ((inputPattern & (int)InputPattern.Plus) > 0 ||
                (inputPattern & (int)InputPattern.Minus) > 0)
        {
            switch (info.inputState)
            {
                case InputState.None:
                    if (!info.isSelectAnimation)
                    {
                        info.inputState = InputState.Wait;
                        info.continueCount = continueWaitTime;
                        MoveCursor(info);
                    }
                    break;
                case InputState.Wait:
                    if (info.continueCount <= 0.0f && !info.isSelectAnimation)
                    {
                        info.inputState = InputState.Interval;
                        info.continueCount = continueInterval;
                        MoveCursor(info);
                    }
                    else
                    {
                        info.continueCount -= Time.deltaTime;
                    }
                    break;
                case InputState.Interval:
                    if (info.continueCount <= 0.0f && !info.isSelectAnimation)
                    {
                        info.continueCount = continueInterval;
                        MoveCursor(info);
                    }
                    else
                    {
                        info.continueCount -= Time.deltaTime;
                    }
                    break;
            }
        }
        else
        {
            info.continueCount = 0.0f;
            info.inputState = InputState.None;
        }
        if ((inputPattern & (int)InputPattern.Submit) > 0)
        {
            Decition(info);
        }
        if ((inputPattern & (int)InputPattern.Cancel) > 0 &&
            info.isSkinSubmit)
        {
            Cancel(info);
        }
    }

    private void MoveCursor(PlayerInfo info)
    {
        int skinLength = playerSkinDataBase.PlayerSkinInfos.Length;
        if ((inputPattern & (int)InputPattern.Plus) > 0)
        {
            info.selectSkin = (info.selectSkin + 1) % skinLength;
            Select(info, info.selectSkin);
        }
        if ((inputPattern & (int)InputPattern.Minus) > 0)
        {
            info.selectSkin = (info.selectSkin - 1 + skinLength) % skinLength;
            Select(info, info.selectSkin);
        }
    }

    private void Decition(PlayerInfo info)
    {
        info.isSkinSubmit = true;
        isCanSelect[info.selectSkin] = false;
        CheckCanSubmitAll();
    }

    private void Cancel(PlayerInfo info)
    {
        info.isSkinSubmit = false;
        isCanSelect[info.selectSkin] = true;
        CheckCanSubmitAll();
    }

    private void Select(PlayerInfo info, int selectIndex)
    {
        //スプライト張替え
        // アニメーション
        // 決定することができないアイコンを付けるのか判断する関数を呼ぶ
        Debug.Log(selectIndex);

        info.SkinPreviewBack.sprite = info.SkinPreviewBack.sprite;
        info.SkinPreview.sprite = playerSkinDataBase.PlayerSkinInfos[selectIndex].Normal;
        info.SkinPreviewBack.color = new Color(info.SkinPreviewBack.color.r, info.SkinPreviewBack.color.g, info.SkinPreviewBack.color.b, 1.0f);
        info.SkinPreview.color = new Color(info.SkinPreview.color.r, info.SkinPreview.color.g, info.SkinPreview.color.b, 0.0f);

        info.Animator.SetBool("IsChange", true);
        StartSelectAnimation(info);

        CheckCanSubmit(info);
    }

    private void CheckCanSubmitAll()
    {
        foreach (PlayerInfo info in playerInfo)
        {
            CheckCanSubmit(info);
        }
    }

    private void CheckCanSubmit(PlayerInfo info)
    {
        if (!isCanSelect[info.selectSkin] && !info.isSkinSubmit)
        {
            // アイコンを表示する
        }
        else
        {
            // アイコンを非表示
        }
    }

    public void StartSelectAnimation(int id)
    {
        foreach (PlayerInfo info in playerInfo)
        {
            if (info.playerId == id)
            {
                info.isSelectAnimation = true;
            }
        }
    }

    public void EndSelectAnimation(int id)
    {
        foreach (PlayerInfo info in playerInfo)
        {
            if (info.playerId == id)
            {
                info.isSelectAnimation = false;
            }
        }
    }

    public void StartSelectAnimation(PlayerInfo info)
    {
        info.isSelectAnimation = true;
    }

    public void EndSelectAnimation(PlayerInfo info)
    {
        info.isSelectAnimation = false;
    }

    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!DeviceManager.Instance.GetIsConnect(i))
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
