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

    [SerializeField]
    private bool isCanvasParent = true;

    public void StartSceneChange(SceneNameEnum s)
    {
        scene = s;
        TransitionCallBack.SetTransitionCallBack(ChangeScene);

        if (isCanvasParent)
        {
            GameObject parent = GameObject.Find("Canvas");
            GameObject t = Instantiate(transition, parent.transform);
        }
        else
        {
            Instantiate(transition);
        }
    }

    public void StartSceneChange()
    {
        TransitionCallBack.SetTransitionCallBack(ChangeScene);

        if (isCanvasParent)
        {
            GameObject parent = GameObject.Find("Canvas");
            GameObject t = Instantiate(transition, parent.transform);
        }
        else
        {
            Instantiate(transition);
        }

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
