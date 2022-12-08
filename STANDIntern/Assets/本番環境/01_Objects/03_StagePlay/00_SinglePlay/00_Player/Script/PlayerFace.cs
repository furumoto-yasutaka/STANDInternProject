using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFace : MonoBehaviour
{
    public enum FaceState
    {
        Normal = 0,
        Kick,
        Kicked,
        Kill,
        Length,
    }

    //=====内部から取得
    [SerializeField]
    private SpriteRenderer bodyRenderer;
    [SerializeField]
    private SpriteRenderer legRenderer;
    [SerializeField]
    private Rigidbody2D rb;

    // プレイヤー情報
    private PlayerInfo playerInfo;
    // ステート
    private int faceState = (int)FaceState.Normal;
    // 各顔の有効時間
    private float[] timeLimit = new float[(int)FaceState.Length];
    
    //=====しきい値
    private static readonly float kickedFaceThreshold = 8.0f;
    private static readonly float kickedStrongFaceThreshold = 15.0f;


    void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();

        for (int i = 0; i < timeLimit.Length; i++)
        {
            timeLimit[i] = 0.0f;
        }

        ChangeFace();

        legRenderer.sprite = playerInfo.SkinInfo.Leg;
        legRenderer.material.SetTexture("_Maintex", playerInfo.SkinInfo.Leg.texture);
    }

    void Update()
    {
        //=====各顔有効時間のカウントダウン
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

        //=====蹴られた顔の場合のみ速度に応じて状態を切り替えるため確認を行う
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

        //=====現在カウントが残っている顔の中で一番優先度が高い顔(値が大きいもの)に設定する
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
                bodyRenderer.sprite = playerInfo.SkinInfo.Normal;
                bodyRenderer.material.SetTexture("_Maintex", playerInfo.SkinInfo.Normal.texture);
                break;
            case (int)FaceState.Kick:
                bodyRenderer.sprite = playerInfo.SkinInfo.Kick;
                bodyRenderer.material.SetTexture("_Maintex", playerInfo.SkinInfo.Kick.texture);
                break;
            case (int)FaceState.Kicked:
                CheckKickedFace();
                break;
            case (int)FaceState.Kill:
                bodyRenderer.sprite = playerInfo.SkinInfo.Kill;
                bodyRenderer.material.SetTexture("_Maintex", playerInfo.SkinInfo.Kill.texture);
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
            bodyRenderer.sprite = playerInfo.SkinInfo.KickedStrong;
            bodyRenderer.material.SetTexture("_Maintex", playerInfo.SkinInfo.KickedStrong.texture);
        }
        else if (sqrMag >= kickedFaceThreshold * kickedFaceThreshold)
        {
            bodyRenderer.sprite = playerInfo.SkinInfo.Kicked;
            bodyRenderer.material.SetTexture("_Maintex", playerInfo.SkinInfo.Kicked.texture);
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

    public PlayerSkinInfo GetSkinInfo()
    {
        return playerInfo.SkinInfo;
    }
}
