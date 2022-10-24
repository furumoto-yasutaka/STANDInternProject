using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange_AtBattleStage : MonoBehaviour
{
    [SerializeField, RenameField("使用するトランジションプレハブ")]
    private GameObject transition;
    [SerializeField]
    private string sceneName = "";
    private ButtonSelectManager buttonSelectManager;

    void Start()
    {
        buttonSelectManager = GetComponent<ButtonSelectManager>();
    }

    public void StartSceneChange()
    {
        TransitionCallBack.SetTransitionCallBack(ChangeSceneCallBack);

        GameObject parent = GameObject.Find("Canvas");
        GameObject t = Instantiate(transition, parent.transform);

        GameObject obj = GameObject.FindGameObjectWithTag("InputLockManager");
        if (obj != null)
        {
            obj.GetComponent<InputLockManager>().LockAll();
        }
    }

    public void ChangeSceneCallBack()
    {
        SceneManager.LoadScene(sceneName + buttonSelectManager.SelectIndex);
    }
}
