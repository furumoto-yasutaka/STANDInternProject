using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // �v���C���[�X�e�[�g
    public enum KickStateId
    {
        None = 0,
        Kick,
        LegReturn,
        Length,
    }

    //=====��������擾�������
    [Header("�v���n�u�擾")]
    private PlayerId playerId;
    private PlayerFace playerFace;
    private PlayerEffect playerEffect;
    private PlayerInvincible playerInvincible;
    private Transform body;
    private Transform leg; 
    private Rigidbody2D rb;

    [SerializeField, RenameField("�v���C���[�̈ړ����x")]
    [Header("�ړ��֌W")]
    private float moveSpeed = 2.0f;
    [SerializeField, RenameField("���͂Ƌt�����։������̌������x")]
    private float moveDecaySpeed = 0.8f;
    [SerializeField, RenameField("�v���C���[�̈ړ����͂̂������l")]
    private float moveThreshold = 2.0f;

    [SerializeField, RenameField("�R��𔭓���������͂������l(�X�e�B�b�N)")]
    [Header("�R��֌W")]
    private float kickExecStickFall = 0.7f;
    [SerializeField, RenameField("����L�΂�����")]
    private float kickRange = 0.85f;
    [SerializeField, RenameField("����L�΂�����")]
    private float kickTime = 0.4f;
    [SerializeField, RenameField("�����Ђ����߂鎞��")]
    private float kickReturnTime = 0.6f;
    [SerializeField, RenameField("�L�b�N��")]
    private float kickPower = 1.0f;
    [SerializeField, RenameField("�G�v���C���[�L�b�N���̎����ւ̔���")]
    private float kickPlayerPower = 2.0f;
    [SerializeField, RenameField("�L�b�N�{��(X)")]
    private float kickMagX = 1.3f;
    [SerializeField, RenameField("�L�b�N���̉�]���x")]
    private float kickAngularPower = 100.0f;
    [SerializeField, RenameField("�R��ꂽ���̃L�b�N�͊�_(�ŏ�)")]
    private float kickedAddPowerMin = 0.85f;
    [SerializeField, RenameField("�R��ꂽ���̃L�b�N�͊�_(�ő�)")]
    private float kickedAddPowerMax = 1.7f;

    //=====�X�e�[�g�֌W
    // �R��̃X�e�[�g
    [ReadOnly]
    public KickStateId kickState = KickStateId.None;
    // ���S�������ǂ���
    private bool isDeath = false;
    // �G�ɔ�΂��ꂽ���ǂ���
    private bool isBlow = false;
    // �X�e�[�g�ɉ������֐�
    private Action[] kickAction;

    //=====���͊֌W
    // �X�e�B�b�N��|���Ă��邩
    private bool fallStick = false;

    //=====���̑�
    // �W�����v�̉����x�𔽉f������(��x�̃L�b�N�ŕ��������x���������Ȃ��悤�ɂ���)
    private bool isJump = false;
    // �R�����
    private Vector2 kickDirection;
    // �L�b�N�̎��Ԍv���֌W
    private float kickTimeCount = 0.0f;
    // ����Ώۂ̃f�o�C�X
    private Gamepad pad;


    public int PlayerId { get { return playerId.Id; } }
    public Vector2 KickDirection { get { return kickDirection; } }
    public bool IsDeath { get { return isDeath; } }


    void Start()
    {
        body = transform.GetChild(0);
        leg = transform.GetChild(1);
        leg.gameObject.SetActive(false);

        playerId = GetComponent<PlayerId>();
        playerFace = GetComponent<PlayerFace>();
        playerEffect = GetComponent<PlayerEffect>();
        playerInvincible = GetComponent<PlayerInvincible>();
        rb = body.GetComponent<Rigidbody2D>();

        kickState = KickStateId.None;
        isDeath = true;
        isJump = false;
        kickAction = new Action[(int)KickStateId.Length]
            { Kick_NoneAction, Kick_KickAction, Kick_LegReturnAction };

        // �R���W�������~�߂Ă���
        Stop();

        DeviceManager.Instance.Add_RemoveDevicePartsCallBack(PlayerNotActive, playerId.Id);
        pad = DeviceManager.Instance.GetDevice_FromPlayerIndex(PlayerId);
    }

    void Update()
    {
        if (!DeviceManager.Instance.GetIsConnect(playerId.Id) || pad == null) { PlayerNotActive(); }
        if (isDeath) { return; }

        // �ړ�����
        MoveAction();
        // �R�����
        kickAction[(int)kickState]();
        // �R����Ԍp���m�F
        CheckBlowEnd();
    }

    /// <summary>
    /// ���E�ړ�����
    /// </summary>
    void MoveAction()
    {
        //=====���͏�Ԏ擾
        float stickHorizontal = pad.leftStick.ReadValue().x;
        float dpadHorizontal = pad.dpad.ReadValue().x;

        //=====���x�v�Z
        float horizontal = Mathf.Abs(stickHorizontal) > Mathf.Abs(dpadHorizontal) ? stickHorizontal : dpadHorizontal;
        float decaySpeed = rb.velocity.x * moveDecaySpeed * horizontal * Time.deltaTime;    // ��������ꍇ�̌v�Z
        float defaultSpeed = moveSpeed * horizontal * Time.deltaTime;                       // ����������ꍇ�̌v�Z

        //=====���x�␳�E���f
        if (horizontal < 0.0f && rb.velocity.x >= -moveThreshold)
        {
            // ��Βl�̑傫�����x�𔽉f����
            if (decaySpeed < defaultSpeed)
            {
                rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }
        else if (horizontal > 0.0f && rb.velocity.x <= moveThreshold)
        {
            // ��Βl�̑傫�����x�𔽉f����
            if (decaySpeed > defaultSpeed)
            {
                rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }
    }

    /// <summary>
    /// �����o��(���͎�t���)����
    /// </summary>
    void Kick_NoneAction()
    {
        //=====���́E�����擾
        Vector2 moveInput = pad.rightStick.ReadValue();
        float sqrMagnitude = moveInput.sqrMagnitude;
        Vector2 normal = moveInput.normalized;

        if (sqrMagnitude < kickExecStickFall * kickExecStickFall)
        {//=====�X�e�B�b�N�̌X���󋵂��X�V
            fallStick = false;
        }
        else if (!fallStick)
        {//=====�R����s
            fallStick = true;

            kickState = KickStateId.Kick;

            kickTimeCount = 0.0f;
            kickDirection = normal;

            leg.position = body.position;
            // �����o�������ɉ�]
            leg.rotation = Quaternion.identity * Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.down, kickDirection), Vector3.forward);
            leg.gameObject.SetActive(true);

            // ����R���ԂɑJ��
            playerFace.SetState((int)PlayerFace.FaceState.Kick, 999.0f);
        }
    }

    /// <summary>
    /// �����o��(���o�����)����
    /// </summary>
    void Kick_KickAction()
    {
        if (kickTimeCount < kickTime)
        {
            kickTimeCount += Time.deltaTime;

            //=====���`��Ԃő��̈ʒu���X�V
            Vector2 vec = kickDirection * kickRange * kickTimeCount / kickTime;
            leg.position = body.position + (Vector3)vec;
        }
        else
        {
            //=====����߂��X�e�[�g��J�ڂ���
            kickState = KickStateId.LegReturn;

            kickTimeCount = kickReturnTime;
        }
    }

    /// <summary>
    /// �����o��(���Ђ����ߏ��)����
    /// </summary>
    void Kick_LegReturnAction()
    {
        if (kickTimeCount > 0.0f)
        {
            kickTimeCount -= Time.deltaTime;

            //=====���`��Ԃő��̈ʒu���X�V
            Vector2 vec = kickDirection * kickRange * kickTimeCount / kickReturnTime;
            leg.position = body.position + (Vector3)vec;
        }
        else
        {
            //=====�����Ȃ���ԂɑJ�ڂ���
            kickState = KickStateId.None;

            isJump = false;
            leg.gameObject.SetActive(false);
            leg.position = body.position;

            // ��̏R���Ԃ�����
            playerFace.LiftState((int)PlayerFace.FaceState.Kick);
        }
    }

    /// <summary>
    /// �n�`���R�鏈��
    /// </summary>
    public void KickPlatformAddForce(Collider2D collision)
    {
        if (!isJump)
        {
            // 1�x�̏R��ŕ�����͂�������Ȃ��悤�ɂ��邽�߂̃t���O�ύX
            isJump = true;

            // ���x���v�Z
            Vector2 vel = CalcKickForce(-kickDirection, kickPower, 1.0f, 0.0f);

            // ���x�𔽉f
            rb.velocity = vel;
            // ��]���x�𑬓x�����Ɍv�Z
            rb.angularVelocity = rb.velocity.x * -kickAngularPower;

            // �G�t�F�N�g�Đ�
            PlayImpactEffect(collision.ClosestPoint(leg.position));

            // ���ʉ��Đ�
            AudioManager.Instance.PlaySe("�W�����v");
        }
    }

    /// <summary>
    /// �v���C���[���R�鏈��
    /// </summary>
    public void KickPlayerAddForce()
    {
        if (!isJump)
        {
            // 1�x�̏R��ŕ�����͂�������Ȃ��悤�ɂ��邽�߂̃t���O�ύX
            isJump = true;

            // ���x���v�Z
            Vector2 vel = CalcKickForce(-kickDirection, kickPlayerPower, 1.0f, 0.0f);

            // ���x�𔽉f
            rb.velocity = vel;
            // ��]���x�𑬓x�����Ɍv�Z
            rb.angularVelocity = rb.velocity.x * -kickAngularPower;
        }
    }

    /// <summary>
    /// �v���C���[�ɏR��ꂽ����
    /// </summary>
    /// <param name="distance"> ����̑��Ǝ����̑̂Ƃ̋����x�N�g�� </param>
    /// <param name="direction"> ���ł������� </param>
    public void KickedAddForce(Vector2 distance, Vector2 direction, Vector2 closestPoint)
    {
        // ���x���v�Z
        Vector2 vel = CalcKickForce(direction, kickPower * 2.0f, 1.0f, 0.5f);
        float distanceSqrMag = distance.sqrMagnitude;  // �����̒���(2��)���v�Z
        float addPowerRate = 0.0f;
        
        //=====����Ƃ̋������߂��قǏR��͂���������悤�ɒ�������
        if (distanceSqrMag < kickedAddPowerMin * kickedAddPowerMin)
        {
            addPowerRate = 1.0f;
        }
        else if (distanceSqrMag > kickedAddPowerMax * kickedAddPowerMax)
        {
            addPowerRate = 0.0f;
        }
        else
        {
            addPowerRate = (distanceSqrMag - kickedAddPowerMin) / (kickedAddPowerMin - kickedAddPowerMax);
        }

        // ���x�𔽉f
        rb.velocity = vel + distance.normalized * addPowerRate;
        // ��]���x�𑬓x�����Ɍv�Z
        rb.angularVelocity = rb.velocity.x * -kickAngularPower;

        // ����R��ꂽ��ԂɑJ��
        playerFace.SetState((int)PlayerFace.FaceState.Kicked, 2.0f);

        // �Ԃ���уG�t�F�N�g�𔭐������邩�m�F
        CheckBlowStart();

        // �R��ꂽ�G�t�F�N�g�𔭐������邩�m�F
        CheckKickedImpact(closestPoint);
    }

    /// <summary>
    /// �v���C���[�ɏR��ꂽ����
    /// </summary>
    /// <param name="direction"> ���ł������� </param>
    /// <param name="kickPower"> �L�b�N�� </param>
    /// <param name="reverseXPowerRate"> ���݂̑��x�ƃW�����v�̑��x��X�������t�������ꍇ�̉��Z�␳�{�� </param>
    /// <param name="reverseYPowerRate"> ���݂̑��x�ƃW�����v�̑��x��Y�������t�������ꍇ�̉��Z�␳�{�� </param>
    private Vector2 CalcKickForce(Vector2 direction, float kickPower,
        float reverseXPowerRate, float reverseYPowerRate)
    {
        Vector2 resultVel;
        Vector2 jumpVel = direction * kickPower;
        // ���E�̑��x�ɕ␳��������
        jumpVel.x *= kickMagX;

        //=====���݂̃v���C���[�̍��E���x���K���W�����v�̍��E�ւ̉����x��
        //=====��������ԂɂȂ�悤�Ƀx�N�g�����v�Z����
        if ((rb.velocity.x > 0 && jumpVel.x > 0) ||
            (rb.velocity.x < 0 && jumpVel.x < 0))
        {
            resultVel.x = jumpVel.x + rb.velocity.x;
        }
        else
        {
            resultVel.x = jumpVel.x - rb.velocity.x * reverseXPowerRate;
        }

        //=====���݂̑��x�ƃW�����v�̑��x��Y�������������̏ꍇ�͑������킹�A
        //=====�t�̏ꍇ�͌��݂̑��x�̔������W�����v�̑��x�ɑ���
        if ((rb.velocity.y > 0 && jumpVel.y > 0) ||
            (rb.velocity.y < 0 && jumpVel.y < 0))
        {
            resultVel.y = jumpVel.y + rb.velocity.y;
        }
        else
        {
            resultVel.y = jumpVel.y - rb.velocity.y * reverseYPowerRate;
        }

        return resultVel;
    }

    /// <summary>
    /// ���S����
    /// </summary>
    public void Death()
    {
        isDeath = true;

        // �R���W�������~�߂�
        Stop();

        // ���S�G�t�F�N�g����
        Vector2 normal = -new Vector2(rb.velocity.x, rb.velocity.y).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, normal);
        playerEffect.PlayDeathEff(body.position + (Vector3)normal * 2.0f, Quaternion.AngleAxis(angle, Vector3.forward));

        // ������уG�t�F�N�g���~
        EndBlow();
    }

    /// <summary>
    /// ��~����
    /// </summary>
    public void Stop()
    {
        // ���W�b�h�{�f�B�𖳌�
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0.0f;

        // �����蔻��𖳌�
        body.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

        // �L�b�N�������I��
        kickState = KickStateId.None;
        isJump = false;
        leg.gameObject.SetActive(false);
        leg.position = body.position;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void Revival()
    {
        isDeath = false;

        // ���W�b�h�{�f�B��L��
        rb.bodyType = RigidbodyType2D.Dynamic;
        body.GetChild(0).GetComponent<CircleCollider2D>().enabled = true;

        // ���G��Ԃ�ݒ�
        playerInvincible.StartSpownInvincible();

        // ��̏�Ԃ�������
        playerFace.ResetParam();
    }

    /// <summary>
    /// �Փ˃G�t�F�N�g�Đ�
    /// </summary>
    private void PlayImpactEffect(Vector3 createPos)
    {
        float sqrMag = rb.velocity.sqrMagnitude;
        float angle = Vector2.SignedAngle(Vector2.up, leg.position - (Vector3)createPos);
        float threshold = PlayerEffect.hitEffectStrongThreshold * PlayerEffect.hitEffectStrongThreshold;

        if (sqrMag >= threshold)
        {
            playerEffect.PlayImpactStrongEff(createPos, Quaternion.AngleAxis(angle, Vector3.back));
        }
        else
        {
            playerEffect.PlayImpactMiddleEff(createPos, Quaternion.AngleAxis(angle, Vector3.back));
        }
    }

    /// <summary>
    /// ������уG�t�F�N�g�Đ��m�F
    /// </summary>
    public void CheckBlowStart()
    {
        if (!isBlow)
        {
            if (rb.velocity.sqrMagnitude >= PlayerEffect.blowEffStartThreshold * PlayerEffect.blowEffStartThreshold)
            {
                StartBlow();
            }
        }
    }

    /// <summary>
    /// ������уG�t�F�N�g�Đ�
    /// </summary>
    public void StartBlow()
    {
        isBlow = true;
        playerEffect.PlayBlowEff();
    }

    /// <summary>
    /// ������уG�t�F�N�g��~�m�F
    /// </summary>
    public void CheckBlowEnd()
    {
        if (isBlow)
        {
            if (rb.velocity.sqrMagnitude <= PlayerEffect.blowEffEndThreshold * PlayerEffect.blowEffEndThreshold)
            {
                EndBlow();
            }
        }
    }

    /// <summary>
    /// ������уG�t�F�N�g��~
    /// </summary>
    public void EndBlow()
    {
        isBlow = false;
        playerEffect.StopBlowEff();
    }

    /// <summary>
    /// �R���G�t�F�N�g�E���ʉ��Đ��m�F
    /// </summary>
    public void CheckKickedImpact(Vector2 createEffectPos)
    {
        float threshold = PlayerEffect.kickedImpactWaveStrongThreshold * PlayerEffect.kickedImpactWaveStrongThreshold;

        if (rb.velocity.sqrMagnitude >= threshold)
        {
            playerEffect.PlayKickedImpactStrongEff(createEffectPos);
            AudioManager.Instance.PlaySe("�R����󂯂�(�d)");
        }
        else
        {
            playerEffect.PlayKickedImpactNormalEff(createEffectPos);
            AudioManager.Instance.PlaySe("�R����󂯂�(�y)");
        }
    }

    /// <summary>
    /// �R���g���[���[�ؒf���p��A�N�e�B�u������
    /// </summary>
    public void PlayerNotActive()
    {
        gameObject.SetActive(false);
    }
}
