using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controller : MonoBehaviour
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
    [SerializeField, RenameField("足オブジェクト")]
    private Transform Leg;

    [SerializeField, RenameField("プレイヤーの移動速度")]
    private float MoveSpeed = 2.0f;
    [SerializeField, RenameField("入力と逆方向へ加速時の減衰速度")]
    private float MoveDecaySpeed = 0.8f;
    [SerializeField, RenameField("プレイヤーの移動入力のしきい値")]
    private float MoveThreshold = 2.0f;
    [SerializeField, RenameField("蹴りを発動させる速度のしきい値")]
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
    [SerializeField, RenameField("キック時の回転速度")]
    private float KickAngularPower = 100.0f;

    [ReadOnly]
    public KickStateId KickState = KickStateId.None;
    private System.Action[] KickAction;
    private bool IsJump = false;
    private Vector2 KickDirection;
    private float KickTimeCount = 0.0f;
    //private float KickExecCoolTimeCount = 0.0f;
    private bool fallStick = false;

    void Start()
    {
        Body = transform.GetChild(0);
        Leg.gameObject.SetActive(false);

        Rb = GetComponent<Rigidbody2D>();
        KickAction = new System.Action[(int)KickStateId.Length]
            { Kick_NoneAction, Kick_KickAction, Kick_LegReturnAction };
    }

    void Update()
    {
        float decaySpeed;
        float defaultSpeed;

        if ((InputManager.GetValue(InputManager.KeyIdList.DPAD_H) < 0.0f || InputManager.GetValue(InputManager.KeyIdList.LS_H) < 0.0f)
            && Rb.velocity.x >= -MoveThreshold)
        {
            float inputValue = 0.0f;
            if (InputManager.GetValue(InputManager.KeyIdList.DPAD_H) < InputManager.GetValue(InputManager.KeyIdList.LS_H))
            {
                inputValue = Mathf.Abs(InputManager.GetValue(InputManager.KeyIdList.DPAD_H));
            }
            else
            {
                inputValue = Mathf.Abs(InputManager.GetValue(InputManager.KeyIdList.LS_H));
            }

            decaySpeed = Rb.velocity.x * -MoveDecaySpeed * Time.deltaTime * inputValue;
            defaultSpeed = -MoveSpeed * Time.deltaTime * inputValue;
            if (decaySpeed < defaultSpeed)
            {
                Rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                Rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }
        if ((InputManager.GetValue(InputManager.KeyIdList.DPAD_H) > 0.0f || InputManager.GetValue(InputManager.KeyIdList.LS_H) > 0.0f)
            && Rb.velocity.x <= MoveThreshold)
        {
            float inputValue = 0.0f;
            if (InputManager.GetValue(InputManager.KeyIdList.DPAD_H) > InputManager.GetValue(InputManager.KeyIdList.LS_H))
            {
                inputValue = InputManager.GetValue(InputManager.KeyIdList.DPAD_H);
            }
            else
            {
                inputValue = InputManager.GetValue(InputManager.KeyIdList.LS_H);
            }

            decaySpeed = Rb.velocity.x * MoveDecaySpeed * Time.deltaTime * inputValue;
            defaultSpeed = MoveSpeed * Time.deltaTime * inputValue;
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


        float horizontal = InputManager.GetValue(InputManager.KeyIdList.RS_H);
        float vertical = InputManager.GetValue(InputManager.KeyIdList.RS_V);
        Vector2 move = new Vector2(horizontal, vertical);

        if (move.sqrMagnitude < KickExecSpeed * KickExecSpeed)
        {
            fallStick = false;
        }
    }

    void Kick_NoneAction()
    {
        float horizontal = InputManager.GetValue(InputManager.KeyIdList.RS_H);
        float vertical = InputManager.GetValue(InputManager.KeyIdList.RS_V);
        Vector2 move = new Vector2(horizontal, vertical);
        Vector2 normal = move.normalized;

        if (move.sqrMagnitude >= KickExecSpeed * KickExecSpeed && !fallStick)
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

            Rb.velocity = resultVel;
            Rb.angularVelocity = Rb.velocity.x * -KickAngularPower;

            AudioManager.Instance.PlaySe("ジャンプ");
        }
    }

    public void KickAddForce(Vector2 angle)
    {
        if (!IsJump)
        {
            IsJump = true;
            Vector2 resultVel;
            Vector2 jumpVel = angle.normalized * KickPower * 2;
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

            AudioManager.Instance.PlaySe("ジャンプ");
        }
    }
}
