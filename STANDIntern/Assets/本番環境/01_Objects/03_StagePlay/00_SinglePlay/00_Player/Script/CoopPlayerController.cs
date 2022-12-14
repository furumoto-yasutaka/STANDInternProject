using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopPlayerController : MonoBehaviour
{
    public enum KickStateId
    {
        None = 0,
        Kick,
        LegReturn,
        Length,
    }

    private Rigidbody2D Rb;
    [SerializeField, RenameField("足オブジェクト")]
    private Transform Leg;

    [SerializeField, RenameField("プレイヤーの移動速度")]
    private float MoveSpeed = 2.0f;
    [SerializeField, RenameField("入力と逆方向へ加速時の減衰速度")]
    private float MoveDecaySpeed = 0.8f;
    [SerializeField, RenameField("プレイヤーの移動入力のしきい値")]
    private float MoveThreshold = 2.0f;
    [SerializeField, RenameField("蹴りを発動させる入力しきい値(スティック)")]
    private float KickExecStickFall = 0.7f;
    [SerializeField, RenameField("蹴りを発動させる速度のしきい値(マウス)")]
    private float KickExecSpeed = 0.5f;
    //[SerializeField, RenameField("蹴り発動後のクールタイム")]
    //private float KickExecCoolTime = 1.0f;
    [SerializeField, RenameField("足を伸ばす距離")]
    private float KickRange = 0.85f;
    [SerializeField, RenameField("足を伸ばす時間")]
    private float KickTime = 0.4f;
    [SerializeField, RenameField("足をひっこめる時間")]
    private float KickReturnTime = 0.6f;
    [SerializeField, RenameField("キック力")]
    private float KickPower = 1.0f;
    [SerializeField, RenameField("キック倍率(X)")]
    private float KickMagX = 1.3f;
    //[SerializeField, RenameField("キック時の回転速度")]
    //private float KickAngularPower = 100.0f;

    [ReadOnly]
    public KickStateId KickState = KickStateId.None;
    private Action[] KickAction;
    private bool IsJump = false;
    private Vector2 KickDirection;
    private float KickTimeCount = 0.0f;
    //private float KickExecCoolTimeCount = 0.0f;
    private bool fallStick = false;

    // ImputSystem関係
    private InputAction moveAction;
    private InputAction prekickAction;
    private InputAction kickmouseAction;
    private InputAction kickpadAction;

    void Start()
    {
        Leg.gameObject.SetActive(false);

        Rb = transform.parent.GetComponent<Rigidbody2D>();
        KickAction = new Action[(int)KickStateId.Length]
            { Kick_NoneAction, Kick_KickAction, Kick_LegReturnAction };

        PlayerInput input = GetComponent<PlayerInput>();
        InputActionMap map = input.currentActionMap;
        moveAction = map["Move"];
        prekickAction = map["PreKick"];
        kickmouseAction = map["Kick_Mouse"];
        kickpadAction = map["Kick_Pad"];
    }

    void Update()
    {
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
    }

    void Kick_NoneAction()
    {
        Vector2 move;
        Vector2 normal;

        if (prekickAction.triggered)
        {
            move = kickmouseAction.ReadValue<Vector2>();
            normal = move.normalized;

            if (move.sqrMagnitude >= KickExecSpeed * KickExecSpeed)
            {
                KickState = KickStateId.Kick;

                KickTimeCount = 0.0f;
                KickDirection = normal;

                Leg.position = transform.position;
                Leg.rotation = Quaternion.FromToRotation(Vector3.down, KickDirection);
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
            KickDirection = normal;

            Leg.position = transform.position;
            Leg.rotation = Quaternion.FromToRotation(Vector3.down, KickDirection);
            Leg.gameObject.SetActive(true);
        }
    }

    void Kick_KickAction()
    {
        if (KickTimeCount < KickTime)
        {
            KickTimeCount += Time.deltaTime;
            
            Vector2 vec = KickDirection * KickRange * KickTimeCount / KickTime;
            Leg.position = transform.position + (Vector3)vec;
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

            Vector2 vec = KickDirection * KickRange * KickTimeCount / KickReturnTime;
            Leg.position = transform.position + (Vector3)vec;
        }
        else
        {
            KickState = KickStateId.None;

            IsJump = false;
            Leg.gameObject.SetActive(false);
            Leg.position = transform.position;
        }
    }

    public void KickAddForce()
    {
        if (!IsJump)
        {
            IsJump = true;
            Vector2 resultVel;
            Vector2 jumpVel = -KickDirection * KickPower;
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

            Rb.AddForceAtPosition(resultVel, Leg.position, ForceMode2D.Impulse);

            AudioManager.Instance.PlaySe("ジャンプ");
        }
    }
}
