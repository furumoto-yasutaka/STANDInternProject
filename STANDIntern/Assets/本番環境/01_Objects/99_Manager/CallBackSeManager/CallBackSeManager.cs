/*******************************************************************************
*
*	�^�C�g���F	�R�[���o�b�N�ɂ��SE�Đ�	[ CallBackSeManager.cs ]
*
*	�쐬�ҁF	�Ö{ �ח�
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �R�[���o�b�N�̎Q�Ɛݒ���y�ɂ��邽�ߐÓI�֐��Ƃ��Ē�`�����Đ��������ꋓ�ɂ܂Ƃ߂Ă�������
/// </summary>
public class CallBackSeManager : MonoBehaviour
{
    //���U���g
    public static void PlayClickSe()
    {
        AudioManager.Instance.PlaySe("���艹(����)");
    }
}
