/*******************************************************************************
*
*	タイトル：	コールバックによるSE再生	[ CallBackSeManager.cs ]
*
*	作成者：	古本 泰隆
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コールバックの参照設定を楽にするため静的関数として定義した再生処理を一挙にまとめておくもの
/// </summary>
public class CallBackSeManager : MonoBehaviour
{
    //リザルト
    public static void PlayClickSe()
    {
        AudioManager.Instance.PlaySe("決定音(試し)");
    }
}
