/*******************************************************************************
*
*	タイトル：	トランジション設定	[ TransitionStarter.cs ]
*
*	作成者：	古本 泰隆
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionStarter : MonoBehaviour
{
    [SerializeField, RenameField("使用するトランジションプレハブ")]
    private GameObject transition;

    void Start()
    {
        // 生成して親子関係を設定する
        GameObject parent = GameObject.Find("Canvas");
        Instantiate(transition, parent.transform);

        // この後は不要なので削除する
        Destroy(gameObject);
    }
}
