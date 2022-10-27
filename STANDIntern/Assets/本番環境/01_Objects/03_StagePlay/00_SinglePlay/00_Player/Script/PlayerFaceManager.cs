using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFaceManager : MonoBehaviour
{
    public enum FaceState
    {
        Normal = 0,
        Kick,
        Kicked,
        Kill,
        Length,
    }

    private int nowFace = (int)FaceState.Normal;
    private int prevFace = (int)FaceState.Normal;

    private SpriteRenderer bodyRenderer;
    private SpriteRenderer legRenderer;

    [SerializeField]
    private PlayerSkinDataBase playerSkinDataBase;
    private int skinId;
    private PlayerSkinInfo skinInfo;

    private float[] timeLimit = new float[(int)FaceState.Length];

    private Rigidbody2D rb;
    private static float kickedFaceThreshold = 8.0f;
    private static float kickedStrongFaceThreshold = 15.0f;

    public PlayerSkinInfo SkinInfo { get { return skinInfo; } }
    public PlayerSkinDataBase PlayerSkinDataBase { get { return playerSkinDataBase; } }

    void Start()
    {
        for (int i = 0; i < timeLimit.Length; i++)
        {
            timeLimit[i] = 0.0f;
        }

        skinId = BattleSumoManager.IsPlayerSkinId[GetComponent<PlayerId>().Id];
        skinInfo = playerSkinDataBase.PlayerSkinInfos[skinId];

        bodyRenderer = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        ChangeTexture();

        legRenderer = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
        legRenderer.sprite = skinInfo.Leg;
        legRenderer.material.SetTexture("_Maintex", skinInfo.Leg.texture);

        rb = transform.GetChild(0).GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        for (int i = 0; i < timeLimit.Length; i++)
        {
            if (timeLimit[i] <= 0.0f)
            {
                timeLimit[i] = 0.0f;
                if (nowFace == i)
                {
                    LiftState(nowFace);
                }
            }
            else
            {
                timeLimit[i] -= Time.deltaTime;
            }
        }

        if (nowFace == (int)FaceState.Kicked)
        {
            CheckKickedFace();
        }
    }

    public void ChangeState(int state, float time)
    {
        if (nowFace <= state)
        {
            prevFace = nowFace;
            nowFace = state;
            ChangeTexture();
        }
        else if (state == (int)FaceState.Kick && nowFace != (int)FaceState.Kill)
        {
            prevFace = nowFace;
            nowFace = state;
            ChangeTexture();
        }

        if (timeLimit[state] < time)
        {
            timeLimit[state] = time;
        }
    }

    public void LiftState(int state)
    {
        if (nowFace != state) { return; }

        timeLimit[state] = 0.0f;

        for (int i = state - 1; i >= 0; i--)
        {
            if (timeLimit[i] > 0.0f)
            {
                prevFace = nowFace;
                nowFace = i;
                ChangeTexture();
                return;
            }
        }

        nowFace = (int)FaceState.Normal;
        ChangeTexture();
    }

    public void ChangeTexture()
    {
        switch (nowFace)
        {
            case (int)FaceState.Normal:
                bodyRenderer.sprite = skinInfo.Normal;
                bodyRenderer.material.SetTexture("_Maintex", skinInfo.Normal.texture);
                break;
            case (int)FaceState.Kick:
                bodyRenderer.sprite = skinInfo.Kick;
                bodyRenderer.material.SetTexture("_Maintex", skinInfo.Kick.texture);
                break;
            case (int)FaceState.Kicked:
                ChangeKickedFace();
                break;
            case (int)FaceState.Kill:
                bodyRenderer.sprite = skinInfo.Kill;
                bodyRenderer.material.SetTexture("_Maintex", skinInfo.Kill.texture);
                break;
        }
    }

    public void ResetParam()
    {
        nowFace = (int)FaceState.Normal;
        prevFace = (int)FaceState.Normal;

        for (int i = 0; i < timeLimit.Length; i++)
        {
            timeLimit[i] = 0.0f;
        }

        ChangeTexture();
    }

    public void ChangeKickedFace()
    {
        float sqrMag = rb.velocity.sqrMagnitude;
        if (sqrMag >= kickedStrongFaceThreshold * kickedStrongFaceThreshold)
        {
            bodyRenderer.sprite = skinInfo.KickedStrong;
            bodyRenderer.material.SetTexture("_Maintex", skinInfo.KickedStrong.texture);
        }
        else if (sqrMag >= kickedFaceThreshold * kickedFaceThreshold)
        {
            bodyRenderer.sprite = skinInfo.Kicked;
            bodyRenderer.material.SetTexture("_Maintex", skinInfo.Kicked.texture);
        }
    }

    private void CheckKickedFace()
    {
        float sqrMag = rb.velocity.sqrMagnitude;
        if (sqrMag >= kickedStrongFaceThreshold * kickedStrongFaceThreshold)
        {
            bodyRenderer.sprite = skinInfo.KickedStrong;
            bodyRenderer.material.SetTexture("_Maintex", skinInfo.KickedStrong.texture);
        }
        else if (sqrMag >= kickedFaceThreshold * kickedFaceThreshold)
        {
            bodyRenderer.sprite = skinInfo.Kicked;
            bodyRenderer.material.SetTexture("_Maintex", skinInfo.Kicked.texture);
        }
        else
        {
            LiftState((int)FaceState.Kicked);
        }
    }
}
