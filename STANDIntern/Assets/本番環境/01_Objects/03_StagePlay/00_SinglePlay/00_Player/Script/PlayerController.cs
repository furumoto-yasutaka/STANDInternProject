using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum KickStateId
    {
        None = 0,
        Kick,
        LegReturn,
        Length,
    }

    private Rigidbody2D Rb;
    private Transform Body;
    private Transform Leg;
    private ParticleSystem BlowEffect;

    [SerializeField, RenameField("�v���C���[�̈ړ����x")]
    private float MoveSpeed = 2.0f;
    [SerializeField, RenameField("���͂Ƌt�����։������̌������x")]
    private float MoveDecaySpeed = 0.8f;
    [SerializeField, RenameField("�v���C���[�̈ړ����͂̂������l")]
    private float MoveThreshold = 2.0f;
    [SerializeField, RenameField("�R��𔭓���������͂������l(�X�e�B�b�N)")]
    private float KickExecStickFall = 0.7f;
    [SerializeField, RenameField("�R��𔭓������鑬�x�̂������l(�}�E�X)")]
    private float KickExecSpeed = 0.5f;
    //[SerializeField, RenameField("�R�蔭����̃N�[���^�C��")]
    //private float KickExecCoolTime = 1.0f;
    [SerializeField, RenameField("����L�΂�����")]
    private float KickRange = 0.85f;
    [SerializeField, RenameField("����L�΂�����")]
    private float KickTime = 0.4f;
    [SerializeField, RenameField("�����Ђ����߂鎞��")]
    private float KickReturnTime = 0.6f;
    [SerializeField, RenameField("�L�b�N��")]
    private float KickPower = 1.0f;
    [SerializeField, RenameField("�G�v���C���[�L�b�N���̎����ւ̔���")]
    private float KickPlayerPower = 2.0f;
    [SerializeField, RenameField("�L�b�N�{��(X)")]
    private float KickMagX = 1.3f;
    [SerializeField, RenameField("�L�b�N���̉�]���x")]
    private float KickAngularPower = 100.0f;
    [SerializeField, RenameField("����v���C���[�L�b�N���̊�_(�ŏ�)")]
    private float KickedAddPowerMin = 0.85f;
    [SerializeField, RenameField("����v���C���[�L�b�N���̊�_(�ő�)")]
    private float KickedAddPowerMax = 1.7f;

    [ReadOnly]
    public KickStateId KickState = KickStateId.None;
    private Action[] KickAction;
    private bool IsJump = false;
    private Vector2 kickDirection;
    public Vector2 KickDirection { get { return kickDirection; } }
    private float KickTimeCount = 0.0f;
    //private float KickExecCoolTimeCount = 0.0f;
    private bool fallStick = false;

    // ImputSystem�֌W
    private InputAction moveAction;
    private InputAction prekickAction;
    private InputAction kickmouseAction;
    private InputAction kickpadAction;

    private bool isDeath = false;
    private bool isBlow = false;
    [SerializeField]
    private float startBlowThreshold = 10.0f;
    [SerializeField]
    private float endBlowThreshold = 8.0f;

    private static float hitEffectStrongThreshold = 12.0f;

    public bool IsDeath { get { return isDeath; } }

    void Start()
    {
        Body = transform.GetChild(0);
        Leg = transform.GetChild(1);
        BlowEffect = transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>();

        Leg.gameObject.SetActive(false);

        Rb = Body.GetComponent<Rigidbody2D>();
        KickAction = new Action[(int)KickStateId.Length]
            { Kick_NoneAction, Kick_KickAction, Kick_LegReturnAction };

        PlayerInput input = GetComponent<PlayerInput>();
        InputActionMap map = input.currentActionMap;
        moveAction = map["Move"];
        prekickAction = map["PreKick"];
        kickmouseAction = map["Kick_Mouse"];
        kickpadAction = map["Kick_Pad"];

        isDeath = true;
        KickState = KickStateId.None;
        IsJump = false;
        Leg.gameObject.SetActive(false);

        Rb.bodyType = RigidbodyType2D.Kinematic;
        Rb.velocity = Vector2.zero;
        transform.GetChild(0).GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
    }

    void Update()
    {
        if (isDeath) { return; }

        float horizontal = moveAction.ReadValue<Vector2>().x;
        float decaySpeed = Rb.velocity.x * MoveDecaySpeed * horizontal * Time.deltaTime;
        float defaultSpeed = MoveSpeed * horizontal * Time.deltaTime;

        if (horizontal < 0.0f && Rb.velocity.x >= -MoveThreshold)
        {
            if (decaySpeed < defaultSpeed)
            {
                Rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                Rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }
        else if (horizontal > 0.0f && Rb.velocity.x <= MoveThreshold)
        {
            if (decaySpeed > defaultSpeed)
            {
                Rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                Rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }

        KickAction[(int)KickState]();

        CheckBlowEnd();
    }

    void Kick_NoneAction()
    {
        Vector2 move;
        Vector2 normal;

        if (prekickAction.IsPressed())
        {
            move = kickmouseAction.ReadValue<Vector2>();
            normal = move.normalized;

            if (move.sqrMagnitude >= KickExecSpeed * KickExecSpeed)
            {
                KickState = KickStateId.Kick;

                KickTimeCount = 0.0f;
                kickDirection = normal;

                Leg.position = Body.position;
                Leg.rotation = Quaternion.identity * Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.down, kickDirection), Vector3.forward);
                Leg.gameObject.SetActive(true);
            }
        }

        move = kickpadAction.ReadValue<Vector2>();
        normal = move.normalized;

        if (move.sqrMagnitude < KickExecStickFall * KickExecStickFall)
        {
            fallStick = false;
        }

        if (move.sqrMagnitude >= KickExecStickFall * KickExecStickFall && !fallStick)
        {
            fallStick = true;

            KickState = KickStateId.Kick;

            KickTimeCount = 0.0f;
            kickDirection = normal;

            Leg.position = Body.position;
            Leg.rotation = Quaternion.identity * Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.down, kickDirection) ,Vector3.forward);
            Leg.gameObject.SetActive(true);
        }
    }

    void Kick_KickAction()
    {
        if (KickTimeCount < KickTime)
        {
            KickTimeCount += Time.deltaTime;
            
            Vector2 vec = kickDirection * KickRange * KickTimeCount / KickTime;
            Leg.position = Body.position + (Vector3)vec;
        }
        else
        {
            KickState = KickStateId.LegReturn;

            KickTimeCount = KickReturnTime;
        }
    }

    void Kick_LegReturnAction()
    {
        if (KickTimeCount > 0.0f)
        {
            KickTimeCount -= Time.deltaTime;

            Vector2 vec = kickDirection * KickRange * KickTimeCount / KickReturnTime;
            Leg.position = Body.position + (Vector3)vec;
        }
        else
        {
            KickState = KickStateId.None;

            IsJump = false;
            Leg.gameObject.SetActive(false);
            Leg.position = Body.position;
        }
    }

    public void KickPlatformAddForce(Collider2D collision)
    {
        if (!IsJump)
        {
            IsJump = true;
            Vector2 resultVel;
            Vector2 jumpVel = -kickDirection * KickPower;
            jumpVel.x *= KickMagX;

            if ((Rb.velocity.x > 0 && jumpVel.x > 0) || (Rb.velocity.x < 0 && jumpVel.x < 0))
            {
                resultVel.x = Rb.velocity.x + jumpVel.x;
            }
            else
            {
                resultVel.x = jumpVel.x - Rb.velocity.x;
            }

            if ((Rb.velocity.y > 0 && jumpVel.y > 0) || (Rb.velocity.y < 0 && jumpVel.y < 0))
            {
                resultVel.y = jumpVel.y + Rb.velocity.y;
            }
            else
            {
                resultVel.y = jumpVel.y - Rb.velocity.y * 0.25f;
            }

            Rb.velocity = resultVel;
            Rb.angularVelocity = Rb.velocity.x * -KickAngularPower;

            AudioManager.Instance.PlaySe("�W�����v");

            float sqrMag = Rb.velocity.sqrMagnitude;
            Vector2 hitPoint = collision.ClosestPoint(Leg.position);
            float angle = Vector2.SignedAngle(Vector2.up, Leg.position - (Vector3)hitPoint);

            if (sqrMag >= hitEffectStrongThreshold * hitEffectStrongThreshold)
            {
                EffectContainer.Instance.PlayEffect("�Փ�(��)", hitPoint, Quaternion.AngleAxis(angle, Vector3.back));
            }
            else
            {
                EffectContainer.Instance.PlayEffect("�Փ�(��)", hitPoint, Quaternion.AngleAxis(angle, Vector3.back));
            }
        }
    }

    public void KickPlayerAddForce()
    {
        if (!IsJump)
        {
            IsJump = true;
            Vector2 resultVel;
            Vector2 jumpVel = -kickDirection * KickPlayerPower;
            jumpVel.x *= KickMagX;

            if ((Rb.velocity.x > 0 && jumpVel.x > 0) || (Rb.velocity.x < 0 && jumpVel.x < 0))
            {
                resultVel.x = Rb.velocity.x + jumpVel.x;
            }
            else
            {
                resultVel.x = jumpVel.x - Rb.velocity.x;
            }

            if ((Rb.velocity.y > 0 && jumpVel.y > 0) || (Rb.velocity.y < 0 && jumpVel.y < 0))
            {
                resultVel.y = jumpVel.y + Rb.velocity.y;
            }
            else
            {
                resultVel.y = jumpVel.y - Rb.velocity.y * 0.25f;
            }

            Rb.velocity = resultVel;
            Rb.angularVelocity = Rb.velocity.x * -KickAngularPower;

            AudioManager.Instance.PlaySe("�W�����v");
        }
    }

    public void KickedAddForce(Vector2 distance, Vector2 kickDirection)
    {
        Vector2 resultVel;
        Vector2 jumpVel = kickDirection * KickPower * 2.0f;
        float distanceLen = distance.sqrMagnitude;
        float addPowerRate = 0.0f;
        jumpVel.x *= KickMagX;

        if ((Rb.velocity.x > 0 && jumpVel.x > 0) || (Rb.velocity.x < 0 && jumpVel.x < 0))
        {
            resultVel.x = Rb.velocity.x + jumpVel.x;
        }
        else
        {
            resultVel.x = jumpVel.x - Rb.velocity.x;
        }

        if ((Rb.velocity.y > 0 && jumpVel.y > 0) || (Rb.velocity.y < 0 && jumpVel.y < 0))
        {
            resultVel.y = jumpVel.y + Rb.velocity.y;
        }
        else
        {
            resultVel.y = jumpVel.y - Rb.velocity.y * 0.25f;
        }

        if (distanceLen < KickedAddPowerMin * KickedAddPowerMin)
        {
            addPowerRate = 1.0f;
        }
        else if (distanceLen > KickedAddPowerMax * KickedAddPowerMax)
        {
            addPowerRate = 0.0f;
        }
        else
        {
            addPowerRate = (distanceLen - KickedAddPowerMin) / (KickedAddPowerMin - KickedAddPowerMax);
        }

        Rb.velocity = resultVel + distance.normalized * addPowerRate;
        Rb.angularVelocity = Rb.velocity.x * -KickAngularPower;

        AudioManager.Instance.PlaySe("�W�����v");

        CheckBlowStart();
    }

    public void Death()
    {
        isDeath = true;
        KickState = KickStateId.None;
        IsJump = false;
        Leg.gameObject.SetActive(false);

        //Vector2 normal = -new Vector2(Body.position.x, Body.position.y).normalized;
        Vector2 normal = -new Vector2(Rb.velocity.x, Rb.velocity.y).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, normal);
        EffectContainer.Instance.PlayEffect("���S", Body.position + (Vector3)normal * 2.0f, Quaternion.AngleAxis(angle, Vector3.forward));

        Rb.bodyType = RigidbodyType2D.Kinematic;
        Rb.velocity = Vector2.zero;
        transform.GetChild(0).GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

        isBlow = false;
        BlowEffect.Stop();
    }

    public void Revival()
    {
        isDeath = false;
        Rb.bodyType = RigidbodyType2D.Dynamic;
        transform.GetChild(0).GetChild(0).GetComponent<CircleCollider2D>().enabled = true;
    }

    public void CheckBlowStart()
    {
        if (Rb.velocity.sqrMagnitude >= startBlowThreshold * startBlowThreshold)
        {
            StartBlow();
        }
    }

    public void CheckBlowEnd()
    {
        if (isBlow)
        {
            if (Rb.velocity.sqrMagnitude <= endBlowThreshold * endBlowThreshold)
            {
                EndBlow();
            }
        }
    }

    public void EndBlow()
    {
        isBlow = false;
        BlowEffect.Stop();
    }

    public void StartBlow()
    {
        isBlow = true;
        BlowEffect.Play();
    }
}
