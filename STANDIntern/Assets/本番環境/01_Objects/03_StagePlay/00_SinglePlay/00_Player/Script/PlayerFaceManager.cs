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
    /// �w�肵����̃J�E���g��ݒ�
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
    /// �w�肵����̃J�E���g�����Z�b�g
    /// </summary>
    public void LiftState(int state)
    {
        if (faceState != state) { return; }

        timeLimit[state] = 0.0f;

        //=====���݃J�E���g���c���Ă����̒��ň�ԗD��x��������ɐݒ肷��
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
    /// ���ύX
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
    /// �R��ꂽ��Ɏg���X�v���C�g���w��
    /// </summary>
    private void CheckKickedFace()
    {
        float sqrMag = rb.velocity.sqrMagnitude;

        // ���x�ɉ����Ċ���w��
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
            // ���x������Ȃ��̂ŉ�������
            LiftState((int)FaceState.Kicked);
        }
    }

    /// <summary>
    /// �p�����[�^��S�ď�����
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
