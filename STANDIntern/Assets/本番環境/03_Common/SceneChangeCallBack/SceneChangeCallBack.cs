/*******************************************************************************
*
*	タイトル：	シーン遷移用コールバッククラス	[ SceneChangeCallBack.cs ]
*
*	作成者：	古本 泰隆
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeCallBack : MonoBehaviour
{
    [SerializeField, RenameField("遷移先のシーン名")]
    private SceneNameEnum scene = SceneNameEnum.Title;

    [SerializeField, RenameField("使用するトランジションプレハブ")]
    private GameObject transition;

    public void BackButtonFunc()
    {
        FadeCallBack.SetTransitionFunc(SceneChange);

        GameObject parent = GameObject.Find("Canvas");
        GameObject t = Instantiate(transition, parent.transform);
        t.transform.parent = parent.transform;
    }

    public void RestartButtonFunc()
    {
        FadeCallBack.SetTransitionFunc(SceneChangeRestart);

        GameObject parent = GameObject.Find("Canvas");
        GameObject t = Instantiate(transition, parent.transform);
        t.transform.parent = parent.transform;
    }

    public void SceneChange()
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void SceneChangeRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
