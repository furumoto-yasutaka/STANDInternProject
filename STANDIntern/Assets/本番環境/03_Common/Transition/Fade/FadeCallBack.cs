/*******************************************************************************
*
*	�^�C�g���F	�t�F�[�h�p�R�[���o�b�N	[ FadeCallBack.cs ]
*
*	�쐬�ҁF	�Ö{ �ח�
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FadeCallBack : MonoBehaviour
{
    // �R�[���o�b�N���ɌĂԊ֐���ێ�����ϐ�
    private static Action transitionFunc;

    /// <summary>
    /// �R�[���o�b�N���ɌĂт����֐���ݒ�
    /// </summary>
    /// <param name="action">�֐��|�C���^</param>
    public static void SetTransitionFunc(Action action)
    {
        transitionFunc = action;
    }

    public void FadeInCompleteFunc()
    {
        // �R�[���o�b�N���ݒ肳��Ă����ꍇ���s����
        if (transitionFunc != null)
        {
            transitionFunc();
            transitionFunc = null;
        }
    }

    public void FadeOutCompleteFunc()
    {
        // ����ȍ~�g�p���Ȃ��̂Ŏ��g���폜����
        Destroy(gameObject);
    }
}
