using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class InputManager
{
    //-----------列挙型-----------
    /// <summary> 各キーのID </summary>
    public enum KeyIdList
    {
        A = 0,
        B,
        X,
        Y,
        LB,
        RB,
        Back,
        Start,
        LSB,
        RSB,
        LT,
        RT,
        LS_H,
        LS_V,
        RS_H,
        RS_V,
        Mouse_X,
        Mouse_Y,
        Mouse_Z,
        Mouse_LB,
        Mouse_RB,
        Mouse_CB,
        Length,
    }

    /// <summary> 各キーの入力値取得のための文字列 </summary>
    public static readonly string[] KeyNameList =
{
        "joystick button 0",    // A
        "joystick button 1",    // B
        "joystick button 2",    // X
        "joystick button 3",    // Y
        "joystick button 4",    // LB
        "joystick button 5",    // RB
        "joystick button 6",    // Back
        "joystick button 7",    // Start
        "joystick button 8",    // LSB
        "joystick button 9",    // RSB
        "L_Trigger",            // LT
        "R_Trigger",            // RT
        "L_Stick_H",            // LS_H
        "L_Stick_V",            // LS_V
        "R_Stick_H",            // RS_H
        "R_Stick_V",            // RS_V
        "Mouse X",              // Mouse_X
        "Mouse Y",              // Mouse_Y
        "Mouse ScrollWheel",    // Mouse_Z
        "",                     // Mouse_LB(入力の取得方法が違うため文字列はなし)
        "",                     // Mouse_RB(入力の取得方法が違うため文字列はなし)
        "",                     // Mouse_CB(入力の取得方法が違うため文字列はなし)
    };

    //-----------パラメータ-----------
    /// <summary> キーの情報 </summary>
    private static Key[] keys = new Key[(int)KeyIdList.Length];
    /// <summary> 入力可能かどうか </summary>
    [SerializeField, ReadOnly]
    private static bool isCanInput = true;

    
    static InputManager()
    {
        // 毎フレーム呼んでもらう為にリストに追加
        OrderedUpdate.OnPreUpdate += OrderUpdate;

        int i = 0;
        // ボタン
        keys[i++] = new Button((int)KeyIdList.A);
        keys[i++] = new Button((int)KeyIdList.B);
        keys[i++] = new Button((int)KeyIdList.X);
        keys[i++] = new Button((int)KeyIdList.Y);
        keys[i++] = new Button((int)KeyIdList.LB);
        keys[i++] = new Button((int)KeyIdList.RB);
        keys[i++] = new Button((int)KeyIdList.Back);
        keys[i++] = new Button((int)KeyIdList.Start);
        keys[i++] = new Button((int)KeyIdList.LSB);
        keys[i++] = new Button((int)KeyIdList.RSB);
        // トリガー
        keys[i++] = new Trigger((int)KeyIdList.LT);
        keys[i++] = new Trigger((int)KeyIdList.RT);
        // スティック
        keys[i++] = new Stick((int)KeyIdList.LS_H);
        keys[i++] = new Stick((int)KeyIdList.LS_V);
        keys[i++] = new Stick((int)KeyIdList.RS_H);
        keys[i++] = new Stick((int)KeyIdList.RS_V);
        // マウス移動
        keys[i++] = new MouseMove((int)KeyIdList.Mouse_X);
        keys[i++] = new MouseMove((int)KeyIdList.Mouse_Y);
        keys[i++] = new MouseMove((int)KeyIdList.Mouse_Z);
        // マウスボタン
        keys[i++] = new MouseButton((int)KeyIdList.Mouse_LB);
        keys[i++] = new MouseButton((int)KeyIdList.Mouse_RB);
        keys[i++] = new MouseButton((int)KeyIdList.Mouse_CB);
    }

    // 更新処理(別クラスに呼び出しを任せる)
    static void OrderUpdate()
    {
        if (!isCanInput) { return; }

        foreach (Key key in keys)
        {
            if (key.IsCanInput)
            {
                key.UpdateInput();
            }
        }
    }

    /// <summary> キーが押した瞬間かどうか取得 </summary>
    public static bool GetKeyDown(KeyIdList id)
    {
        return keys[(int)id].KeyDown;
    }

    /// <summary> キーが押しているかどうか取得 </summary>
    public static bool GetKeyPress(KeyIdList id)
    {
        return keys[(int)id].KeyPress;
    }

    /// <summary> キーを離した瞬間かどうか取得 </summary>
    public static bool GetKeyUp(KeyIdList id)
    {
        return keys[(int)id].KeyUp;
    }

    /// <summary> キーの入力値を取得 </summary>
    public static float GetValue(KeyIdList id)
    {
        return keys[(int)id].Value;
    }

    /// <summary> 全ての入力を無効化・有効化できます </summary>
    public static void SetCanInput(bool isCan)
    {
        isCanInput = isCan;
        if (!isCan)
        {
            foreach (Key key in keys)
            {
                key.CancelFlag();
            }
        }
    }

    /// <summary> 特定の入力を無効化・有効化できます </summary>
    public static void SetCanInput(bool isCan, KeyIdList id)
    {
        keys[(int)id].IsCanInput = isCan;
        if (!isCan)
        {
            keys[(int)id].CancelFlag();
        }
    }
}


