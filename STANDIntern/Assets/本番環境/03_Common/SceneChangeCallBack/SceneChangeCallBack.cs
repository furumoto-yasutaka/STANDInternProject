/*******************************************************************************
*
*	�^�C�g���F	�V�[���J�ڗp�R�[���o�b�N�N���X	[ SceneChangeCallBack.cs ]
*
*	�쐬�ҁF	�Ö{ �ח�
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeCallBack : MonoBehaviour
{
    [SerializeField, RenameField("�J�ڐ�̃V�[����")]
    private SceneNameEnum scene = SceneNameEnum.Title;

    [SerializeField, RenameField("�g�p����g�����W�V�����v���n�u")]
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
