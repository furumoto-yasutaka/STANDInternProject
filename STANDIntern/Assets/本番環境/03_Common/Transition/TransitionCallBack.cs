using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCallBack : MonoBehaviour
{
    // コールバック時に呼ぶ関数を保持する
    private static System.Action transitionCallBack;

    /// <summary>
    /// コールバック時に呼びたい関数を設定
    /// </summary>
    /// <param name="action">関数ポインタ</param>
    public static void SetTransitionCallBack(System.Action action)
    {
        transitionCallBack = action;
    }

    public void FadeInCompleteCallBack()
    {
        // コールバックが設定されていた場合実行する
        if (transitionCallBack != null)
        {
            transitionCallBack();
            transitionCallBack = null;
        }
    }

    public void FadeOutCompleteCallBack()
    {
        // これ以降使用しないので自身を削除する
        Destroy(gameObject);
    }
}
