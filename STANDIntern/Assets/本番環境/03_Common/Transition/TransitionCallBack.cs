using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCallBack : MonoBehaviour
{
    // �R�[���o�b�N���ɌĂԊ֐���ێ�����
    private static System.Action transitionCallBack;

    /// <summary>
    /// �R�[���o�b�N���ɌĂт����֐���ݒ�
    /// </summary>
    /// <param name="action">�֐��|�C���^</param>
    public static void SetTransitionCallBack(System.Action action)
    {
        transitionCallBack = action;
    }

    public void FadeInCompleteCallBack()
    {
        // �R�[���o�b�N���ݒ肳��Ă����ꍇ���s����
        if (transitionCallBack != null)
        {
            transitionCallBack();
            transitionCallBack = null;
        }
    }

    public void FadeOutCompleteCallBack()
    {
        // ����ȍ~�g�p���Ȃ��̂Ŏ��g���폜����
        Destroy(gameObject);
    }
}
