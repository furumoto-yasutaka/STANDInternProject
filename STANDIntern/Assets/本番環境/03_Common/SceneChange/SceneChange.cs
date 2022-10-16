using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーンチェンジを行うスクリプトを持つオブジェクトと一緒にアタッチする
/// </summary>

public class SceneChange : MonoBehaviour
{
    [SerializeField, RenameField("使用するトランジションプレハブ")]
    private GameObject transition;
    [SerializeField, RenameField("遷移先シーン(OnClick用)")]
    private SceneNameEnum scene = SceneNameEnum.TitleScene;

    public void StartSceneChange(SceneNameEnum s)
    {
        scene = s;
        TransitionCallBack.SetTransitionCallBack(ChangeScene);

        GameObject parent = GameObject.Find("Canvas");
        GameObject t = Instantiate(transition, parent.transform);
    }

    public void StartSceneChange()
    {
        TransitionCallBack.SetTransitionCallBack(ChangeScene);

        GameObject parent = GameObject.Find("Canvas");
        GameObject t = Instantiate(transition, parent.transform);

        GameObject obj = GameObject.FindGameObjectWithTag("InputLockManager");
        if (obj != null)
        {
            obj.GetComponent<InputLockManager>().LockAll();
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
