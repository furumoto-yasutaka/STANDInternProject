/*******************************************************************************
*
*	�^�C�g���F	�V�[���J�ڎ�BGM�ݒ�	[ BgmStarter.cs ]
*
*	�쐬�ҁF	�Ö{ �ח�
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmStarter : MonoBehaviour
{
    [SerializeField, RenameField("�Đ�����BGM�̉���")]
    private string BgmName;

    void Start()
    {
        AudioManager.Instance.PlayBgm(BgmName, true);
        
        // ���̌�͕s�v�Ȃ̂ō폜����
        Destroy(gameObject);
    }
}
