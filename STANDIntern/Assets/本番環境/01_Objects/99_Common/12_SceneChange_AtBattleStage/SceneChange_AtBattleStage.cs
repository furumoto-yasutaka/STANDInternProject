using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange_AtBattleStage : MonoBehaviour
{
    [SerializeField, RenameField("�g�p����g�����W�V�����v���n�u")]
    private GameObject transition;
    [SerializeField]
    private string sceneName = "";
    private ButtonSelectManager buttonSelectManager;

    [SerializeField]
    private bool isCanvasParent = true;

    void Start()
    {
        buttonSelectManager = GetComponent<ButtonSelectManager>();
    }

    public void StartSceneChange()
    {
        TransitionCallBack.SetTransitionCallBack(ChangeSceneCallBack);

        if (!isCanvasParent)
        {
            GameObject parent = GameObject.Find("Canvas");
            Instantiate(transition, parent.transform);
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

    public void ChangeSceneCallBack()
    {
        SceneManager.LoadScene(sceneName + (buttonSelectManager.SelectIndex + 1));
    }
}