#region キー用クラス
/// <summary> 全てのキーの基底クラス </summary>
public abstract class Key
{
    public int Id;                  // キー番号
    public string InputName;        // 入力取得時の名前             
    public bool KeyDown = false;    // キーを押した瞬間かどうか
    public bool KeyPress = false;   // キーを押しているかどうか
    public bool KeyUp = false;      // キーを離した瞬間かどうか
    public float Value = 0.0f;      // 入力値
    public bool IsCanInput = true;  // 入力を有効にするかどうか

    public Key(int id)
    {
        Id = id;
        InputName = InputManager.KeyNameList[Id];
    }
    public abstract void UpdateInput(); // 入力状況の更新
    public abstract void CancelFlag();  // 全ての入力状態を無効にする
}

//-----------↓派生クラス↓-----------

/// <summary> ボタン専用 </summary>
public class Button : Key
{
    public Button(int id) : base(id) { }

    public override void UpdateInput()
    {
        KeyDown = Input.GetKeyDown(InputName);
        KeyPress = Input.GetKey(InputName);
        KeyUp = Input.GetKeyUp(InputName);
        Value = KeyPress ? 1.0f : 0.0f;
    }
    public override void CancelFlag()
    {
        KeyDown = false;
        KeyPress = false;
        KeyUp = false;
        Value = 0.0f;
    }
}

/// <summary> トリガー専用 </summary>
public class Trigger : Key
{
    private bool isPreviousPush = false;    // 前フレームに入力があったか
    private const float threshold = 0.7f;   // ボタンのように押した扱いとするしきい値

    public Trigger(int id) : base(id) { }
    public override void UpdateInput()
    {
        Value = Input.GetAxis(InputName);

        KeyDown = !isPreviousPush && Value > threshold;
        KeyPress = Value > threshold;
        KeyUp = isPreviousPush && Value <= threshold;
        isPreviousPush = KeyPress;
    }
    public override void CancelFlag()
    {
        KeyDown = false;
        KeyPress = false;
        KeyUp = false;
        Value = 0.0f;
        isPreviousPush = false;
    }
}

/// <summary> スティック専用 </summary>
public class Stick : Key
{
    private bool isPreviousPush = false;    // 前フレームに入力があったか
    private const float threshold = 0.7f;   // ボタンのように押した扱いとするしきい値

    public Stick(int id) : base(id) { }
    public override void UpdateInput()
    {
        Value = Input.GetAxis(InputName);

        KeyDown = !isPreviousPush && Mathf.Abs(Value) > threshold;
        KeyPress = Mathf.Abs(Value) > threshold;
        KeyUp = isPreviousPush && Mathf.Abs(Value) <= threshold;
        isPreviousPush = KeyPress;
    }
    public override void CancelFlag()
    {
        KeyDown = false;
        KeyPress = false;
        KeyUp = false;
        Value = 0.0f;
        isPreviousPush = false;
    }
}

/// <summary> マウス移動値専用 </summary>
public class MouseMove : Key
{
    private bool isPreviousPush = false;    // 前フレームに入力があったか
    private const float threshold = 0.7f;   // ボタンのように押した扱いとするしきい値

    public MouseMove(int id) : base(id) { }
    public override void UpdateInput()
    {
        Value = Input.GetAxis(InputName);

        KeyDown = !isPreviousPush && Mathf.Abs(Value) > threshold;
        KeyPress = Mathf.Abs(Value) > threshold;
        KeyUp = isPreviousPush && Mathf.Abs(Value) <= threshold;
        isPreviousPush = KeyPress;
    }
    public override void CancelFlag()
    {
        KeyDown = false;
        KeyPress = false;
        KeyUp = false;
        Value = 0.0f;
        isPreviousPush = false;
    }
}

/// <summary> マウスボタン専用 </summary>
public class MouseButton : Key
{
    public MouseButton(int id) : base(id) { }
    public override void UpdateInput()
    {
        int num = Id - (int)InputManager.KeyIdList.Mouse_LB;
        bool isPush = Input.GetMouseButton(num);
        Value = isPush ? 1 : 0;

        KeyDown = Input.GetMouseButtonDown(num);
        KeyPress = isPush;
        KeyUp = Input.GetMouseButtonUp(num);
    }
    public override void CancelFlag()
    {
        KeyDown = false;
        KeyPress = false;
        KeyUp = false;
        Value = 0.0f;
    }
}
#endregion
