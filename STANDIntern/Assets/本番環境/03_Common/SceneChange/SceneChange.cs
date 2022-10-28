using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �V�[���`�F���W���s���X�N���v�g�����I�u�W�F�N�g�ƈꏏ�ɃA�^�b�`����
/// </summary>

public class SceneChange : MonoBehaviour
{
    [SerializeField, RenameField("�g�p����g�����W�V�����v���n�u")]
    private GameObject transition;
    [SerializeField, RenameField("�J�ڐ�V�[��(OnClick�p)")]
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
