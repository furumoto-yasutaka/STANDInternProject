/*******************************************************************************
*
*	�^�C�g���F	�g�����W�V�����ݒ�	[ TransitionStarter.cs ]
*
*	�쐬�ҁF	�Ö{ �ח�
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionStarter : MonoBehaviour
{
    [SerializeField, RenameField("�g�p����g�����W�V�����v���n�u")]
    private GameObject transition;

    void Start()
    {
        // �������Đe�q�֌W��ݒ肷��
        GameObject parent = GameObject.Find("Canvas");
        Instantiate(transition, parent.transform);

        // ���̌�͕s�v�Ȃ̂ō폜����
        Destroy(gameObject);
    }
}
