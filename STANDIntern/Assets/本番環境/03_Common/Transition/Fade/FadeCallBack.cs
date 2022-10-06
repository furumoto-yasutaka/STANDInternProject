/*******************************************************************************
*
*	タイトル：	フェード用コールバック	[ FadeCallBack.cs ]
*
*	作成者：	古本 泰隆
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FadeCallBack : MonoBehaviour
{
    // コールバック時に呼ぶ関数を保持する変数
    private static Action transitionFunc;

    /// <summary>
    /// コールバック時に呼びたい関数を設定
    /// </summary>
    /// <param name="action">関数ポインタ</param>
    public static void SetTransitionFunc(Action action)
    {
        transitionFunc = action;
    }

    public void FadeInCompleteFunc()
    {
        // コールバックが設定されていた場合実行する
        if (transitionFunc != null)
        {
            transitionFunc();
            transitionFunc = null;
        }
    }

    public void FadeOutCompleteFunc()
    {
        // これ以降使用しないので自身を削除する
        Destroy(gameObject);
    }
}
