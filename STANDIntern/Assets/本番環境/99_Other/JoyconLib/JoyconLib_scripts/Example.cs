using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Example : MonoBehaviour
{
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;
    private Vector3 accMin = new Vector3(999.0f, 999.0f, 999.0f);
    private Vector3 accMax = new Vector3(-999.0f, -999.0f, -999.0f);

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    private void Update()
    {
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        foreach (var button in m_buttons)
        {
            if (m_joyconL.GetButton(button))
            {
                m_pressedButtonL = button;
            }
            if (m_joyconR.GetButton(button))
            {
                m_pressedButtonR = button;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_joyconL.SetRumble(160, 320, 0.6f, 200);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_joyconR.SetRumble(160, 320, 0.6f, 200);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            accMin = new Vector3(999.0f, 999.0f, 999.0f);
            accMax = new Vector3(-999.0f, -999.0f, -999.0f);
        }
    }

    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 40;

        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) が接続されていません");
            return;
        }

        GUILayout.BeginHorizontal(GUILayout.Width(1920));
        GUILayout.BeginVertical(GUILayout.Width(1080));

        foreach (var joycon in m_joycons)
        {
            var isLeft = joycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var key = isLeft ? "Z キー" : "X キー";
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;
            var stick = joycon.GetStick();
            var gyro = joycon.GetGyro();
            var accel = joycon.GetAccel();
            if (!isLeft)
            {
                if (accel.x < accMin.x) { accMin.x = accel.x; }
                else if (accel.x > accMax.x) { accMax.x = accel.x; }
                if (accel.y < accMin.y) { accMin.y = accel.y; }
                else if (accel.y > accMax.y) { accMax.y = accel.y; }
                if (accel.z < accMin.z) { accMin.z = accel.z; }
                else if (accel.z > accMax.z) { accMax.z = accel.z; }
            }
            var orientation = joycon.GetVector();

            GUILayout.Label(name);
            GUILayout.Label(key + "：振動");
            GUILayout.Label("押されているボタン：" + button);
            GUILayout.Label(string.Format("スティック：({0}, {1})", stick[0], stick[1]));
            GUILayout.Label("ジャイロ：" + gyro);
            GUILayout.Label("加速度：" + accel);
            GUILayout.Label("加速度(最小)：" + accMin.x.ToString("f3") + "　" + accMin.y.ToString("f3") + "　" + accMin.z.ToString("f3"));
            GUILayout.Label("加速度(最大)：" + accMax.x.ToString("f3") + "　" + accMax.y.ToString("f3") + "　" + accMax.z.ToString("f3"));
            GUILayout.Label("傾き：" + orientation + "\n");
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}