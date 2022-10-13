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

    private SceneNameEnum scene = SceneNameEnum.TitleScene;

    public void StartSceneChange(SceneNameEnum s)
    {
        scene = s;
        TransitionCallBack.SetTransitionCallBack(ChangeScene);

        GameObject parent = GameObject.Find("Canvas");
        GameObject t = Instantiate(transition, parent.transform);
        //t.transform.parent = parent.transform;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
