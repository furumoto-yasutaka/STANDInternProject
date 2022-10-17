using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [SerializeField, RenameField("���I�u�W�F�N�g")]
    private Transform Leg;

    [SerializeField, RenameField("�v���C���[�̈ړ����x")]
    private float MoveSpeed = 2.0f;
    [SerializeField, RenameField("���͂Ƌt�����։������̌������x")]
    private float MoveDecaySpeed = 0.8f;
    [SerializeField, RenameField("�v���C���[�̈ړ����͂̂������l")]
    private float MoveThreshold = 2.0f;
    [SerializeField, RenameField("�R��𔭓������鑬�x�̂������l")]
    private float KickExecSpeed = 0.7f;
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
    [SerializeField, RenameField("�L�b�N�{��(X)")]
    private float KickMagX = 1.3f;
    [SerializeField, RenameField("�L�b�N���̉�]���x")]
    private float KickAngularPower = 100.0f;

    [ReadOnly]
    public KickStateId KickState = KickStateId.None;
    private System.Action[] KickAction;
    private bool IsJump = false;
    private Vector2 KickDirection;
    private float KickTimeCount = 0.0f;
    //private float KickExecCoolTimeCount = 0.0f;


    // JoyCon�֌W
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Vector2 jumpAngleL;
    private Vector2 jumpAngleR;

    private float rotL = 0.0f;
    private float rotR = 0.0f;
    private Vector3 v;

    void Start()
    {
        Leg.gameObject.SetActive(false);

        Rb = transform.parent.GetComponent<Rigidbody2D>();
        KickAction = new System.Action[(int)KickStateId.Length]
            { Kick_NoneAction, Kick_KickAction, Kick_LegReturnAction };

        // JoyCon�֌W
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    void Update()
    {
        float decaySpeed;
        float defaultSpeed;

        if ((Input.GetKey(KeyCode.A)
            //||
            //m_joyconL.GetButton(Joycon.Button.DPAD_LEFT) ||
            //m_joyconL.GetStick()[0] < -0.5f ||
            //m_joyconR.GetButton(Joycon.Button.DPAD_LEFT) ||
            //m_joyconR.GetStick()[0] < -0.5f
            ) &&
            Rb.velocity.x >= -MoveThreshold)
        {
            decaySpeed = Rb.velocity.x * -MoveDecaySpeed * Time.deltaTime;
            defaultSpeed = -MoveSpeed * Time.deltaTime;
            if (decaySpeed < defaultSpeed)
            {
                Rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                Rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }
        if ((Input.GetKey(KeyCode.D)
            //||
            //m_joyconL.GetButton(Joycon.Button.DPAD_RIGHT) ||
            //m_joyconL.GetStick()[0] > 0.5f ||
            //m_joyconR.GetButton(Joycon.Button.DPAD_RIGHT) ||
            //m_joyconR.GetStick()[0] > 0.5f
            ) &&
            Rb.velocity.x <= MoveThreshold)
        {
            decaySpeed = Rb.velocity.x * MoveDecaySpeed * Time.deltaTime;
            defaultSpeed = MoveSpeed * Time.deltaTime;
            if (decaySpeed > defaultSpeed)
            {
                Rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                Rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }

        //Vector3 accel = m_joyconL.GetAccel();
        //float sqrMag = accel.sqrMagnitude;
        //if (sqrMag <= 1.04f && sqrMag >= 0.92f/* && gyroZDistance <= 1.5f*/)
        //{
        //    //jumpAngleL = new Vector2(accel.z, -accel.y).normalized;
        //    rotL = Mathf.Atan2(accel.y, accel.z) * Mathf.Rad2Deg;
        //    jumpAngleL = Quaternion.Euler(0, 0, -rotL) * Vector2.right;
        //}

        //accel = m_joyconR.GetAccel();
        //v = accel;
        //sqrMag = accel.sqrMagnitude;
        //if (sqrMag <= 1.04f && sqrMag >= 0.92f/* && gyroZDistance <= 1.5f*/)
        //{
        //    //jumpAngleR = new Vector2(accel.z, -accel.y).normalized;
        //    rotR = Mathf.Atan2(accel.y, accel.z) * Mathf.Rad2Deg;
        //    Debug.Log(rotR);
        //    jumpAngleR = Quaternion.Euler(0, 0, -rotR) * Vector2.right;
        //}

        KickAction[(int)KickState]();
    }

    void Kick_NoneAction()
    {
        if (InputManager.GetKeyPress(InputManager.KeyIdList.Mouse_LB))
        {
            float horizontal = InputManager.GetValue(InputManager.KeyIdList.Mouse_X);
            float vertical = InputManager.GetValue(InputManager.KeyIdList.Mouse_Y);
            Vector2 move = new Vector2(horizontal, vertical);
            Vector2 normal = move.normalized;

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


        
        //Vector3 gyro = m_joyconL.GetGyro();
        //if (gyro.z <= -5.0f)
        //{
        //    KickState = KickStateId.Kick;

        //    Vector3 accel = m_joyconR.GetAccel();
        //    if (accel.z > 0.5f)
        //    {
        //        float num = accel.z - 0.5f * 10;
        //        jumpAngleL = Quaternion.Euler(0.0f, 0.0f, num) * jumpAngleL;
        //    }
        //    else if (accel.z < -0.5f)
        //    {
        //        float num = accel.z + 0.5f * 10;
        //        jumpAngleL = Quaternion.Euler(0.0f, 0.0f, num) * jumpAngleL;
        //    }

        //    KickTimeCount = 0.0f;
        //    KickDirection = jumpAngleL;

        //    Leg.position = transform.position;
        //    Leg.rotation = Quaternion.FromToRotation(Vector3.up, jumpAngleL);
        //    Leg.gameObject.SetActive(true);
        //}

        //gyro = m_joyconR.GetGyro();
        //if (gyro.z <= -5.0f)
        //{
        //    KickState = KickStateId.Kick;

        //    Vector3 accel = m_joyconR.GetAccel();
        //    if (accel.z > 0.5f)
        //    {
        //        float num = accel.z - 0.5f * 10;
        //        jumpAngleL = Quaternion.Euler(0.0f, 0.0f, num) * jumpAngleL;
        //    }
        //    else if (accel.z < -0.5f)
        //    {
        //        float num = accel.z + 0.5f * 10;
        //        jumpAngleL = Quaternion.Euler(0.0f, 0.0f, num) * jumpAngleL;
        //    }

        //    KickTimeCount = 0.0f;
        //    KickDirection = jumpAngleR;

        //    Leg.position = transform.position;
        //    Leg.rotation = Quaternion.FromToRotation(Vector3.up, jumpAngleR);
        //    Leg.gameObject.SetActive(true);
        //}
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

            //Rb.velocity = resultVel;
            //Rb.angularVelocity = Rb.velocity.x * -KickAngularPower;
            Rb.AddForceAtPosition(resultVel, Leg.position, ForceMode2D.Impulse);

            AudioManager.Instance.PlaySe("�W�����v");
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

            AudioManager.Instance.PlaySe("�W�����v");
        }
    }

    //private void OnGUI()
    //{
    //    var style = GUI.skin.GetStyle("label");
    //    style.fontSize = 32;

    //    GUILayout.BeginHorizontal(GUILayout.Width(1920));
    //    GUILayout.BeginVertical(GUILayout.Width(1080));

    //    GUILayout.Label(rotR.ToString("f3"));
    //    GUILayout.Label(jumpAngleR.x.ToString("f3") + "�@" + jumpAngleR.y.ToString("f3"));
    //    GUILayout.Label(v.x.ToString("f3") + "�@" + v.y.ToString("f3") + "�@" + v.z.ToString("f3"));

    //    GUILayout.EndVertical();
    //    GUILayout.EndHorizontal();
    //}
}
