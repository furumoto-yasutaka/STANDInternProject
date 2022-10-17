using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange_AtStage : MonoBehaviour
{
    [SerializeField, RenameField("�g�p����g�����W�V�����v���n�u")]
    private GameObject transition;
    [SerializeField]
    private string sceneName = "";
    private int stageNum = 0;

    public void StartSceneChange(int index)
    {
        TransitionCallBack.SetTransitionCallBack(ChangeScene);

        GameObject parent = GameObject.Find("Canvas");
        GameObject t = Instantiate(transition, parent.transform);
        stageNum = index;

        GameObject obj = GameObject.FindGameObjectWithTag("InputLockManager");
        if (obj != null)
        {
            obj.GetComponent<InputLockManager>().LockAll();
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName + stageNum);
    }
}
