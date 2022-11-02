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

    [SerializeField]
    private SpriteRenderer bodyRenderer;
    [SerializeField]
    private SpriteRenderer legRenderer;
    [SerializeField]
    private PlayerSkinDataBase playerSkinDataBase;
    [SerializeField]
    private Rigidbody2D rb;

    private int faceState = (int)FaceState.Normal;
    private PlayerSkinInfo skinInfo;
    private float[] timeLimit = new float[(int)FaceState.Length];
    
    private static readonly float kickedFaceThreshold = 8.0f;
    private static readonly float kickedStrongFaceThreshold = 15.0f;

    public PlayerSkinInfo SkinInfo { get { return skinInfo; } }
    public PlayerSkinDataBase PlayerSkinDataBase { get { return playerSkinDataBase; } }

    void Start()
    {
        for (int i = 0; i < timeLimit.Length; i++)
        {
            timeLimit[i] = 0.0f;
        }

        int skinId = BattleSumoManager.IsPlayerSkinId[GetComponent<PlayerId>().Id];
        skinInfo = playerSkinDataBase.PlayerSkinInfos[skinId];

        ChangeFace();

        legRenderer.sprite = skinInfo.Leg;
        legRenderer.material.SetTexture("_Maintex", skinInfo.Leg.texture);
    }

    void Update()
    {
        for (int i = 0; i < timeLimit.Length; i++)
        {
            if (timeLimit[i] <= 0.0f)
            {
                timeLimit[i] = 0.0f;
                if (faceState == i)
                {
                    LiftState(faceState);
                }
            }
            else
            {
                timeLimit[i] -= Time.deltaTime;
            }
        }

        if (faceState == (int)FaceState.Kicked)
        {
            CheckKickedFace();
        }
    }

    /// <summary>
    /// 指定した顔のカウントを設定
    /// </summary>
    public void SetState(int state, float time)
    {
        if (faceState <= state ||
            (state == (int)FaceState.Kick && faceState != (int)FaceState.Kill))
        {
            faceState = state;
            ChangeFace();
        }

        if (timeLimit[state] < time)
        {
            timeLimit[state] = time;
        }
    }

    /// <summary>
    /// 指定した顔のカウントをリセット
    /// </summary>
    public void LiftState(int state)
    {
        if (faceState != state) { return; }

        timeLimit[state] = 0.0f;

        //=====現在カウントが残っている顔の中で一番優先度が高い顔に設定する
        for (int i = state - 1; i >= 0; i--)
        {
            if (timeLimit[i] > 0.0f)
            {
                faceState = i;
                ChangeFace();
                return;
            }
        }

        faceState = (int)FaceState.Normal;
        ChangeFace();
    }

    /// <summary>
    /// 顔を変更
    /// </summary>
    public void ChangeFace()
    {
        switch (faceState)
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
                CheckKickedFace();
                break;
            case (int)FaceState.Kill:
                bodyRenderer.sprite = skinInfo.Kill;
                bodyRenderer.material.SetTexture("_Maintex", skinInfo.Kill.texture);
                break;
        }
    }

    /// <summary>
    /// 蹴られた顔に使うスプライトを指定
    /// </summary>
    private void CheckKickedFace()
    {
        float sqrMag = rb.velocity.sqrMagnitude;

        // 速度に応じて顔を指定
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
            // 速度が足りないので解除する
            LiftState((int)FaceState.Kicked);
        }
    }

    /// <summary>
    /// パラメータを全て初期化
    /// </summary>
    public void ResetParam()
    {
        faceState = (int)FaceState.Normal;

        for (int i = 0; i < timeLimit.Length; i++)
        {
            timeLimit[i] = 0.0f;
        }

        ChangeFace();
    }
}
