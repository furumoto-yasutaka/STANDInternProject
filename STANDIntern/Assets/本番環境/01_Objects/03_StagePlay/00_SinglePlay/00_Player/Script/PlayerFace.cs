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

    //=====��������擾
    [SerializeField]
    private SpriteRenderer bodyRenderer;
    [SerializeField]
    private SpriteRenderer legRenderer;
    [SerializeField]
    private Rigidbody2D rb;

    // �v���C���[���
    private PlayerInfo playerInfo;
    // �X�e�[�g
    private int faceState = (int)FaceState.Normal;
    // �e��̗L������
    private float[] timeLimit = new float[(int)FaceState.Length];
    
    //=====�������l
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
        //=====�e��L�����Ԃ̃J�E���g�_�E��
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

        //=====�R��ꂽ��̏ꍇ�̂ݑ��x�ɉ����ď�Ԃ�؂�ւ��邽�ߊm�F���s��
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

        //=====���݃J�E���g���c���Ă����̒��ň�ԗD��x��������(�l���傫������)�ɐݒ肷��
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
    /// �R��ꂽ��Ɏg���X�v���C�g���w��
    /// </summary>
    private void CheckKickedFace()
    {
        float sqrMag = rb.velocity.sqrMagnitude;

        // ���x�ɉ����Ċ���w��
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

    public PlayerSkinInfo GetSkinInfo()
    {
        return playerInfo.SkinInfo;
    }
}
