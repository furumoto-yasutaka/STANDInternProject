/*******************************************************************************
*
*	タイトル：	FPS表示	[ FPS.cs ]
*
*	作成者：	古本 泰隆
*
*******************************************************************************/
using UnityEngine;

public class FPS : MonoBehaviour
{
#if UNITY_EDITOR
    private const float threshold = 1.0f;                   // しきい値(1秒)
    private float countTime = 0.0f;                         // 時間計測用
    private float countFrame = 0;                           // フレーム計測用
    private string textFps = "";                            // 表示用
    private GUIStyle textStyle = new GUIStyle();            // 書式設定用
    [SerializeField]
    private int fontSize = 30;                              // フォントサイズ
    [SerializeField]
    private Color fontColor = new Color(0, 0, 0, 255);      // フォントカラー
    [SerializeField]
    private Vector2 drawPos = new Vector2(10.0f, 10.0f);    // 表示位置(左上基準)
    
    void Start()
    {
        // フォントサイズ設定
        textStyle.fontSize = fontSize;
        // フォントカラー設定
        GUIStyleState textStyleState = new GUIStyleState();
        textStyleState.textColor = fontColor;
        textStyle.normal = textStyleState;
    }

    void Update()
    {
        // 経過時間とフレームを加算
        countTime += Time.deltaTime;
        countFrame++;

        // 経過時間がしきい値以上になったらFPSを更新
        if (countTime >= threshold)
        {
            float fps = countFrame / countTime;
            textFps = fps.ToString("F1");   // 小数点以下を1桁表示

            countTime -= threshold;
            countFrame = 0;
        }
    }

    /// <summary>
    /// 表示処理
    /// </summary>
    void OnGUI()
    {
        GUI.Label(new Rect(drawPos, new Vector2()), textFps, textStyle);
    }
#endif
}
