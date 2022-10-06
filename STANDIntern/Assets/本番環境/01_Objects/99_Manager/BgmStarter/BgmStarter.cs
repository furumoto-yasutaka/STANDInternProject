/*******************************************************************************
*
*	タイトル：	シーン遷移時BGM設定	[ BgmStarter.cs ]
*
*	作成者：	古本 泰隆
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmStarter : MonoBehaviour
{
    [SerializeField, RenameField("再生するBGMの音名")]
    private string BgmName;

    void Start()
    {
        AudioManager.Instance.PlayBgm(BgmName, true);
        
        // この後は不要なので削除する
        Destroy(gameObject);
    }
}
