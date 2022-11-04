using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleCountDown : MonoBehaviour
{
    private TextMeshProUGUI second; // 秒数を表すテキスト
    private Animator animator;      // テキストのアニメーター
    private int count = 5;          // カウントダウンの秒数

    public int Count { get { return count; } }

    void Start()
    {
        second = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
        second.text = count.ToString();
    }

    /// <summary>
    /// テキストを次の数字に変える
    /// </summary>
    public void NextNumber()
    {
        count--;
        second.text = count.ToString();
    }

    /// <summary>
    /// アニメーションのループを次で止めるかどうか判断する
    /// </summary>
    public void CheckStop()
    {
        // カウントが１(このカウントで終了)の場合フラグを立てる
        if (count == 1)
        {
            animator.SetBool("Stop", true);
        }
    }
}
