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
